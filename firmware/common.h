#include "Arduino.h"

#include "config.h"

#define N64_BITCOUNT  32
#define SNES_BITCOUNT       16
#define SNES_BITCOUNT_EXT   32
#define NES_BITCOUNT         8
#define GC_BITCOUNT  64
#define ThreeDO_BITCOUNT  32
#define PCFX_BITCOUNT     16

#define PIN_READ( pin )  (PIND&(1<<(pin)))
#define PINC_READ( pin ) (PINC&(1<<(pin)))
#define PINB_READ( pin ) (PINB&(1<<(pin)))
#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );
#define WAIT_LEADING_EDGE( pin ) while( PIN_READ(pin) ); while( !PIN_READ(pin) );

#define WAIT_FALLING_EDGEB( pin ) while( !PINB_READ(pin) ); while( PINB_READ(pin) );
#define WAIT_LEADING_EDGEB( pin ) while( PINB_READ(pin) ); while( !PINB_READ(pin) );

#define ZERO  ((uint8_t)0)  // Use a byte value of 0x00 to represent a bit with value 0.
#define ONE   '1'  // Use an ASCII one to represent a bit with value 1.  This makes Arduino debugging easier.
#define SPLIT '\n'  // Use a new-line character to split up the controller state packets.

// Declare some space to store the bits we read from a controller.
extern unsigned char rawData[];
extern word currentState;
extern word lastState;

void common_pin_setup();

void read_shiftRegister_2wire(unsigned char latch, unsigned char data, unsigned char longWait, unsigned char bits);
void sendRawData(unsigned char first, unsigned char count);
void sendRawData(unsigned char rawControllerData[], unsigned char first, unsigned char count);
