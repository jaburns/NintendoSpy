//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Bandai Pippin Controller Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// Uncomment this to spy on Controller 1
//#define CURRENT_SPY_ADDRESS 0xF
// Uncomment this to spy on Controller 2
//#define CURRENT_SPY_ADDRESS 0xE

// ---------- Uncomment this for debugging ouput --------------
//#define DEBUG
// ---------- Uncomment this for a mostly complete general purpose ADB sniffer  --------------
//#define SNIFFER

#include "TimerOne.h"// https://code.google.com/archive/p/arduino-timerone/downloads

#define BUFFER_SIZE 45

#define ADB_PIN 2

#define WAITING_FOR_ATTENTION 0
#define WAITING_FOR_SYNC 1
#define READING_COMMAND_BITS 2
#define WAITING_FOR_CMD_STOP 3
#define WAITING_FOR_DATA_START 4
#define WAITING_DATA_START_BIT 5
#define READING_DATA_BITS 6
#define COULD_BE_DATA_STOP_BIT 7

#ifndef CURRENT_SPY_ADDRESS
#define CURRENT_SPY_ADDRESS 0xF
#endif

struct packet
{
  byte commandType = 0;
  byte commandAddress = 0;
  byte commandRegister = 0;
  bool commandStop = false;
  bool HasData = false;
  bool dataStart = false;
  byte numBytes = 0;
  byte data[9];
  bool dataStop = false;
  int count  = 0;
  int syncTiming = 0;
};

volatile packet* buffer[BUFFER_SIZE];
uint8_t head, tail;
volatile packet* currentReadPacket = NULL;

volatile packet* currentWritePacket = NULL;
volatile unsigned long diff;
volatile unsigned int count = 0, state = WAITING_FOR_ATTENTION;
volatile unsigned char command;

byte rawData[16][27];

void setup() {
  pinMode(ADB_PIN, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(ADB_PIN), adbStateChanged, CHANGE);
  Timer1.initialize(10000);
  Timer1.stop();
  Timer1.restart();
  Timer1.detachInterrupt();
  Serial.begin(115200);
  delay(200);

}

void loop() {

  if (state == WAITING_FOR_DATA_START)
  {
    unsigned long _diff = TCNT1 >> 1;
    if (_diff > 260)
    {
      currentWritePacket->HasData = false;
      volatile packet* donePacket = currentWritePacket;
      state = WAITING_FOR_ATTENTION;
      currentWritePacket = NULL;
      uint8_t i = head + 1;
      if (i >= BUFFER_SIZE) i = 0;
      if (i != tail) {
        buffer[i] = donePacket;
        head = i;
      }
    }
  }
  else if (state == COULD_BE_DATA_STOP_BIT)
  {
    unsigned long _diff = TCNT1 >> 1;
    if (_diff > 50)
    {
      currentWritePacket->dataStop = true;
      volatile packet* donePacket = currentWritePacket;
      state = WAITING_FOR_ATTENTION;
      currentWritePacket = NULL;
      uint8_t i = head + 1;
      if (i >= BUFFER_SIZE) i = 0;
      if (i != tail) {
        buffer[i] = donePacket;
        head = i;
      }
    }  
  }
  
  uint8_t i = tail;
  if (i != head)
  {
    i++;
    if (i >= BUFFER_SIZE) i = 0;
    currentReadPacket = buffer[i];
    tail = i;

#ifdef SNIFFER
    Serial.print(currentReadPacket->commandType, HEX);
    Serial.print(" ");
    Serial.print(currentReadPacket->commandAddress, HEX);
    Serial.print(" ");
    Serial.print(currentReadPacket->commandRegister, HEX);
    Serial.print(" ");
    Serial.print(currentReadPacket->syncTiming);
    Serial.print(" ");
    Serial.print(currentReadPacket->commandStop ? "S" : "-");
    Serial.print(" ");
    Serial.print(currentReadPacket->HasData ? "D" : "N");
    Serial.print(" ");
    Serial.print(currentReadPacket->dataStart ? "S" : "-");
    Serial.print(" ");
    Serial.print(currentReadPacket->numBytes, HEX);
    Serial.print(" ");
    Serial.print(currentReadPacket->count);
    Serial.print(" ");
    for(int j = 0; j < currentReadPacket->numBytes; ++j)
    {
      Serial.print(currentReadPacket->data[j], HEX);
      Serial.print(" ");
    }
    Serial.println(currentReadPacket->dataStop ? "S" : "-");
#else
    if (currentReadPacket->HasData && currentReadPacket->numBytes == 4 && (currentReadPacket->commandAddress == 0xF || currentReadPacket->commandAddress == 0xE))    {
      for(int j = 0; j < 3; ++j)
      {
        for(int k = 0; k < 8; ++k)
          rawData[currentReadPacket->commandAddress][(j*8)+k] = (currentReadPacket->data[j] & (1 << k)) != 0;

        rawData[currentReadPacket->commandAddress][24] = (currentReadPacket->data[3] & 0b00000001) != 0;
        rawData[currentReadPacket->commandAddress][25] = (currentReadPacket->data[3] & 0b00000010) != 0;
        rawData[currentReadPacket->commandAddress][26] = (currentReadPacket->data[3] & 0b00000100) != 0;
      }
    }
#ifdef DEBUG
    Serial.print((currentReadPacket->commandAddress & 0x0F) << 4);
    Serial.print("|");
    Serial.print(currentReadPacket->commandAddress &0xF0);  
    Serial.print("|");
    for(int j = 0; j < 27; ++j)
      Serial.print(rawData[currentReadPacket->commandAddress][j] == 0 ? "0" : "1");
    Serial.print("\n");
#else
    if (currentReadPacket->commandAddress == CURRENT_SPY_ADDRESS)
    {
      Serial.write((currentReadPacket->commandAddress & 0x0F) << 4);
      Serial.write(currentReadPacket->commandAddress &0xF0);  
      for(int j = 0; j < 27; ++j)
        Serial.write(rawData[currentReadPacket->commandAddress][j] == 0 ? 0 : 1);
      Serial.write(currentReadPacket->HasData == 1);
      Serial.write("\n");
    }
#endif
#endif      
    delete currentReadPacket;
  }
}

