#include "common.h"
#include "FMTowns.h"

inline void read_FMTowns()
{
  rawData[0] = PIN_READ(2);
  rawData[1] = PIN_READ(3);
  rawData[2] = PIN_READ(4);
  rawData[3] = PIN_READ(5);
  rawData[4] = PIN_READ(6);
  rawData[5] = PIN_READ(7);
  rawData[6] = PINB_READ(0);
  rawData[7] = PINB_READ(1);
  rawData[8] = PINB_READ(2);
}

inline void sendFMTowns()
{
    #ifndef DEBUG
    for (unsigned char i = 0; i < 9; ++i)
    {
      Serial.write( rawData[i] ? ZERO : ONE );
    }
    Serial.write( SPLIT );
    #else
    for(int i = 0; i < 9; ++i)
    {
      Serial.print(rawData[i] ? "0" : "1");
    }
    Serial.print("\n");
    #endif
}

inline void loop_FMTowns()
{
  noInterrupts();
  read_FMTowns();
  interrupts();
  sendFMTowns();
}

