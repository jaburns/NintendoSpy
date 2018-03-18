/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Arduino
// v2.0
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns


// ---------- Uncomment one of these options to select operation mode --------------
//#define MODE_GC
//#define MODE_N64
//#define MODE_SNES
//#define MODE_NES
//#define MODE_SEGA
//#define MODE_CLASSIC
//#define MODE_BOOSTER_GRIP
//Bridge one of the analog GND to the right analog IN to enable your selected mode
//#define MODE_DETECT
// ---------------------------------------------------------------------------------
// The only reason you'd want to use 2-wire SNES mode is if you built a NintendoSpy
// before the 3-wire firmware was implemented.  This mode is for backwards
// compatibility only.
//#define MODE_2WIRE_SNES
// ---------------------------------------------------------------------------------
// Uncomment this for MODE_SEGA, MODE_CLASSIC and MODE_BOOSTER_GRIP serial debugging output
//#define SEGADEBUG

#include <SegaControllerSpy.h>
#include <ClassicController.h>
#include <BoosterGrip.h>

SegaControllerSpy segaController;
word currentState = 0;
word lastState = 0;

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
ClassicController classicController(2, 3, 4, 5, 7, 8);

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
BoosterGrip boosterGrip(2, 3, 4, 5, 6, 7, 8);
 
#define PIN_READ( pin )  (PIND&(1<<(pin)))
#define PINC_READ( pin ) (PINC&(1<<(pin)))
#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );

#define MODEPIN_SNES 0
#define MODEPIN_N64  1
#define MODEPIN_GC   2

#define N64_PIN        2
#define N64_PREFIX     9
#define N64_BITCOUNT  32

#define SNES_LATCH      3
#define SNES_DATA       4
#define SNES_CLOCK      6
#define SNES_BITCOUNT  16
#define  NES_BITCOUNT   8

#define GC_PIN        5
#define GC_PREFIX    25
#define GC_BITCOUNT  64

#define ZERO  '\0'  // Use a byte value of 0x00 to represent a bit with value 0.
#define ONE    '1'  // Use an ASCII one to represent a bit with value 1.  This makes Arduino debugging easier.
#define SPLIT '\n'  // Use a new-line character to split up the controller state packets.




// Declare some space to store the bits we read from a controller.
unsigned char rawData[ 128 ];


/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
{
    #ifndef MODE_SEGA
    #ifndef MODE_CLASSIC
    PORTC = 0xFF; // Set the pull-ups on the port we use to check operation mode.
    DDRC  = 0x00;
    PORTD = 0x00;
    DDRD  = 0x00;
    #endif
    #endif
    
    Serial.begin( 115200 );
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

// Verifies that the 9 bits prefixing N64 controller data in 'rawData'
// are actually indicative of a controller state signal.
inline bool checkPrefixN64 ()
{
    if( rawData[0] != 0 ) return false; // 0
    if( rawData[1] != 0 ) return false; // 0
    if( rawData[2] != 0 ) return false; // 0
    if( rawData[3] != 0 ) return false; // 0
    if( rawData[4] != 0 ) return false; // 0
    if( rawData[5] != 0 ) return false; // 0
    if( rawData[6] != 0 ) return false; // 0
    if( rawData[7] == 0 ) return false; // 1
    if( rawData[8] == 0 ) return false; // 1
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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Sends a packet of controller data over the Arduino serial interface.
inline void sendRawData( unsigned char first, unsigned char count )
{
    for( unsigned char i = first ; i < first + count ; i++ ) {
        Serial.write( rawData[i] ? ONE : ZERO );
    }
    Serial.write( SPLIT );
}

inline void sendRawSegaData()
{
  #ifndef SEGADEBUG
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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Update loop definitions for the various console modes.

inline void loop_GC()
{
    noInterrupts();
    read_oneWire< GC_PIN >( GC_PREFIX + GC_BITCOUNT );
    interrupts();
    sendRawData( GC_PREFIX , GC_BITCOUNT );
}

inline void loop_N64()
{
    noInterrupts();
    read_oneWire< N64_PIN >( N64_PREFIX + N64_BITCOUNT );
    interrupts();
    if( checkPrefixN64() ) {
        sendRawData( N64_PREFIX , N64_BITCOUNT );
    }
}

inline void loop_SNES()
{
    noInterrupts();
#ifdef MODE_2WIRE_SNES
    read_shiftRegister_2wire< SNES_LATCH , SNES_DATA , false >( SNES_BITCOUNT );
#else
    read_shiftRegister< SNES_LATCH , SNES_DATA , SNES_CLOCK >( SNES_BITCOUNT );
#endif
    interrupts();
    sendRawData( 0 , SNES_BITCOUNT );
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
