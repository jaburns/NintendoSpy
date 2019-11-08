#include "TimerOne.h"// https://code.google.com/archive/p/arduino-timerone/downloads
#define NUM_DIFFS 128

#define ADB_PIN 2

#define WAITING_FOR_ATTENTION 0
#define WAITING_FOR_SYNC 1
#define READING_COMMAND_BITS 2
#define WAITING_FOR_CMD_STOP 3
#define WAITING_FOR_DATA_START 4
#define WAITING_DATA_START_BIT 5
#define WAITING_FOR_DATA_BITS 6
#define WAITING_FOR_DATA_STOP   7
#define WAITING_FOR_DATA_STOP_TIMEOUT 8

volatile unsigned long diff;
volatile unsigned long diffs[NUM_DIFFS];
volatile unsigned int count = 0, state = WAITING_FOR_ATTENTION;
volatile unsigned char command;

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

struct packet
{
  byte commandType = 0;
  byte commandAddress = 0;
  byte commandRegister = 0;
  bool commandStop = false;
  bool HasData = false;
  bool dataStart = false;
};

#define BUFFER_SIZE 45
static volatile packet* buffer[BUFFER_SIZE];
static volatile uint8_t head, tail;
static volatile packet* currentWritePacket = NULL;
static volatile packet* currentReadPacket = NULL;

void loop() {

  if (state == WAITING_FOR_DATA_START)
  {
    unsigned long _diff = TCNT1 >> 1;
    if (_diff > 300)
    {
      currentWritePacket->HasData = false;
      packet* donePacket = currentWritePacket;
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
    Serial.print(currentReadPacket->commandType, HEX);
    Serial.print(" ");
    Serial.print(currentReadPacket->commandAddress, HEX);
    Serial.print(" ");
    Serial.print(currentReadPacket->commandRegister, HEX);
    Serial.print(" ");
    Serial.print(currentReadPacket->commandStop ? "S" : "N");
    Serial.print(" ");
    Serial.print(currentReadPacket->HasData ? "D" : "N");
    Serial.print(" ");
    Serial.println(currentReadPacket->dataStart ? "S" : "N");
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
    if (diff < 75 && diff > 55) {
      state = READING_COMMAND_BITS;
      count = 0;
      command = 0;
    } else {
      state = WAITING_FOR_ATTENTION;
    }
  } else if (state == READING_COMMAND_BITS) {
    diffs[count] = diff;
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
      state = WAITING_FOR_DATA_STOP;
    }
    uint8_t i = head + 1;
    if (i >= BUFFER_SIZE) i = 0;
    if (i != tail) {
      buffer[i] = currentWritePacket;
      head = i;
      currentWritePacket = NULL;
    }
    state = WAITING_FOR_ATTENTION;
  }
  
  TCNT1 = 0;
}
