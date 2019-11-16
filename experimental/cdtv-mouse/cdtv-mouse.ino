#include "TimerOne.h"// https://code.google.com/archive/p/arduino-timerone/downloads

#define BUFFER_SIZE 45

#define PRDT_PIN 2

//#define DEBUG

#define WAITING_FOR_SYNC_HIGH 1
#define WAITING_FOR_SYNC_LOW  2
#define SYNCED                3
#define WAITING_FOR_START     4

struct packet
{
  byte rawData[25];
};

volatile packet* buffer[BUFFER_SIZE];
uint8_t head, tail;
volatile packet* currentReadPacket = NULL;

volatile packet* currentWritePacket = NULL;
volatile unsigned long diff;
volatile unsigned int count = 0, state = WAITING_FOR_SYNC_HIGH;

void setup() {
  pinMode(PRDT_PIN, INPUT_PULLUP);
  attachInterrupt(digitalPinToInterrupt(PRDT_PIN), PRDTStateChanged, CHANGE);
  Timer1.initialize(10000);
  Timer1.stop();
  Timer1.restart();
  Timer1.detachInterrupt();
  Serial.begin(115200);
  delay(200);

}

void loop() {
  
  uint8_t i = tail;
  if (i != head)
  {
    i++;
    if (i >= BUFFER_SIZE) i = 0;
    currentReadPacket = buffer[i];
    tail = i;

#ifdef DEBUG
    for(int i = 0; i < 24; ++i)
      Serial.print(currentReadPacket->rawData[i] == 0 ? "0" : "1");

    Serial.print("|");
    byte x = 0;
    for(int i = 3; i < 11; ++i)
    {
      if (currentReadPacket->rawData[i] == 1)
        x |= (1 << (7 - (i-3)));
    }
    Serial.print(x);
     Serial.print("|");
    byte y = 0;
    for(int i = 11; i < 19; ++i)
    {
      if (currentReadPacket->rawData[i] == 1)
        y |= (1 << (7 - (i-11)));
    }
    Serial.print(y);   
    Serial.print("\n");

#else
    int checksum = 0;
    for(int i = 0; i < 24; ++i)
    {
      checksum += currentReadPacket->rawData[i] == 0 ? 0 : 1;
      Serial.write(currentReadPacket->rawData[i] == 0 ? 0 : 1);
    }
    Serial.write((checksum & 0x0F) << 4);
    Serial.write((checksum & 0xF0));
    Serial.write("\n");
#endif
    delete currentReadPacket;
  }
}

void PRDTStateChanged() 
{
  diff = TCNT1 >> 1;
        
  if (state == WAITING_FOR_SYNC_HIGH) {
    if (diff > 1000 ) {
      state = WAITING_FOR_SYNC_LOW;
    }
  } else if (state == WAITING_FOR_SYNC_LOW) {
    if (diff > 1000)
    { 
      state = SYNCED;
      if (currentWritePacket == NULL)
        currentWritePacket = new packet();
      count = -1;
    }
  } else if (state == SYNCED) {
    //Serial.println("SYNCED");
    if (count % 2 == 0) {
      if (diff > 400) {
        currentWritePacket->rawData[count/2] = 1;
      }
      else
      {
        currentWritePacket->rawData[count/2] = 0;
      }
    }
    count++;
    if ((count == 48 && currentWritePacket->rawData[0] == 0) || (count == 38 && currentWritePacket->rawData[0] == 1)) {
      volatile packet* donePacket = currentWritePacket;
      state = WAITING_FOR_START;
      count = 0;
      currentWritePacket = NULL;
      uint8_t i = head + 1;
      if (i >= BUFFER_SIZE) i = 0;
      if (i != tail) {
        buffer[i] = donePacket;
        head = i;
      }
    }
  }
  else if (state == WAITING_FOR_START)
  {
    //Serial.println("WAITING_FOR_START");
    if (count == 2)
    {
      if (currentWritePacket == NULL)
        currentWritePacket = new packet();
      count = -1;
      state = SYNCED;
    }
    else
      count++;
  }
  TCNT1 = 0;
}
