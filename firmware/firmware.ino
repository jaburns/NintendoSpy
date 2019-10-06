/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Arduino
// v3.3
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns


// ---------- Uncomment one of these options to select operation mode --------------
//#define MODE_GC
//#define MODE_N64
//#define MODE_SNES
//#define MODE_NES
//#define MODE_SEGA  // For Genesis. Use MODE_CLASSIC for Master System
//#define MODE_GENESIS_MOUSE
//#define MODE_CLASSIC
//#define MODE_BOOSTER_GRIP
//#define MODE_PLAYSTATION
//#define MODE_TG16
//#define MODE_SATURN
//#define MODE_SATURN3D
//#define MODE_NEOGEO
//#define MODE_3DO
//#define MODE_INTELLIVISION
//#define MODE_JAGUAR
//Bridge one of the analog GND to the right analog IN to enable your selected mode
//#define MODE_DETECT
// ---------------------------------------------------------------------------------
// The only reason you'd want to use 2-wire SNES mode is if you built a NintendoSpy
// before the 3-wire firmware was implemented.  This mode is for backwards
// compatibility only. 
//#define MODE_2WIRE_SNES
// ---------------------------------------------------------------------------------
// Uncomment this for serial debugging output
//#define DEBUG

#include "SegaControllerSpy.h"
#include "ClassicControllerSpy.h"
#include "BoosterGripSpy.h"

SegaControllerSpy segaController;
word currentState = 0;
unsigned int uiCurrentState = 0;
word lastState = 0;
byte ssState1 = 0;
byte ssState2 = 0;
byte ssState3 = 0;
byte ssState4 = 0;

bool seenGC2N64 = false;

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
ClassicControllerSpy classicController(2, 3, 4, 5, 7, 8);

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
BoosterGripSpy boosterGrip(2, 3, 4, 5, 6, 7, 8);

#define PIN_READ( pin )  (PIND&(1<<(pin)))
#define PINC_READ( pin ) (PINC&(1<<(pin)))
#define PINB_READ( pin ) (PINB&(1<<(pin)))
#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );
#define WAIT_LEADING_EDGE( pin ) while( PIN_READ(pin) ); while( !PIN_READ(pin) );

#define WAIT_FALLING_EDGEB( pin ) while( !PINB_READ(pin) ); while( PINB_READ(pin) );
#define WAIT_LEADING_EDGEB( pin ) while( PINB_READ(pin) ); while( !PINB_READ(pin) );

#define MODEPIN_SNES 0
#define MODEPIN_N64  1
#define MODEPIN_GC   2

#define N64_PIN        2
#define N64_BITCOUNT  32

#define SNES_LATCH           3
#define SNES_DATA            4
#define SNES_CLOCK           6
#define SNES_BITCOUNT       16
#define SNES_BITCOUNT_EXT   32
#define NES_BITCOUNT         8

#define GC_PIN        5
#define GC_PREFIX    25
#define GC_BITCOUNT  64

#define ThreeDO_LATCH      2
#define ThreeDO_DATA       4
#define ThreeDO_CLOCK      3   
#define ThreeDO_BITCOUNT  32

#define ZERO  '\0'  // Use a byte value of 0x00 to represent a bit with value 0.
#define ONE    '1'  // Use an ASCII one to represent a bit with value 1.  This makes Arduino debugging easier.
#define SPLIT '\n'  // Use a new-line character to split up the controller state packets.

// Declare some space to store the bits we read from a controller.
unsigned char rawData[ 1024 ];

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
{
    PORTC = 0xFF; // Set the pull-ups on the port we use to check operation mode.
    DDRC  = 0x00;
  
  #ifdef MODE_SEGA
      sega_classic_pin_setup();
      goto setup1;
  #elif defined MODE_CLASSIC
      sega_classic_pin_setup();
      goto setup1;
  #elif defined MODE_DETECT
  	#ifdef MODEPIN_SEGA
      if( !PINC_READ( MODEPIN_SEGA ) ) {
          sega_classic_pin_setup();
  		    goto setup1;
      }
  	#endif 
  	#ifdef MODEPIN_CLASSIC
  	if( !PINC_READ( MODEPIN_CLASSIC ) ) {
          sega_classic_pin_setup();
  		    goto setup1;
      }
  	#endif
  #endif

  common_pin_setup();

setup1:
  
    lastState = -1;
    currentState = 0;
    seenGC2N64 = false;
  
    Serial.begin( 115200 );
}

void sega_classic_pin_setup()
{
  for(int i = 2; i <= 6; ++i)
    pinMode(i, INPUT_PULLUP);
}

void common_pin_setup()
{
  PORTD = 0x00;
  PORTB = 0x00;
  DDRD  = 0x00;

  for(int i = 2; i <= 6; ++i)
    pinMode(i, INPUT_PULLUP);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Performs a read cycle from one of Nintendo's one-wire interface based controllers.
// This includes the N64 and the Gamecube.
//     pin  = Pin index on Port D where the data wire is attached.
//     bits = Number of bits to read from the line.
template< unsigned char pin >
void read_oneWire( unsigned char bits )
{
    unsigned char *rawDataPtr = rawData;

read_loop:

    // Wait for the line to go high then low.
    WAIT_FALLING_EDGE( pin );

    // Wait ~2us between line reads
    
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );

    // Read a bit from the line and store as a byte in "rawData"
    *rawDataPtr = PIN_READ(pin);
    ++rawDataPtr;
    if( --bits == 0 ) return;

    goto read_loop;
}

