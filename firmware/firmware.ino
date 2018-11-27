/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Arduino
// v2.0
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns


// ---------- Uncomment one of these options to select operation mode --------------
//#define MODE_GC
//#define MODE_N64
//#define MODE_SNES
//#define MODE_SUPER_GAMEBOY
//#define MODE_NES
//#define MODE_SEGA
//#define MODE_CLASSIC
//#define MODE_BOOSTER_GRIP
//#define MODE_PLAYSTATION
//#define MODE_TG16
//#define MODE_SATURN
//Bridge one of the analog GND to the right analog IN to enable your selected mode
//#define MODE_DETECT
// ---------------------------------------------------------------------------------
// The only reason you'd want to use 2-wire SNES mode is if you built a NintendoSpy
// before the 3-wire firmware was implemented.  This mode is for backwards
// compatibility only. 
//#define MODE_2WIRE_SNES
// ---------------------------------------------------------------------------------
// Uncomment this for MODE_SEGA, MODE_CLASSIC and MODE_BOOSTER_GRIP serial debugging output
//#define DEBUG

#include <SegaControllerSpy.h>
#include <ClassicControllerSpy.h>
#include <BoosterGripSpy.h>

SegaControllerSpy segaController;
word currentState = 0;
word lastState = 0;
byte ssState1 = 0;
byte ssState2 = 0;
byte ssState3 = 0;
byte ssState4 = 0;

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
ClassicControllerSpy classicController(2, 3, 4, 5, 7, 8);

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
BoosterGripSpy boosterGrip(2, 3, 4, 5, 6, 7, 8);

#ifdef MODE_KEYBOARD_CONTROLLER
KeyboardController keyboardController;
#endif

#define PIN_READ( pin )  (PIND&(1<<(pin)))
#define PINC_READ( pin ) (PINC&(1<<(pin)))
#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );
#define WAIT_LEADING_EDGE( pin ) while( PIN_READ(pin) ); while( !PIN_READ(pin) );

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
#define SGB_BITCOUNT   32
#define NES_BITCOUNT    8

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

    for(int i = 2; i <= 6; ++i)
      pinMode(i, INPUT_PULLUP);

    lastState = -1;
    currentState = 0;
    
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
    for( unsigned char i = first ; i < first + count ; i++ ) {
        Serial.print( rawData[i] ? "1" : "0" );
        if (i % 8 == 0)
        Serial.print("|");
    }
    Serial.print("\n");
    #endif
}

#define SS_SELECT1 6
#define SS_SELECT2 7
#define SS_DATA1   2
#define SS_DATA2   3
#define SS_DATA3   4
#define SS_DATA4   5

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

