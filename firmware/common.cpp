#include "Arduino.h"
#include "common.h"

// Declare some space to store the bits we read from a controller.
unsigned char rawData[1024];

void common_pin_setup()
{
  PORTD = 0x00;
  PORTB = 0x00;
  DDRD  = 0x00;

  for(int i = 2; i <= 6; ++i)
    pinMode(i, INPUT_PULLUP);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Performs a read cycle from a shift register based controller (SNES + NES) using only the data and latch
// wires, and waiting a fixed time between reads.  This read method is deprecated due to being finicky,
// but still exists here to support older builds.
//     latch = Pin index on Port D where the latch wire is attached.
//     data  = Pin index on Port D where the output data wire is attached.
//     bits  = Number of bits to read from the controller.
//  longWait = The NES takes a bit longer between reads to get valid results back.
template< unsigned char latch, unsigned char data, unsigned char longWait >
void read_shiftRegister_2wire( unsigned char bits )
{
    unsigned char *rawDataPtr = rawData;

    WAIT_FALLING_EDGE( latch );

read_loop:

    // Read the data from the line and store in "rawData"
    *rawDataPtr = !PIN_READ(data);
    ++rawDataPtr;
    if( --bits == 0 ) return;

    // Wait until the next button value is on the data line. ~12us between each.
    asm volatile(
        MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS
        MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS
        MICROSECOND_NOPS MICROSECOND_NOPS
    );
    if( longWait ) {
        asm volatile(
            MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS
            MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS
            MICROSECOND_NOPS MICROSECOND_NOPS
        );
    }

    goto read_loop;
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Sends a packet of controller data over the Arduino serial interface.
inline void sendRawData( unsigned char first, unsigned char count )
{
    #ifndef DEBUG
    for( unsigned char i = first ; i < first + count ; i++ ) {
        Serial.write( rawData[i] ? ONE : ZERO );
    }
    Serial.write( SPLIT );
    #else

    for( unsigned char i = 0 ; i < first; i++ ) {
        Serial.print( rawData[i] ? "1" : "0" );
    }
    Serial.print("|");
    int j = 0;
    for( unsigned char i = first ; i < first + count ; i++ ) {

        if (j % 8 == 0 && j != 0)
          Serial.print("|");
        Serial.print( rawData[i] ? "1" : "0" );
        ++j;
    }
    Serial.print("\n");
    #endif
}