void read_N64( )
{
    unsigned short bits;
    
    unsigned char *rawDataPtr = &rawData[1];
    byte /*bit7, bit6, bit5, bit4, bit3, */bit2, bit1, bit0;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit7 = PIND & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit6 = PIND & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit5 = PIND & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit4 = PIND & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit3 = PIND & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    bit2 = PIND & 0b00000100;
    if (bit2 != 0)  // Controller Reset
    {
          WAIT_FALLING_EDGE( N64_PIN );
          asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
          // bit1 = PIND & 0b00000100;
          WAIT_FALLING_EDGE( N64_PIN );
          asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
          // bit0 = PIND & 0b00000100;
          bits = 25;
          rawData[0] = 0xFF;
          goto read_loop;
    }
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    bit1 = PIND & 0b00000100;
    if (bit1 != 0) // read or write to memory pack (this doesn't work correctly)
    {
          WAIT_FALLING_EDGE( N64_PIN );
          asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
          // bit0 = PIND & 0b00000100;
          bits = 281;
          rawData[0] = 0x02;
          goto read_loop;
    }
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    bit0 = PIND & 0b00000100;
    if (bit0 != 0) // controller poll
    {
          bits = 33;
          rawData[0] = 0x01;
          goto read_loop;
    }
    bits = 25;   // Get controller info
    rawData[0] = 0x00;
           
read_loop:

    // Wait for the line to go high then low.
    WAIT_FALLING_EDGE( N64_PIN );

    // Wait ~2us between line reads
    
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );

    // Read a bit from the line and store as a byte in "rawData"
    *rawDataPtr = PIND & 0b00000100;
    ++rawDataPtr;
    if( --bits == 0 ) return;

    goto read_loop;
}

// Verifies that the 9 bits prefixing N64 controller data in 'rawData'
// are actually indicative of a controller state signal.
inline bool checkPrefixN64 ()
{
    return rawData[0] == 0x01;
}

inline bool checkPrefixGC ()
{
    if( rawData[0] != 0 ) return false; // 0
    if( rawData[1] == 0 ) return false; // 1
    if( rawData[2] != 0 ) return false; // 0
    if( rawData[3] != 0 ) return false; // 0
    if( rawData[4] != 0 ) return false; // 0
    if( rawData[5] != 0 ) return false; // 0
    if( rawData[6] != 0 ) return false; // 0
    if( rawData[7] != 0 ) return false; // 0
    if( rawData[8] != 0 ) return false; // 0
    if( rawData[9] != 0 ) return false; // 0
    if( rawData[10] != 0 ) return false; // 0
    if( rawData[11] != 0 ) return false; // 0
    if( rawData[12] != 0 ) return false; // 0
    if( rawData[13] != 0 ) return false; // 0
    if( rawData[14] == 0 ) return false; // 1
    if( rawData[15] == 0 ) return false; // 1
    if( rawData[16] != 0 ) return false; // 0
    if( rawData[17] != 0 ) return false; // 0
    if( rawData[18] != 0 ) return false; // 0
    if( rawData[19] != 0 ) return false; // 0
    if( rawData[20] != 0 ) return false; // 0
    if( rawData[21] != 0 ) return false; // 0
    //if( rawData[22] != 0 ) return false; // 0 or 1
    if( rawData[23] != 0 ) return false; // 0
    if( rawData[24] == 0 ) return false; // 1
    seenGC2N64 = false;
    return true;
}

inline bool checkBothGCPrefixOnRaphnet()
{
    if( rawData[0] != 0 ) return false; // 0
    if( rawData[1] != 0 ) return false; // 0
    if( rawData[2] != 0 ) return false; // 0
    if( rawData[3] != 0 ) return false; // 0
    if( rawData[4] != 0 ) return false; // 0
    if( rawData[5] != 0 ) return false; // 0
    if( rawData[6] != 0 ) return false; // 0
    if( rawData[7] != 0 ) return false; // 0
    if( rawData[8] == 0 ) return false; // 1
    if( rawData[9] != 0 ) return false; // 0
    if( rawData[10] != 0 ) return false; // 0
    if( rawData[11] != 0 ) return false; // 0
    if( rawData[12] != 0 ) return false; // 0
    if( rawData[13] == 0 ) return false; // 1
    if( rawData[14] != 0 ) return false; // 0
    if( rawData[15] != 0 ) return false; // 0
    if( rawData[16] == 0 ) return false; // 1
    if( rawData[17] != 0 ) return false; // 0
    if( rawData[18] != 0 ) return false; // 0
    if( rawData[19] != 0 ) return false; // 0
    if( rawData[20] != 0 ) return false; // 0
    if( rawData[21] != 0 ) return false; // 0
    if( rawData[22] != 0 ) return false; // 0
    if( rawData[23] != 0 ) return false; // 0
    if( rawData[24] != 0 ) return false; // 0
    if( rawData[25] != 0 ) return false; // 0 
    //if( rawData[26] != 0 ) return false; // 0 or 1
    if( rawData[27] == 0 ) return false; // 1
    if( rawData[28] != 0 ) return false; // 0
    if( rawData[29] != 0 ) return false; // 0
    if( rawData[30] != 0 ) return false; // 0
    if( rawData[31] == 0 ) return false; // 1
    if( rawData[32] == 0 ) return false; // 1
    if( rawData[33] != 0 ) return false; // 0
    if( rawData[34] != 0 ) return false; // 0
    if( rawData[35] == 0 ) return false; // 1
    if( rawData[36] != 0 ) return false; // 0
    if( rawData[37] != 0 ) return false; // 0
    if( rawData[38] != 0 ) return false; // 0
    if( rawData[39] != 0 ) return false; // 0
    if( rawData[40] != 0 ) return false; // 0
    if( rawData[41] != 0 ) return false; // 0
    if( rawData[42] != 0 ) return false; // 0
    if( rawData[43] != 0 ) return false; // 0
    if( rawData[44] != 0 ) return false; // 0
    if( rawData[45] != 0 ) return false; // 0
    if( rawData[46] != 0 ) return false; // 0
    if( rawData[47] != 0 ) return false; // 0
    if( rawData[48] == 0 ) return false; // 1
    if( rawData[49] == 0 ) return false; // 1
    if( rawData[50] != 0 ) return false; // 0
    if( rawData[51] != 0 ) return false; // 0
    if( rawData[52] != 0 ) return false; // 0
    if( rawData[53] != 0 ) return false; // 0
    if( rawData[54] != 0 ) return false; // 0
    if( rawData[55] != 0 ) return false; // 0
    if( rawData[56] != 0 ) return false; // 0 
    if( rawData[57] != 0 ) return false; // 0
    if( rawData[58] == 0 ) return false; // 1
    seenGC2N64 = true;
    return true;
}


