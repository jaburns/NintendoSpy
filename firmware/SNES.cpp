#include "common.h"
#include "SNES.h"

template< unsigned char latch, unsigned char data, unsigned char clock >
unsigned char read_shiftRegister_SNES()
{
    unsigned char position = 0;
    unsigned char bits = 0;

    WAIT_FALLING_EDGE( latch );

    do {
        WAIT_FALLING_EDGE( clock );
        rawData[position++] = !PIN_READ(data);
    }
    while( ++bits <= SNES_BITCOUNT );    

    if (rawData[15] != 0x0)
    {
      bits = 0;
      do {
	      WAIT_FALLING_EDGE( clock );
          rawData[position++] = !PIN_READ(data);
      }
      while( ++bits <= SNES_BITCOUNT );

      return SNES_BITCOUNT_EXT;
    }

    return SNES_BITCOUNT;
}

inline void loop_SNES()
{
    noInterrupts();
    unsigned char bytesToReturn = SNES_BITCOUNT;
#ifdef MODE_2WIRE_SNES
    read_shiftRegister_2wire< SNES_LATCH , SNES_DATA , false >( SNES_BITCOUNT );
#else
    bytesToReturn = read_shiftRegister_SNES< SNES_LATCH , SNES_DATA , SNES_CLOCK >();
#endif
    interrupts();
    sendRawData( 0 , bytesToReturn );
}
