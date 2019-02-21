/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Teensy 3.5
// v1.0
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns

// ---------- Uncomment one of these options to select operation mode --------------
//#define MODE_GC
//#define MODE_N64
//#define MODE_SNES
//#define MODE_NES
//#define MODE_DREAMCAST

//Bridge one of the GND to the right ping IN to enable your selected mode
//#define MODE_DETECT

// Uncomment this for serial debugging output
//#define DEBUG

#define PIN_READ( pin )  ((GPIOD_PDIR & 0xFF)&(1<<(pin)))

#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );

#define DETECT_FALLING_EDGE rawData[byteCount] = (GPIOD_PDIR & 0x3); do { prevPin = rawData[byteCount]; rawData[byteCount] = (GPIOD_PDIR & 0x3); } while( rawData[byteCount] >= prevPin);

#define MODEPIN_SNES        33
#define MODEPIN_N64         34
#define MODEPIN_GC          35
#define MODEPIN_DREAMCAST   36

#define N64_PIN        2
#define N64_PREFIX     9
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

#define ZERO  '\0'  // Use a byte value of 0x00 to represent a bit with value 0.
#define ONE    '1'  // Use an ASCII one to represent a bit with value 1.  This makes Arduino debugging easier.
#define SPLIT '\n'  // Use a new-line character to split up the controller state packets.

// Declare some space to store the bits we read from a controller.
byte rawData[16000];
byte* p;
int byteCount;

void dreamcast_setup()
{ 
  pinMode(2, INPUT);
  pinMode(14, INPUT);

  p = &rawData[6];
}