inline bool checkPrefixGBA ()
{
    if( rawData[0] != 0 ) return false; // 0
    if( rawData[1] != 0 ) return false; // 0
    if( rawData[2] != 0 ) return false; // 0
    if( rawData[3] == 0 ) return false; // 1
    if( rawData[4] != 0 ) return false; // 0
    if( rawData[5] == 0 ) return false; // 1
    if( rawData[6] != 0 ) return false; // 0
    if( rawData[7] != 0 ) return false; // 0
    if( rawData[8] == 0 ) return false; // 1
    if( rawData[9] != 0 ) return false; // 0
    if( rawData[10] != 0 ) return false; // 0
    if( rawData[11] == 0 ) return false; // 1
    seenGC2N64 = false;
    return true;
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
// Preferred method for reading SNES + NES controller data.
template< unsigned char latch, unsigned char data, unsigned char clock >
void read_shiftRegister( unsigned char bits )
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

template< unsigned char latch, unsigned char data, unsigned char clock >
void read_shiftRegister_reverse_clock( unsigned char bits )
{
    unsigned char *rawDataPtr = rawData;

    WAIT_FALLING_EDGE( latch );

    do {
        WAIT_LEADING_EDGE( clock );
        *rawDataPtr = PIN_READ(data);
        ++rawDataPtr;
    }
    while( --bits > 0 );
}

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

inline void sendN64Data( unsigned char first, unsigned char count )
{
    #ifndef DEBUG
    for( unsigned char i = first ; i < first + count ; i++ ) {
        Serial.write( rawData[i] ? ONE : ZERO );
    }
    Serial.write( SPLIT );
    #else
    Serial.print(rawData[0]);
    Serial.print("|");
    int j = 0;
    for( unsigned char i = 0 ; i < 32; i++ ) {
        if (j % 8 == 0 && j != 0)
          Serial.print("|");
        Serial.print( rawData[i+2] ? "1" : "0" );
        j++;
    }
    Serial.print("\n");
    #endif
}

inline void sendRawGBAData()
{
  #ifndef DEBUG
  Serial.write(ZERO );
  Serial.write(ZERO );
  Serial.write(ZERO );
  Serial.write(rawData[21] ? ONE : ZERO);
  Serial.write(ZERO );
  Serial.write(ZERO );
  Serial.write(rawData[23] ? ONE : ZERO);
  Serial.write(rawData[24] ? ONE : ZERO);
  Serial.write(ZERO );
  Serial.write(rawData[31] ? ONE : ZERO);
  Serial.write(rawData[32] ? ONE : ZERO);
  Serial.write(rawData[22] ? ONE : ZERO);
  Serial.write(rawData[18] ? ONE : ZERO);
  Serial.write(rawData[17] ? ONE : ZERO);
  Serial.write(rawData[20] ? ONE : ZERO);
  Serial.write(rawData[19] ? ONE : ZERO);
  Serial.write(ONE);
  for(int i = 0; i < 7; ++i)
    Serial.write(ZERO );
  Serial.write(ONE);
  for(int i = 0; i < 7; ++i)
    Serial.write(ZERO );
  Serial.write(ONE);
  for(int i = 0; i < 7; ++i)
    Serial.write(ZERO );
  Serial.write(ONE);
  for(int i = 0; i < 7; ++i)
    Serial.write(ZERO );
  for(int i = 0; i < 8; ++i)
    Serial.write(ZERO );
  for(int i = 0; i < 8; ++i)
    Serial.write(ZERO );
  Serial.write( SPLIT );
  #else
  Serial.print("0");
  Serial.print("0");
  Serial.print("0");
  Serial.print(rawData[21] ? "t" : "0");
  Serial.print("0" );
  Serial.print("0" );
  Serial.print(rawData[23] ? "b" : "0");
  Serial.print(rawData[24] ? "a" : "0");
  Serial.print("0" );
  Serial.print(rawData[31] ? "L": "0");
  Serial.print(rawData[32] ? "R": "0");
  Serial.print(rawData[22] ? "s" : "0");
  Serial.print(rawData[18] ? "u" : "0");
  Serial.print(rawData[17] ? "d" : "0");
  Serial.print(rawData[20] ? "l" : "0");
  Serial.print(rawData[19] ? "r" : "0");
  Serial.print(128);
  Serial.print(128);
  Serial.print(128);
  Serial.print(128);
  Serial.print(0);
  Serial.print(0);
  Serial.print( "\n" );
  #endif
}

#define SS_SELECT0 6
#define SS_SEL 6
#define SS_SELECT1 7
#define SS_REQ 7
#define SS_ACK 0 // 8 - 0 on PORTB 
#define SS_DATA0   2
#define SS_DATA1   3
#define SS_DATA2   4
#define SS_DATA3   5

inline void read_SSData()
{
  word pincache = 0;

  while((PIND & 0b11000000) != 0b10000000){}
  pincache |= PIND;
  if ((pincache & 0b11000000) == 0b10000000)
    ssState3 = ~pincache;

  pincache = 0;
  while((PIND & 0b11000000) != 0b01000000){}
  pincache |= PIND;
  if ((pincache & 0b11000000) == 0b01000000)
    ssState2 = ~pincache;
    
  pincache = 0;
  while((PIND & 0b11000000) != 0){}
  pincache |= PIND;
  if ((pincache & 0b11000000) == 0)
    ssState1 = ~pincache;

  pincache = 0;
  while((PIND & 0b11000000) != 0b11000000){}
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

#define TG_SELECT 6
#define TG_DATA1  2
#define TG_DATA2  3
#define TG_DATA3  4
#define TG_DATA4  5

inline void read_TgData()
{
  currentState = 0x0000;
  while((PIND & 0b01000000) == 0){}    
  asm volatile(
        "nop\nnop\n");
    currentState |= ((PIND & 0b00111100) >> 2);

  while((PIND & 0b01000000) != 0){}    
    asm volatile(
        "nop\nnop\n");
    currentState |= ((PIND & 0b00111100) << 2);

    currentState = ~currentState;
}

#define PS_ATT 2
#define PS_CLOCK 3
#define PS_ACK 4
#define PS_CMD 5
#define PS_DATA 6

byte playstationCommand[8];

inline void read_Playstation( )
{
  byte numBits = 0;
  WAIT_FALLING_EDGE(PS_ATT);

  unsigned char bits = 8;
  do {
     WAIT_LEADING_EDGE(PS_CLOCK);
  }
  while( --bits > 0 );

  bits = 0;
  do {
      WAIT_LEADING_EDGE(PS_CLOCK);
      byte pins = PIND;
      
      rawData[numBits] = pins & 0b01000000;
      playstationCommand[numBits++] = pins & 0b00100000;
  }
  while( ++bits < 8 );
  
  bits = 0;
  do {
      WAIT_LEADING_EDGE(PS_CLOCK);
  }
  while( ++bits < 8 );
  
  bits = 0;
  do {
      WAIT_LEADING_EDGE(PS_CLOCK);
      rawData[numBits++] = !PIN_READ(PS_DATA);
  }
  while( ++bits < 16 );

  //Read analog sticks for Analog Controller in Red Mode
  if (rawData[0] != 0 && rawData[1] != 0 && rawData[2] == 0 && rawData[3] == 0 && rawData[4] != 0 && rawData[5] != 0  && rawData[6] != 0 && rawData[7] == 0 /*controllerType == 0x73 (DualShock 1)*/)
  {
    for(int i = 0; i < 4; ++i)
    {
      bits = 0;
      do {
          WAIT_LEADING_EDGE(PS_CLOCK);
          rawData[numBits++] = PIN_READ(PS_DATA);
      }
      while( ++bits < 8 );
    }
  }  
  else if (rawData[0] == 0 && rawData[1] != 0 && rawData[2] == 0 && rawData[3] == 0 && rawData[4] != 0 && rawData[5] == 0  && rawData[6] == 0 && rawData[7] == 0 /*controllerType == 0x12 (mouse)*/)
  {
    for(int i = 0; i < 2; ++i)
    {
      bits = 0;
      do {
          WAIT_LEADING_EDGE(PS_CLOCK);
          rawData[numBits++] = PIN_READ(PS_DATA);
      }
      while( ++bits < 8 );
    }  
  }
  else if (rawData[0] != 0 && rawData[1] == 0 && rawData[2] == 0 && rawData[3] != 0 && rawData[4] != 0 && rawData[5] != 0  && rawData[6] != 0 && rawData[7] == 0 /*controllerType == 0x79 (DualShock 2)*/)
  {
    for(int i = 0; i < 16; ++i)
    {
      bits = 0;
      do {
          WAIT_LEADING_EDGE(PS_CLOCK);
          rawData[numBits++] = PIN_READ(PS_DATA);
      }
      while( ++bits < 8 );
    }    
  }
  else
  {
    
  }
}

inline void sendRawPs2Data()
{
    #ifndef DEBUG
    if (playstationCommand[0] == 0 && playstationCommand[1] != 0 && playstationCommand[2] == 0 && playstationCommand[3] == 0 && playstationCommand[4] == 0 && playstationCommand[5] == 0  && playstationCommand[6] != 0 && playstationCommand[7] == 0 /*playstationCommand=0x42 (Controller Poll)*/)
    {
      for (unsigned char i = 0; i < 152; ++i)
      {
        Serial.write( rawData[i] ? ONE : ZERO );
      }
      Serial.write( SPLIT );
    }
    #else
    for(int i = 0; i < 152; ++i)
    {
      if (i % 8 == 0)
        Serial.print("|");
      Serial.print(rawData[i] ? "1" : "0");
    }
    Serial.print("\n");
    #endif
}

inline void sendRawTgData()
{
    #ifndef DEBUG
    for (unsigned char i = 0; i < 8; ++i)
    {
      Serial.write (currentState & (1 << i) ? ONE : ZERO );
    }
    Serial.write( SPLIT );
    #else 
    Serial.print((currentState & 0b0000000000000001)    ? "U" : "0");
    Serial.print((currentState & 0b0000000000000010)    ? "R" : "0");
    Serial.print((currentState & 0b0000000000000100)    ? "D" : "0");
    Serial.print((currentState & 0b0000000000001000)    ? "L" : "0");
    Serial.print((currentState & 0b0000000000010000)    ? "A" : "0");
    Serial.print((currentState & 0b0000000000100000)    ? "B" : "0");
    Serial.print((currentState & 0b0000000001000000)    ? "S" : "0");
    Serial.print((currentState & 0b0000000010000000)    ? "R" : "0");
    Serial.print("\n");
    #endif
}

inline void sendRawSegaData()
{
  #ifndef DEBUG
  for (unsigned char i = 0; i < 13; ++i)
  {
    Serial.write (currentState & (1 << i) ? ONE : ZERO );
  }
  Serial.write( SPLIT );
  #else
  #ifdef MODE_SEGA
  if (currentState != lastState)
  {
      Serial.print((currentState & SCS_CTL_ON)    ? "+" : "-");
      Serial.print((currentState & SCS_BTN_UP)    ? "U" : "0");
      Serial.print((currentState & SCS_BTN_DOWN)  ? "D" : "0");
      Serial.print((currentState & SCS_BTN_LEFT)  ? "L" : "0");
      Serial.print((currentState & SCS_BTN_RIGHT) ? "R" : "0");
      Serial.print((currentState & SCS_BTN_START) ? "S" : "0");
      Serial.print((currentState & SCS_BTN_A)     ? "A" : "0");
      Serial.print((currentState & SCS_BTN_B)     ? "B" : "0");
      Serial.print((currentState & SCS_BTN_C)     ? "C" : "0");
      Serial.print((currentState & SCS_BTN_X)     ? "X" : "0");
      Serial.print((currentState & SCS_BTN_Y)     ? "Y" : "0");
      Serial.print((currentState & SCS_BTN_Z)     ? "Z" : "0");
      Serial.print((currentState & SCS_BTN_MODE)  ? "M" : "0");
      Serial.print("\n");
      lastState = currentState;
  }
  #endif
  #ifdef MODE_CLASSIC
  if (currentState != lastState)
  {
      Serial.print((currentState & CC_BTN_UP)    ? "U" : "0");
      Serial.print((currentState & CC_BTN_DOWN)  ? "D" : "0");
      Serial.print((currentState & CC_BTN_LEFT)  ? "L" : "0");
      Serial.print((currentState & CC_BTN_RIGHT) ? "R" : "0");
      Serial.print((currentState & CC_BTN_1)     ? "1" : "0");
      Serial.print((currentState & CC_BTN_2)     ? "2" : "0");
      Serial.print("\n");
      lastState = currentState;
  } 
  #endif
  #ifdef MODE_BOOSTER_GRIP
  if (currentState != lastState)
  {
      Serial.print((currentState & BG_BTN_UP)    ? "U" : "0");
      Serial.print((currentState & BG_BTN_DOWN)  ? "D" : "0");
      Serial.print((currentState & BG_BTN_LEFT)  ? "L" : "0");
      Serial.print((currentState & BG_BTN_RIGHT) ? "R" : "0");
      Serial.print((currentState & BG_BTN_1)     ? "1" : "0");
      Serial.print((currentState & BG_BTN_2)     ? "2" : "0");
      Serial.print((currentState & BG_BTN_3)     ? "3" : "0");
      Serial.print("\n");
      lastState = currentState;
  } 
  #endif
  #endif
}

inline void sendRawSegaMouseData()
{
  #ifndef DEBUG
  for(int i = 0; i < 3; ++i)
    for(int j = 0; j < 8; ++j)
      Serial.write((rawData[i] & (1 << j)) == 0 ? ZERO : ONE);
  #else
    for(int i = 0; i < 3; ++i)
      for(int j = 0; j < 8; ++j)
        Serial.print((rawData[i] & (1 << j)) == 0 ? "0" : "1");
  #endif   
  Serial.print("\n");
}

// PORTD
#define NEOGEO_SELECT 2
#define NEOGEO_D 3
#define NEOGEO_B 4
#define NEOGEO_RIGHT 5
#define NEOGEO_DOWN 6
#define NEOGEO_START 7
//PORTB
#define NEOGEO_C 0
#define NEOGEO_A 1
#define NEOGEO_LEFT 2
#define NEOGEO_UP 3

inline void read_NeoGeo()
{
  rawData[0] = PIN_READ(NEOGEO_SELECT);
  rawData[1] = PIN_READ(NEOGEO_D);
  rawData[2] = PIN_READ(NEOGEO_B);
  rawData[3] = PIN_READ(NEOGEO_RIGHT);
  rawData[4] = PIN_READ(NEOGEO_DOWN);
  rawData[5] = PIN_READ(NEOGEO_START);
  rawData[6] = PINB_READ(NEOGEO_C);
  rawData[7] = PINB_READ(NEOGEO_A);
  rawData[8] = PINB_READ(NEOGEO_LEFT);
  rawData[9] = PINB_READ(NEOGEO_UP);
}

inline void sendNeoGeoData()
{
    #ifndef DEBUG
    for (unsigned char i = 0; i < 10; ++i)
    {
      Serial.write( rawData[i] ? ZERO : ONE );
    }
    Serial.write( SPLIT );
    #else
    for(int i = 0; i < 10; ++i)
    {
      Serial.print(rawData[i] ? "0" : "1");
    }
    Serial.print("\n");
    #endif
}

//PORT D PINS
#define INTPIN1 2  // Digital Pin 2
#define INTPIN2 3  // Digital Pin 3
#define INTPIN3 4  // Digital Pin 4
#define INTPIN4 5  // Digital Pin 5
#define INTPIN5 NA // Not Connected
#define INTPIN6 7  // Digital Pin 7
// PORT B PINS
#define INTPIN7 2  // Digital Pin 10
#define INTPIN8 3  // Digital Pin 11
#define INTPIN9 0  // Digital Pin 8

// The raw output of an Intellivision controller when
// multiple buttons is pressed is very strange and 
// no likely what you want to see, but set this to false
// if you do want the strange behavior
#define INT_SANE_BEHAVIOR true

byte intRawData;

void read_IntellivisionData()
{
    intRawData = 0x00;

    intRawData |= PIN_READ(INTPIN1) == 0 ? 0x080 : 0x00;
    intRawData |= PIN_READ(INTPIN2) == 0 ? 0x040 : 0x00;
    intRawData |= PIN_READ(INTPIN3) == 0 ? 0x020 : 0x00;
    intRawData |= PIN_READ(INTPIN4) == 0 ? 0x010 : 0x00;
    intRawData |= PIN_READ(INTPIN6) == 0 ? 0x008 : 0x00;
    intRawData |= PINB_READ(INTPIN7) == 0 ? 0x004 : 0x00;
    intRawData |= PINB_READ(INTPIN8) == 0 ? 0x002 : 0x00;
    intRawData |= PINB_READ(INTPIN9) == 0 ? 0x001 : 0x00;
}

void writeIntellivisionDataToSerial()
{
    #ifndef DEBUG
    for (unsigned char i = 0; i < 32; ++i)
    {
      Serial.write( rawData[i] );
    }
    Serial.write( SPLIT );
    #else
    Serial.print(intRawData);
    Serial.print("|");
    for(int i = 0; i < 16; ++i)
    {
      Serial.print(rawData[i]);
    }
    Serial.print("|");
    for(int i = 0; i < 12; ++i)
    {
      Serial.print(rawData[i+16]);
    }
    Serial.print("|");   
    for(int i = 0; i < 4; ++i)
    {
      Serial.print(rawData[i+28]);
    }
    Serial.print("\n");
    #endif
}

void sendIntellivisionData_Sane()
{
  // This is an interpretted display method that tries to emulate what most
  // games would actually do.

  // If a shoulder button is pressed, keypad is ignored
  // 1 shoulder button and 1 disc direction can be hit at the same time
  // 2 or 3 shoulder buttons at the same time basically negates all pushes 
  // keypad pressed ignores disc

    // Check shoulder buttons first
    rawData[28] = rawData[29] = (intRawData & 0b00001010) == 0b00001010 ? 1 : 0;
    rawData[30] = (intRawData & 0b00000110) == 0b00000110 ? 1 : 0;
    rawData[31] = (intRawData & 0b00001100) == 0b00001100 ? 1 : 0;

    byte numPushed = 0;
    for(int i = 29; i < 32; ++i)
    {
      if (rawData[i])
        numPushed++;
    }

    if (numPushed > 1)
    {
      for(int i = 0; i < 32; ++i)
        rawData[i] = 0;
    }
    else
    {
      bool keyPadPushed = false;
      if (numPushed == 0)
      {
        rawData[16] = (intRawData & 0b00011000) == 0b00011000 ? 1 : 0;
        rawData[17] = (intRawData & 0b00010100) == 0b00010100 ? 1 : 0;
        rawData[18] = (intRawData & 0b00010010) == 0b00010010 ? 1 : 0;
        rawData[19] = (intRawData & 0b00101000) == 0b00101000 ? 1 : 0;
        rawData[20] = (intRawData & 0b00100100) == 0b00100100 ? 1 : 0;
        rawData[21] = (intRawData & 0b00100010) == 0b00100010 ? 1 : 0;
        rawData[22] = (intRawData & 0b01001000) == 0b01001000 ? 1 : 0;
        rawData[23] = (intRawData & 0b01000100) == 0b01000100 ? 1 : 0;
        rawData[24] = (intRawData & 0b01000010) == 0b01000010 ? 1 : 0;
        rawData[25] = (intRawData & 0b10001000) == 0b10001000 ? 1 : 0;
        rawData[26] = (intRawData & 0b10000100) == 0b10000100 ? 1 : 0;
        rawData[27] = (intRawData & 0b10000010) == 0b10000010 ? 1 : 0;
  
        for(int i = 16; i < 28; ++i)
          if (rawData[i])
          {
            keyPadPushed = true;
            break;
          }
      }
      else
      {
        for(int i = 16; i < 28; ++i)
        rawData[i] = 0;      
      }
  
      if (!keyPadPushed)
      {
        byte mask = 0b11110001;
      
        rawData[0]  = (intRawData & mask) == 0b01000000 ? 1 : 0;
        rawData[1]  = (intRawData & mask) == 0b01000001 ? 1 : 0;
        rawData[2]  = (intRawData & mask) == 0b01100001 ? 1 : 0;
        rawData[3]  = (intRawData & mask) == 0b01100000 ? 1 : 0;
        rawData[4]  = (intRawData & mask) == 0b00100000 ? 1 : 0;
        rawData[5]  = (intRawData & mask) == 0b00100001 ? 1 : 0;
        rawData[6]  = (intRawData & mask) == 0b00110001 ? 1 : 0;
        rawData[7]  = (intRawData & mask) == 0b00110000 ? 1 : 0;
        rawData[8]  = (intRawData & mask) == 0b00010000 ? 1 : 0;
        rawData[9]  = (intRawData & mask) == 0b00010001 ? 1 : 0;
        rawData[10] = (intRawData & mask) == 0b10010001 ? 1 : 0;
        rawData[11] = (intRawData & mask) == 0b10010000 ? 1 : 0;
        rawData[12] = (intRawData & mask) == 0b10000000 ? 1 : 0;
        rawData[13] = (intRawData & mask) == 0b10000001 ? 1 : 0;
        rawData[14] = (intRawData & mask) == 0b11000001 ? 1 : 0;
        rawData[15] = (intRawData & mask) == 0b11000000 ? 1 : 0;  
      }
      else
      {
        for(int i = 0; i < 16; ++i)
          rawData[i] = 0;          
      }
    }

    writeIntellivisionDataToSerial();
}

void sendIntellivisionData_Raw()
{
    // The raw behavior of multiple buttons pushes is completely bizarre.
    // I am trying to replicate how bizare it is!
    
    // Check shoulder buttons first
    rawData[28] = rawData[29] = (intRawData & 0b00001010) == 0b00001010 ? 1 : 0;
    rawData[30] = (intRawData & 0b00000110) == 0b00000110 ? 1 : 0;
    rawData[31] = (intRawData & 0b00001100) == 0b00001100 ? 1 : 0;

    bool shouldersPushed = false;
    if (rawData[28] == 1 || rawData[30] == 1 || rawData[31] == 1)
      shouldersPushed = true;

    rawData[16] = (intRawData & 0b00011000) == 0b00011000 ? 1 : 0;
    rawData[17] = (intRawData & 0b00010100) == 0b00010100 ? 1 : 0;
    rawData[18] = (intRawData & 0b00010010) == 0b00010010 ? 1 : 0;
    rawData[19] = (intRawData & 0b00101000) == 0b00101000 ? 1 : 0;
    rawData[20] = (intRawData & 0b00100100) == 0b00100100 ? 1 : 0;
    rawData[21] = (intRawData & 0b00100010) == 0b00100010 ? 1 : 0;
    rawData[22] = (intRawData & 0b01001000) == 0b01001000 ? 1 : 0;
    rawData[23] = (intRawData & 0b01000100) == 0b01000100 ? 1 : 0;
    rawData[24] = (intRawData & 0b01000010) == 0b01000010 ? 1 : 0;
    rawData[25] = (intRawData & 0b10001000) == 0b10001000 ? 1 : 0;
    rawData[26] = (intRawData & 0b10000100) == 0b10000100 ? 1 : 0;
    rawData[27] = (intRawData & 0b10000010) == 0b10000010 ? 1 : 0;

    // Keypad takes precedent, as long as all pushed buttons are in
    // the same column and shoulders are not "active"
    bool keypadPressed = false;
    if ((rawData[16] && rawData[19] && rawData[22] && rawData[25])
        || (rawData[17] && rawData[20] && rawData[23] && rawData[26])
        || (rawData[18] && rawData[21] && rawData[24] && rawData[27]))
      keypadPressed = !shouldersPushed;
    
    if (!keypadPressed)
    {
      // If shoulders are pushed its a bit check, if shoulders
      // are not active its a full equal check.
      byte mask = shouldersPushed ? 0b11110001 : 0xFF;
    
      rawData[0]  = (intRawData & mask) == 0b01000000 ? 1 : 0;
      rawData[1]  = (intRawData & mask) == 0b01000001 ? 1 : 0;
      rawData[2]  = (intRawData & mask) == 0b01100001 ? 1 : 0;
      rawData[3]  = (intRawData & mask) == 0b01100000 ? 1 : 0;
      rawData[4]  = (intRawData & mask) == 0b00100000 ? 1 : 0;
      rawData[5]  = (intRawData & mask) == 0b00100001 ? 1 : 0;
      rawData[6]  = (intRawData & mask) == 0b00110001 ? 1 : 0;
      rawData[7]  = (intRawData & mask) == 0b00110000 ? 1 : 0;
      rawData[8]  = (intRawData & mask) == 0b00010000 ? 1 : 0;
      rawData[9]  = (intRawData & mask) == 0b00010001 ? 1 : 0;
      rawData[10] = (intRawData & mask) == 0b10010001 ? 1 : 0;
      rawData[11] = (intRawData & mask) == 0b10010000 ? 1 : 0;
      rawData[12] = (intRawData & mask) == 0b10000000 ? 1 : 0;
      rawData[13] = (intRawData & mask) == 0b10000001 ? 1 : 0;
      rawData[14] = (intRawData & mask) == 0b11000001 ? 1 : 0;
      rawData[15] = (intRawData & mask) == 0b11000000 ? 1 : 0;  
    }
    else  // disk is inactive on a "typical" keypad push
    {
      for(int i = 0; i < 16; ++i)
        rawData[i] = 0;
    }

    writeIntellivisionDataToSerial();
}

#define AJ_COLUMN1   8
#define AJ_COLUMN2   9
#define AJ_COLUMN3  10
#define AJ_COLUMN4  11

inline void read_JaguarData()
{
  WAIT_FALLING_EDGEB(0);
  asm volatile ( MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
  rawData[3] = (PIND & 0b11111000);

  WAIT_FALLING_EDGEB(1);
  asm volatile ( MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
  rawData[2] = (PIND & 0b11111000);
  
  WAIT_FALLING_EDGEB(2);
  asm volatile ( MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
  rawData[1] = (PIND & 0b11111000);
  
  WAIT_FALLING_EDGEB(3);
  asm volatile ( MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
  rawData[0] = (PIND & 0b11111100);
  
}


inline void sendRawJaguarData()
{
    #ifndef DEBUG
    Serial.write (rawData[0]);
    Serial.write (rawData[1]);
    Serial.write (rawData[2]);
    Serial.write (rawData[3]);
    Serial.write( SPLIT );
    #else 
    Serial.print((rawData[0] & 0b00000100) == 0  ? "P" : "0");
    Serial.print((rawData[0] & 0b00001000) == 0  ? "A" : "0");
    Serial.print((rawData[0] & 0b00010000) == 0  ? "R" : "0");
    Serial.print((rawData[0] & 0b00100000) == 0  ? "L" : "0");
    Serial.print((rawData[0] & 0b01000000) == 0  ? "D" : "0");
    Serial.print((rawData[0] & 0b10000000) == 0  ? "U" : "0");

    //Serial.print((rawData[1] & 0b00000100) == 0  ? "P" : "0");
    Serial.print((rawData[1] & 0b00001000) == 0  ? "B" : "0");
    Serial.print((rawData[1] & 0b00010000) == 0  ? "1" : "0");
    Serial.print((rawData[1] & 0b00100000) == 0  ? "4" : "0");
    Serial.print((rawData[1] & 0b01000000) == 0  ? "7" : "0");
    Serial.print((rawData[1] & 0b10000000) == 0  ? "*" : "0");

    //Serial.print((rawData[2] & 0b00000100) == 0  ? "P" : "0");
    Serial.print((rawData[2] & 0b00001000) == 0  ? "C" : "0");
    Serial.print((rawData[2] & 0b00010000) == 0  ? "2" : "0");
    Serial.print((rawData[2] & 0b00100000) == 0  ? "5" : "0");
    Serial.print((rawData[2] & 0b01000000) == 0  ? "8" : "0");
    Serial.print((rawData[2] & 0b10000000) == 0  ? "Z" : "0");

    //Serial.print((rawData[3] & 0b00000100) == 0  ? "P" : "0");
    Serial.print((rawData[3] & 0b00001000) == 0  ? "O" : "0");
    Serial.print((rawData[3] & 0b00010000) == 0  ? "3" : "0");
    Serial.print((rawData[3] & 0b00100000) == 0  ? "6" : "0");
    Serial.print((rawData[3] & 0b01000000) == 0  ? "9" : "0");
    Serial.print((rawData[3] & 0b10000000) == 0  ? "#" : "0");

    Serial.print("\n");
    #endif
}


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Update loop definitions for the various console modes.

inline void loop_GC()
{
    if (!seenGC2N64)
    {
      noInterrupts();
      read_oneWire< GC_PIN >( GC_PREFIX + GC_BITCOUNT );
      interrupts();
      if (checkPrefixGC()) {
        sendRawData( GC_PREFIX, GC_BITCOUNT );
      }
      else if (checkPrefixGBA() ) {
        sendRawGBAData();
      }
      else if (checkBothGCPrefixOnRaphnet()) {
         // Sets seenGC2N64RaphnetAdapter to true
      }
    }
    else
    {
      noInterrupts();
      read_oneWire< GC_PIN >( 34+GC_PREFIX + GC_BITCOUNT );
      interrupts();
      if (checkBothGCPrefixOnRaphnet())
        sendRawData(34+GC_PREFIX, GC_BITCOUNT);
      else if (checkPrefixGC())
        sendRawData( GC_PREFIX, GC_BITCOUNT );
      else if (checkPrefixGBA() )
        sendRawGBAData();
    }
}

inline void loop_N64()
{
    noInterrupts();
    read_N64();
    interrupts();
    if( checkPrefixN64() ) {
        sendN64Data( 2 , N64_BITCOUNT);
    }
    else  // This makes no sense, but its needed after command 0x0 or else you get garbage on the line
      delay(2);
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

inline void loop_NES()
{
    noInterrupts();
#ifdef MODE_2WIRE_SNES
    read_shiftRegister< SNES_LATCH , SNES_DATA , true >( NES_BITCOUNT );
#else
    read_shiftRegister< SNES_LATCH , SNES_DATA , SNES_CLOCK >( NES_BITCOUNT );
#endif
    interrupts();
    sendRawData( 0 , NES_BITCOUNT );
}

inline void loop_Sega()
{
  currentState = segaController.getState();
  sendRawSegaData();
}

inline void loop_Genesis_Mouse()
{
  segaController.getMouseState(rawData);
  sendRawSegaMouseData();
}

inline void loop_Classic()
{
  currentState = classicController.getState();
  sendRawSegaData();
}

inline void loop_BoosterGrip()
{
  currentState = boosterGrip.getState();
  sendRawSegaData();
}

inline void loop_Playstation2()
{
  noInterrupts();
  read_Playstation();
  interrupts();
  sendRawPs2Data();
}

inline void loop_TG16()
{
  noInterrupts();
  read_TgData();
  interrupts();
  sendRawTgData();
}

inline void loop_SS()
{
  noInterrupts();
  read_SSData();
  interrupts();
  sendRawSSDataV2();
}

inline void loop_SS3D()
{
  noInterrupts();
  read_SS3DData();
 interrupts();
  sendRawSS3DData();
}

inline void loop_NeoGeo()
{
  noInterrupts();
  read_NeoGeo();
 interrupts();
  sendNeoGeoData();
}

inline void loop_3DO()
{
    noInterrupts();
    byte bits = read_3do< ThreeDO_LATCH , ThreeDO_DATA , ThreeDO_CLOCK >( );
    interrupts();
    sendRawData( 0 , bits );
}


inline void loop_Intellivision()
{
    noInterrupts();
    read_IntellivisionData();
    interrupts();
    if(INT_SANE_BEHAVIOR)
      sendIntellivisionData_Sane();
    else
      sendIntellivisionData_Raw();
}

inline void loop_Jaguar()
{
  noInterrupts();
  read_JaguarData();
  interrupts();
  sendRawJaguarData();
  delay(1);
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch main loop definition.
void loop()
{
#ifdef MODE_GC
    loop_GC();
#elif defined MODE_N64
    loop_N64();
#elif defined MODE_SNES
    loop_SNES();
#elif defined MODE_NES
    loop_NES();
#elif defined MODE_SEGA
    loop_Sega();
#elif defined MODE_CLASSIC
    loop_Classic();
#elif defined MODE_BOOSTER_GRIP
    loop_BoosterGrip();
#elif defined MODE_PLAYSTATION
    loop_Playstation2();
#elif defined MODE_TG16
    loop_TG16();
#elif defined MODE_SATURN
    loop_SS();
#elif defined MODE_SATURN3D
    loop_SS3D();
#elif defined MODE_NEOGEO
    loop_NeoGeo();
#elif defined MODE_3DO
    loop_3DO();
#elif defined MODE_INTELLIVISION
    loop_Intellivision();
#elif defined MODE_GENESIS_MOUSE
    loop_Genesis_Mouse();
#elif defined MODE_JAGUAR
    loop_Jaguar();
#elif defined MODE_COLECOVISION
    loop_ColecoVision();
#elif defined MODE_DETECT
    if( !PINC_READ( MODEPIN_SNES ) ) {
        loop_SNES();
    } else if( !PINC_READ( MODEPIN_N64 ) ) {
        loop_N64();
    } else if( !PINC_READ( MODEPIN_GC ) ) {
        loop_GC();
    } else {
        loop_NES();
    }
#endif
}
