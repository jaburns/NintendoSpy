#include "common.h"
#include "3DO.h"

template< unsigned char latch, unsigned char data, unsigned char clock >
byte read_3do( )
{
    unsigned char *rawDataPtr = rawData;

    byte numBitsToRead = 0;
    byte bits = 0;
    WAIT_FALLING_EDGE( latch );

    do {
        WAIT_LEADING_EDGE( clock );
        *rawDataPtr = PIN_READ(data);
        
        if(bits == 0 && *rawDataPtr != 0)
          numBitsToRead = bits = 32;
        else if (bits == 0)
         numBitsToRead = bits = 16;
        
        ++rawDataPtr;
    }
    while( --bits > 0 );

    return numBitsToRead;
}

inline void loop_3DO()
{
    noInterrupts();
    byte bits = read_3do< ThreeDO_LATCH , ThreeDO_DATA , ThreeDO_CLOCK >( );
    interrupts();
    sendRawData( 0 , bits );
}

