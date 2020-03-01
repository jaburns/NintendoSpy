#include "common.h"

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Performs a read cycle from a shift register based controller (SNES + NES) using only the data and latch
// wires, and waiting a fixed time between reads.  This read method is deprecated due to being finicky,
// but still exists here to support older builds.
//     latch = Pin index on Port D where the latch wire is attached.
//     data  = Pin index on Port D where the output data wire is attached.
//     bits  = Number of bits to read from the controller.
//  longWait = The NES takes a bit longer between reads to get valid results back.
void read_shiftRegister_2wire(unsigned char rawData[], unsigned char latch, unsigned char data, unsigned char longWait, unsigned char bits)
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
void sendRawData(unsigned char rawControllerData[], unsigned char first, unsigned char count)
{
    for( unsigned char i = first ; i < first + count ; i++ ) {
        Serial.write( rawControllerData[i] ? ONE : ZERO );
    }
    Serial.write( SPLIT );
}

void sendRawDataDebug(unsigned char rawControllerData[], unsigned char first, unsigned char count)
{
    for( unsigned char i = 0 ; i < first; i++ ) {
        Serial.print( rawControllerData[i] ? "1" : "0" );
    }
    Serial.print("|");
    int j = 0;
    for( unsigned char i = first ; i < first + count ; i++ ) {

        if (j % 8 == 0 && j != 0)
          Serial.print("|");
        Serial.print( rawControllerData[i] ? "1" : "0" );
        ++j;
    }
    Serial.print("\n");
}