inline void sendRawSSData()
{
    #ifndef DEBUG
    Serial.write ((ssState1 & 0b00000100) ? ONE : ZERO );
    Serial.write ((ssState1 & 0b00001000) ? ONE : ZERO );
    Serial.write ((ssState1 & 0b00010000) ? ONE : ZERO );
    Serial.write ((ssState1 & 0b00100000) ? ONE : ZERO );

    Serial.write ((ssState2 & 0b00000100) ? ONE : ZERO );
    Serial.write ((ssState2 & 0b00001000) ? ONE : ZERO );
    Serial.write ((ssState2 & 0b00010000) ? ONE : ZERO );
    Serial.write ((ssState2 & 0b00100000) ? ONE : ZERO );

    Serial.write ((ssState3 & 0b00000100) ? ONE : ZERO );
    Serial.write ((ssState3 & 0b00001000) ? ONE : ZERO );
    Serial.write ((ssState3 & 0b00010000) ? ONE : ZERO );
    Serial.write ((ssState3 & 0b00100000) ? ONE : ZERO );

    Serial.write ((ssState4 & 0b00010000) ? ONE : ZERO );

    Serial.write( SPLIT );
    #else 
    Serial.print((ssState1 & 0b00000100)    ? "Y" : "0");
    Serial.print((ssState1 & 0b00001000)    ? "Z" : "0");
    Serial.print((ssState1 & 0b00010000)    ? "R" : "0");
    Serial.print((ssState1 & 0b00100000)    ? "X" : "0");

    Serial.print((ssState2 & 0b00000100)    ? "C" : "0");
    Serial.print((ssState2 & 0b00001000)    ? "B" : "0");
    Serial.print((ssState2 & 0b00010000)    ? "S" : "0");
    Serial.print((ssState2 & 0b00100000)    ? "A" : "0");

    Serial.print((ssState3 & 0b00000100)    ? "d" : "0");
    Serial.print((ssState3 & 0b00001000)    ? "u" : "0");
    Serial.print((ssState3 & 0b00010000)    ? "r" : "0");
    Serial.print((ssState3 & 0b00100000)    ? "l" : "0");
    
    Serial.print((ssState4 & 0b00010000)    ? "L" : "0");
 
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

inline void read_Playstation( )
{
  WAIT_FALLING_EDGE(PS_ATT);

  unsigned char bits = 8;
  do {
     WAIT_LEADING_EDGE(PS_CLOCK);
  }
  while( --bits > 0 );

  byte controllerType = 0;
  bits = 0;
  do {
      WAIT_LEADING_EDGE(PS_CLOCK);
      controllerType |= ((PIN_READ(PS_DATA) == 0 ? 0 : 1) << bits);
  }
  while( ++bits < 8 );
  rawData[0] = controllerType;
  
  bits = 0;
  do {
      WAIT_LEADING_EDGE(PS_CLOCK);
  }
  while( ++bits < 8 );
  
  bits = 0;
  do {
      WAIT_LEADING_EDGE(PS_CLOCK);
      rawData[bits+1] = !PIN_READ(PS_DATA);
  }
  while( ++bits < 16 );

  // Read analog sticks for Analog Controller in Red Mode
  if (controllerType == 0x73)
  {
    for(int i = 0; i < 4; ++i)
    {
      bits = 0;
      rawData[17+i] = 0;
      do {
          WAIT_LEADING_EDGE(PS_CLOCK);
          rawData[17+i] |= ((PIN_READ(PS_DATA) == 0 ? 0 : 1) << bits);
      }
      while( ++bits < 8 );
    }
  }
}

inline void sendRawPsData()
{
    #ifndef DEBUG
    Serial.write( rawData[0] );
    for (unsigned char i = 1; i < 17; ++i)
    {
      Serial.write( rawData[i] ? ONE : ZERO );
    }
    Serial.write( rawData[17] );
    Serial.write( rawData[18] );
    Serial.write( rawData[19] );
    Serial.write( rawData[20] );
    Serial.write( SPLIT );
    #else
    Serial.print(rawData[0]);
    Serial.print(" ");
    Serial.print(rawData[1] != 0 ? "S" : "0");
    Serial.print(rawData[2] != 0 ? "5" : "0");
    Serial.print(rawData[3] != 0 ? "6" : "0");
    Serial.print(rawData[4] != 0 ? "T" : "0");
    Serial.print(rawData[5] != 0 ? "U" : "0");
    Serial.print(rawData[6] != 0 ? "R" : "0");
    Serial.print(rawData[7] != 0 ? "D" : "0");
    Serial.print(rawData[8] != 0 ? "L" : "0");
    Serial.print(rawData[9] != 0 ? "1" : "0");
    Serial.print(rawData[10] != 0 ? "2" : "0");
    Serial.print(rawData[11] != 0 ? "3" : "0");
    Serial.print(rawData[12] != 0 ? "4" : "0");
    Serial.print(rawData[13] != 0 ? "/" : "0");
    Serial.print(rawData[14] != 0 ? "O" : "0");
    Serial.print(rawData[15] != 0 ? "X" : "0");
    Serial.print(rawData[16] != 0 ? "Q" : "0");
    Serial.print(rawData[17]);
    Serial.print(rawData[18]);
    Serial.print(rawData[19]);
    Serial.print(rawData[20]);
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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Update loop definitions for the various console modes.

inline void loop_GC()
{
    noInterrupts();
    read_oneWire< GC_PIN >( GC_PREFIX + GC_BITCOUNT );
    interrupts();
    if (checkPrefixGC() ) {
      sendRawData( GC_PREFIX , GC_BITCOUNT );
    }
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

inline void loop_SGB()
{
    noInterrupts();
    read_shiftRegister< SNES_LATCH , SNES_DATA , SNES_CLOCK >( SGB_BITCOUNT );
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

inline void loop_Playstation()
{
  noInterrupts();
  read_Playstation();
  interrupts();
  sendRawPsData();
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
  sendRawSSData();
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
#elif defined MODE_SUPER_GAMEBOY
    loop_SGB();
#elif defined MODE_NES
    loop_NES();
#elif defined MODE_SEGA
    loop_Sega();
#elif defined MODE_CLASSIC
    loop_Classic();
#elif defined MODE_BOOSTER_GRIP
    loop_BoosterGrip();
#elif defined MODE_PLAYSTATION
    loop_Playstation();
#elif defined MODE_TG16
    loop_TG16();
#elif defined MODE_SATURN
    loop_SS();
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
