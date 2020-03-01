#include "common.h"
#include "NES.h"

template< unsigned char latch, unsigned char data, unsigned char clock, unsigned char data0, unsigned char data1 >
void read_shiftRegister_NES( unsigned char bits )
{
    unsigned char *rawDataPtr = rawData;

    WAIT_FALLING_EDGE( latch );

    do {
        WAIT_FALLING_EDGE( clock );
        *rawDataPtr = !PIN_READ(data);
        *(rawDataPtr+8) = !PIN_READ(data0);
        *(rawDataPtr+16) = !PIN_READ(data1);
        ++rawDataPtr;
    }
    while( --bits > 0 );    
}

inline void loop_NES()
{
    noInterrupts();
#ifdef MODE_2WIRE_NES
    read_shiftRegister_2wire< NES_LATCH , NES_DATA , true >( NES_BITCOUNT );
#else
    read_shiftRegister_NES< NES_LATCH , NES_DATA , NES_CLOCK, NES_DATA0, NES_DATA1>( NES_BITCOUNT );
#endif
    interrupts();
    sendRawData( 0 , NES_BITCOUNT*3 );
}

