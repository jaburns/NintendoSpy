#include "common.h"
#include "PCFX.h"

template< unsigned char latch, unsigned char data, unsigned char clock >
void read_shiftRegister_PCFX( unsigned char bits )
{
    unsigned char *rawDataPtr = rawData;

    WAIT_FALLING_EDGE( latch );

    do {
        WAIT_FALLING_EDGE( clock );
        *rawDataPtr = !PIN_READ(data);
        ++rawDataPtr;
    }
    while( --bits > 0 );    
}

inline void loop_PCFX()
{
    noInterrupts();
    read_shiftRegister_PCFX< PCFX_LATCH , PCFX_DATA , PCFX_CLOCK >( PCFX_BITCOUNT );
    interrupts();
    sendRawData( 0 , PCFX_BITCOUNT );
}