void adbStateChanged() 
{
  diff = TCNT1 >> 1;
        
  if (state == WAITING_FOR_ATTENTION) {
    if (diff < 850 && diff > 750) {
      if (currentWritePacket == NULL)
         currentWritePacket = new packet();
      state = WAITING_FOR_SYNC;
    }
  } else if (state == WAITING_FOR_SYNC) {
    //if (diff < 75 && diff > 55) { // The Sync pulse in practice appears to be shorter than expected
      currentWritePacket->syncTiming = diff;
      state = READING_COMMAND_BITS;
      count = 0;
      command = 0;
    //} else {
    //  state = WAITING_FOR_ATTENTION;
    //}
  } else if (state == READING_COMMAND_BITS) {
    if (count % 2 == 0 && (count / 2) < 8) {
      if (diff < 50) {
        command |= (1 << (7 - (count / 2)));
      }
    }
    count++;
    if (count >= 16) {
      currentWritePacket->commandType = ((command >> 2) & 3);
      currentWritePacket->commandAddress = (command >> 4);
      currentWritePacket->commandRegister = (command & 3);
      state = WAITING_FOR_CMD_STOP;
    }
  }
  else if (state == WAITING_FOR_CMD_STOP)
  {
    if (diff < 75 && diff > 55) {
      currentWritePacket->commandStop = true;
      state = WAITING_FOR_DATA_START;
    }
    else
      state = WAITING_FOR_ATTENTION;
  }
  else if (state == WAITING_FOR_DATA_START)
  {
    currentWritePacket->HasData = true;
    state = WAITING_DATA_START_BIT;
  }
  else if (state == WAITING_DATA_START_BIT)
  {
    if (diff > 25 && diff < 45) {
      currentWritePacket->dataStart = true;
    }
    else
      state = WAITING_FOR_ATTENTION;
      count = -1;
      currentWritePacket->data[currentWritePacket->numBytes] = 0;
      state = READING_DATA_BITS;
  }
  else if (state == READING_DATA_BITS || state == COULD_BE_DATA_STOP_BIT)
  {
    state = READING_DATA_BITS;
      
    if (count % 2 == 0 && (count / 2) < 8) 
    {
      if (diff < 50) // Is a 1, so can't be stop bit
      {
        currentWritePacket->data[currentWritePacket->numBytes] |= (1 << (7 - (count / 2)));
        state = READING_DATA_BITS;
      }
      else if (count == 0) // Is a 0 and the first bit after a full byte
      {
        state = COULD_BE_DATA_STOP_BIT;
      } 
    }
    
    count++;
    if (count >= 16) {
      currentWritePacket->numBytes++;
      currentWritePacket->count += count;
      count = 0;
      currentWritePacket->data[currentWritePacket->numBytes] = 0;
    }
  }
  TCNT1 = 0;
}