void common_pin_setup()
{
  // GPIOD_PDIR & 0xFF;
  pinMode(2, INPUT_PULLUP);
  pinMode(14, INPUT_PULLUP);
  pinMode(7, INPUT_PULLUP);      
  pinMode(8, INPUT_PULLUP);
  pinMode(6, INPUT_PULLUP);
  pinMode(20, INPUT_PULLUP);
  pinMode(21, INPUT_PULLUP);
  pinMode(5, INPUT_PULLUP);
  
  // GPIOB_PDIR & 0xF;
  pinMode(16, INPUT_PULLUP);
  pinMode(17, INPUT_PULLUP);
  pinMode(19, INPUT_PULLUP);
  pinMode(18, INPUT_PULLUP); 
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
{    
  #ifdef MODE_DREAMCAST
    dreamcast_setup();
  #elif defined MODE_DETECT
    if( !digitalReadFast( MODEPIN_DREAMCAST ) ) {
        dreamcast_setup();
    } else {
        common_pin_setup();
    }
  #else
    common_pin_setup();
  #endif

  delay(5000);
  
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
    return true;
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

inline void loop_NES()
{
    noInterrupts();
    read_shiftRegister< SNES_LATCH , SNES_DATA , SNES_CLOCK >( NES_BITCOUNT );
    interrupts();
    sendRawData( 0 , NES_BITCOUNT );
    delay(5);
}

inline void loop_SNES()
{
    noInterrupts();
    unsigned char bytesToReturn = SNES_BITCOUNT;
    bytesToReturn = read_shiftRegister_SNES< SNES_LATCH , SNES_DATA , SNES_CLOCK >();
    interrupts();
    sendRawData( 0 , bytesToReturn );
    delay(5);
}

inline void loop_N64()
{
    noInterrupts();
    read_oneWire< N64_PIN >( N64_PREFIX + N64_BITCOUNT );
    interrupts();
    if( checkPrefixN64() ) {
        sendRawData( N64_PREFIX , N64_BITCOUNT );
    }
    delay(5);
}

inline void loop_GC()
{
    noInterrupts();
    read_oneWire< GC_PIN >( GC_PREFIX + GC_BITCOUNT );
    interrupts();
    if (checkPrefixGC() ) {
      sendRawData( GC_PREFIX , GC_BITCOUNT );
    }
    else if (checkPrefixGBA() ) {
      sendRawGBAData();
    }
    delay(5);
}

FASTRUN void loop_Dreamcast() 
{

  byte prevPin;

start_state:    
state1:
  interrupts();
  byteCount = 0;
  DETECT_FALLING_EDGE
  if (prevPin != 0x3 || rawData[byteCount] != 0x2)   // Starts with both pins High and pin 1 drops
    goto state1;
  ++byteCount;

//state2:
  DETECT_FALLING_EDGE
  if (prevPin != 0x2 || rawData[byteCount] != 0x0)  // Pin 5 now drops, pin 1 stays low
    goto state1;
  ++byteCount;

state3:
  DETECT_FALLING_EDGE
  if (prevPin != 0x2 || rawData[byteCount] != 0x0)  // Pin 5 drops 3 more times, pin 1 says low
    goto state1;
  else if (++byteCount != 5)
    goto state3;

//state4:
  DETECT_FALLING_EDGE
  if (rawData[byteCount] != 0x01 || prevPin != 0x3) // Pin 5 drops 1 more times, this time Pin 1 is high
    goto state1;
  ++byteCount;

  noInterrupts();
state5:                             // Phase 1
  DETECT_FALLING_EDGE
  if (prevPin == 0x3 && rawData[byteCount] == 0x1)
    goto state7;
  else if (prevPin == 0x02 || prevPin == 0x0)
    goto state1;
  ++byteCount;
  
//state6:                           // Phase 2
  DETECT_FALLING_EDGE
  if (prevPin == 0x01 || prevPin == 0x0)
    goto state1;
  ++byteCount;
  goto state5;

state7:
  interrupts();
#ifndef DEBUG
  rawData[byteCount++] = '\n';
  Serial.write(p, byteCount-6);    
#else
  int j = 0;

  Serial.print(byteCount-6);
  Serial.print("|");

  byte numFrames = 0;
  for(int i = 0; i < 4; ++i)
  {
    numFrames |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
    numFrames |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
    j += 2;
  }
  Serial.print(numFrames);
  
  for(int i = 8; i < 16; i++)
  {
    if (j % 8 == 0 && j != 0)
      Serial.print("|");
    Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
    Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
    j += 2;
  }

  Serial.print("|");
  byte dcCommand = 0;
  for(int i = 0; i < 4; ++i)
  {
    dcCommand |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
    dcCommand |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
    j += 2;
  }
  Serial.print(dcCommand);

  Serial.print("|");
  unsigned int controllerType = 0;
  for(int i = 0; i < 2; i++)
  {
    for(int k = 0; k < 4; ++k)
    {
      controllerType |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(k*2)+(i*8));
      controllerType |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(k*2)+(i*8));
      j += 2;
    }
  }
  Serial.print(controllerType);
  
  j+=16;

  if (dcCommand == 8 && controllerType == 0x40)
  {
    byte keycode[6];

    keycode[1] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[1] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[1] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    
    keycode[0] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[0] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[0] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    Serial.print("|");
    for(int i = 0; i < 4; i++)
    {
      Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
      Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
      j += 2;
    } 

    Serial.print("|");
    for(int i = 0; i < 4; i++)
    {
      Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
      Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
      j += 2;
    } 

    keycode[5] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[5] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[5] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    keycode[4] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[4] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[4] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    keycode[3] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[3] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[3] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    keycode[2] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[2] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[2] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    for(int i = 0; i < 6; ++i)
    {
      Serial.print("|");
      Serial.print(keycode[i]);
    }

  }
  else if (dcCommand == 8 && controllerType == 0x200)
  {
    j += 24;
    Serial.print("|");
    for(int i = 0; i < 4; i++)
    {
      Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
      Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
      j += 2;
    } 

    Serial.print("|");
    unsigned int axis1 = 0;
    for(int i = 1; i >= 0; --i)
    {
      for(int k = 0; k < 4; ++k)
      {
        axis1 |= ((p[j] & 0x02) != 0 ? 1 : 0) << ((7-k*2)+(i*8));
        axis1 |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(k*2)+(i*8));
        j += 2;
      }
    }
    Serial.print(axis1);
    
    Serial.print("|");
    unsigned int axis2 = 0;
    for(int i = 1; i >= 0; --i)
    {
      for(int k = 0; k < 4; ++k)
      {
        axis2 |= ((p[j] & 0x02) != 0 ? 1 : 0) << ((7-k*2)+(i*8));
        axis2 |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(k*2)+(i*8));
        j += 2;
      }
    }
    Serial.print(axis2);

    j += 16;

    Serial.print("|");
    unsigned int axis3 = 0;
    for(int i = 1; i >= 0; --i)
    {
      for(int k = 0; k < 4; ++k)
      {
        axis3 |= ((p[j] & 0x02) != 0 ? 1 : 0) << ((7-k*2)+(i*8));
        axis3 |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(k*2)+(i*8));
        j += 2;
      }
    }
    Serial.print(axis3);
  
  }
  else if (dcCommand == 8 && controllerType == 1)
  {
    Serial.print("|");
    byte ltrigger = 0;
    for(int i = 0; i < 4; ++i)
    {
      ltrigger |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      ltrigger |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    Serial.print(ltrigger);
  
    Serial.print("|");
    byte rtrigger = 0;
    for(int i = 0; i < 4; ++i)
    {
      rtrigger |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      rtrigger |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    Serial.print(rtrigger);
  
    for(int i = 0; i < 8; i++)
    {
      if (j % 8 == 0 && j != 0)
        Serial.print("|");
      Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
      Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
      j += 2;
    } 
  
    j += 16;
  
    Serial.print("|");
    byte joyy = 0;
    for(int i = 0; i < 4; ++i)
    {
      joyy |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      joyy |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    Serial.print(joyy);
  
    Serial.print("|");
    byte joyx = 0;
    for(int i = 0; i < 4; ++i)
    {
      joyx |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      joyx |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    Serial.print(joyx);  
  }
  Serial.print("\n");
#endif
  
  goto start_state;
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
        if (i % 8 == 0 && i != 0)
        Serial.print("|");
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
  Serial.write(rawData[19] ? ONE : ZERO);
  Serial.write(rawData[20] ? ONE : ZERO);
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
  Serial.print(rawData[19] ? "r" : "0");
  Serial.print(rawData[20] ? "l" : "0");
  Serial.print(128);
  Serial.print(128);
  Serial.print(128);
  Serial.print(128);
  Serial.print(0);
  Serial.print(0);
  Serial.print( "\n" );
  #endif
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
#elif defined MODE_DREAMCAST
  loop_Dreamcast();
#elif defined MODE_DETECT
    if( !digitalReadFast( MODEPIN_SNES ) ) {
        loop_SNES();
    } else if( !digitalReadFast( MODEPIN_N64 ) ) {
        loop_N64();
    } else if( !digitalReadFast( MODEPIN_GC ) ) {
        loop_GC();
    } else if( !digitalReadFast( MODEPIN_DREAMCAST ) ) {
        loop_Dreamcast();
    } else {
        loop_NES();
    }
#endif

}