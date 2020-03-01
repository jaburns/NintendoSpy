#include "common.h"
#include "Saturn.h"

byte ssState1 = 0;
byte ssState2 = 0;
byte ssState3 = 0;
byte ssState4 = 0;

inline void loop_SS()
{
  noInterrupts();
  read_SSData();
  interrupts();
  sendRawSSDataV2();
}

inline void read_SSData()
{
  word pincache = 0;

  while((PIND & 0b11000000) != 0b10000000){}
  asm volatile( MICROSECOND_NOPS );
  pincache |= PIND;
  if ((pincache & 0b11000000) == 0b10000000)
    ssState3 = ~pincache;
  
  pincache = 0;
  while((PIND & 0b11000000) != 0b01000000){}
  asm volatile( MICROSECOND_NOPS );
  pincache |= PIND;
  if ((pincache & 0b11000000) == 0b01000000)
    ssState2 = ~pincache;
    
  pincache = 0;
  while((PIND & 0b11000000) != 0){}
  asm volatile( MICROSECOND_NOPS );
  pincache |= PIND;
  if ((pincache & 0b11000000) == 0)
    ssState1 = ~pincache;

  pincache = 0;
  while((PIND & 0b11000000) != 0b11000000){}
  asm volatile( MICROSECOND_NOPS );
  pincache |= PIND;
  if ((pincache & 0b11000000) == 0b11000000)
    ssState4 = ~pincache;

}

inline void sendRawSSDataV2()
{
    #ifndef DEBUG
    for(int i = 0; i < 8;++i)
    {
      Serial.write(i == 6 ? ONE : ZERO);
    }
    Serial.write((ssState3 & 0b00100000) ? ZERO : ONE);
    Serial.write((ssState3 & 0b00010000) ? ZERO : ONE);
    Serial.write((ssState3 & 0b00001000) ? ZERO : ONE);
    Serial.write((ssState3 & 0b00000100) ? ZERO : ONE);

    Serial.write((ssState2 & 0b00100000) ? ZERO : ONE);
    Serial.write((ssState2 & 0b00010000) ? ZERO : ONE);
    Serial.write((ssState2 & 0b00001000) ? ZERO : ONE);
    Serial.write((ssState2 & 0b00000100) ? ZERO : ONE);
    
    Serial.write ((ssState1 & 0b00100000) ? ZERO : ONE );
    Serial.write ((ssState1 & 0b00010000) ? ZERO : ONE );
    Serial.write ((ssState1 & 0b00001000) ? ZERO : ONE );
    Serial.write ((ssState1 & 0b00000100)  ? ZERO : ONE );

    Serial.write ((ssState4 & 0b00100000) ? ZERO : ONE );
    Serial.write (ONE);
    Serial.write (ONE);
    Serial.write (ONE);

    for(int i = 0; i < 32;++i)
      Serial.write(ZERO);

    Serial.write( SPLIT );
    #else 
    Serial.print((ssState1 & 0b00000100)    ? "Z" : "0");
    Serial.print((ssState1 & 0b00001000)    ? "Y" : "0");
    Serial.print((ssState1 & 0b00010000)    ? "X" : "0");
    Serial.print((ssState1 & 0b00100000)    ? "R" : "0");

    Serial.print((ssState2 & 0b00000100)    ? "B" : "0");
    Serial.print((ssState2 & 0b00001000)    ? "C" : "0");
    Serial.print((ssState2 & 0b00010000)    ? "A" : "0");
    Serial.print((ssState2 & 0b00100000)    ? "S" : "0");

    Serial.print((ssState3 & 0b00000100)    ? "u" : "0");
    Serial.print((ssState3 & 0b00001000)    ? "d" : "0");
    Serial.print((ssState3 & 0b00010000)    ? "l" : "0");
    Serial.print((ssState3 & 0b00100000)    ? "r" : "0");
    
    Serial.print((ssState4 & 0b00100000)    ? "L" : "0");
 
    Serial.print("\n");
    #endif
}

inline void loop_SS3D()
{
  noInterrupts();
  read_SS3DData();
 interrupts();
  sendRawSS3DData();
}

inline void read_SS3DData()
{
  byte numBits = 0;
  
  WAIT_FALLING_EDGE(SS_SEL);

  for(int i = 0; i < 3; ++i)
  {
      WAIT_FALLING_EDGE(SS_REQ);
    
      WAIT_FALLING_EDGEB(SS_ACK);
  
      rawData[numBits++] = PIN_READ(SS_DATA3);
      rawData[numBits++] = PIN_READ(SS_DATA2);
      rawData[numBits++] = PIN_READ(SS_DATA1);    
      rawData[numBits++] = PIN_READ(SS_DATA0);
  
      WAIT_LEADING_EDGE(SS_REQ);
    
      WAIT_LEADING_EDGEB(SS_ACK);
  
      rawData[numBits++] = PIN_READ(SS_DATA3);
      rawData[numBits++] = PIN_READ(SS_DATA2);
      rawData[numBits++] = PIN_READ(SS_DATA1);    
      rawData[numBits++] = PIN_READ(SS_DATA0);
  }
  int numBytes = 0;
  if (rawData[2] != 0 && rawData[3] != 0)
    numBytes = 1;
  else if (rawData[3] != 0)
    numBytes = 4;

  if (rawData[3] != 0)
  {
    for(int i = 0; i < numBytes; ++i)
    {
      WAIT_FALLING_EDGE(SS_REQ);
    
      WAIT_FALLING_EDGEB(SS_ACK);

      rawData[numBits++] = PIN_READ(SS_DATA3);
      rawData[numBits++] = PIN_READ(SS_DATA2);
      rawData[numBits++] = PIN_READ(SS_DATA1);    
            rawData[numBits++] = PIN_READ(SS_DATA0);
  
      WAIT_LEADING_EDGE(SS_REQ);
      
      WAIT_LEADING_EDGEB(SS_ACK);
    
      rawData[numBits++] = PIN_READ(SS_DATA3);
      rawData[numBits++] = PIN_READ(SS_DATA2);
      rawData[numBits++] = PIN_READ(SS_DATA1);    
      rawData[numBits++] = PIN_READ(SS_DATA0);
    }
  }
  else
  {
    rawData[numBits++] = 1;
    for(int i = 0; i < 7; ++i)
      rawData[numBits++] = 0;

    rawData[numBits++] = 1;
    for(int i = 0; i < 7; ++i)
      rawData[numBits++] = 0;

    for(int i = 0; i < 16;++i)
      rawData[numBits++] = 0;
  }
}

inline void sendRawSS3DData()
{
    #ifndef DEBUG

    for (unsigned char i = 0; i < 56; ++i)
    {
      Serial.write( rawData[i] ? ONE : ZERO );
    }
    Serial.write( SPLIT );
    #else
    for(int i = 0; i < 56; ++i)
    {
      if (i % 8 == 0)
        Serial.print("|");
      Serial.print(rawData[i] ? "1" : "0");
    }
    Serial.print("\n");
    #endif
}

