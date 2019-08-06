/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Teensy 3.5
// v3.2
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns

// ---------- Uncomment one of these options to select operation mode --------------
//#define MODE_GC
//#define MODE_N64
//#define MODE_SNES
//#define MODE_NES
//#define MODE_DREAMCAST
//#define MODE_WII
//#define MODE_CD32

//Bridge one of the GND to the right ping IN to enable your selected mode
//#define MODE_DETECT

// Uncomment this for serial debugging output
//#define DEBUG

#define PIN_READ( pin )  ((GPIOD_PDIR & 0xFF)&(1<<(pin)))

#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );
#define WAIT_LEADING_EDGE( pin ) while( PIN_READ(pin) ); while( !PIN_READ(pin) );

#define DETECT_FALLING_EDGE rawData[byteCount] = (GPIOD_PDIR & 0x3); do { prevPin = rawData[byteCount]; rawData[byteCount] = (GPIOD_PDIR & 0x3); } while( rawData[byteCount] >= prevPin);

#define MODEPIN_SNES        33
#define MODEPIN_N64         34
#define MODEPIN_GC          35
#define MODEPIN_DREAMCAST   36
#define MODEPIN_WII         37

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

#define CD32_LATCH    5  // Digital Pin 5
#define CD32_DATA     7  // Digital Pin 7
#define CD32_CLOCK    6  // Digital Pin 6
#define CD32_BITCOUNT 7

#define ZERO  '\0'  // Use a byte value of 0x00 to represent a bit with value 0.
#define ONE    '1'  // Use an ASCII one to represent a bit with value 1.  This makes Arduino debugging easier.
#define SPLIT '\n'  // Use a new-line character to split up the controller state packets.

// Declare some space to store the bits we read from a controller.
byte      rawData[16000];
byte      cleanData[274];
byte*     p;
int       byteCount;
bool      seenGC2N64 = false;
uint8_t   current_portb = 0;
uint8_t   last_portb;
int       i2c_index  = 0;
bool      isControllerPoll = false;
bool      isControllerID = false;
bool      isEncrypted = false;
byte      encryptionKeySet = 0;
bool      isKeyThing = false;
byte      keyThing[8];

void dreamcast_setup()
{ 
  pinMode(2, INPUT);
  pinMode(14, INPUT);

  p = &rawData[6];
}

void cd32_setup()
{ 
  // GPIOD_PDIR & 0xFF;
  pinMode(2, INPUT_PULLUP);
  pinMode(14, INPUT_PULLUP);
  pinMode(7, INPUT_PULLUP);      
  pinMode(8, INPUT_PULLUP);
  pinMode(6, INPUT_PULLUP);
  pinMode(20, INPUT);
  pinMode(21, INPUT_PULLUP);
  pinMode(5, INPUT);

  // GPIOB_PDIR & 0xF;
  pinMode(16, INPUT_PULLUP);
  pinMode(17, INPUT_PULLUP);  
  pinMode(19, INPUT_PULLUP);
  pinMode(18, INPUT_PULLUP); 
}

void wii_setup()
{ 
  pinMode(19, INPUT);
  pinMode(18, INPUT);
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

  // for MODE_DETECT
  for(int i = 33; i < 40; ++i)
    pinMode(i, INPUT_PULLUP);
    
  #ifdef MODE_DREAMCAST
    dreamcast_setup();
    goto setup1;
  #elif defined MODE_CD32
    cd32_setup();
    goto setup1;
  #elif defined MODE_WII
    wii_setup();
    goto setup1;
  #elif defined MODE_DETECT
    #ifdef MODEPIN_DREAMCAST 
    if( !digitalReadFast( MODEPIN_DREAMCAST ) ) {
        dreamcast_setup();
        goto setup1;
    }
    #endif
    #ifdef MODEPIN_CD32
    if( !digitalReadFast( MODEPIN_CD32 ) ) {
        cd32_setup();
        goto setup1;
    }
    #endif
    #ifdef MODEPIN_WII
      else if( !digitalReadFast( MODEPIN_WII ) ) {
          wii_setup();
          goto setup1;        
      } 
    #endif  
  #endif  

  common_pin_setup();

setup1:

  cleanData[0] = 2;
  cleanData[1] = -1;
  cleanData[46] = '\n';

  seenGC2N64 = false;

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

void read_N64( )
{
    unsigned short bits;
    
    unsigned char *rawDataPtr = &rawData[1];
    byte /*bit7, bit6, bit5, bit4, bit3, */bit2, bit1, bit0;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit7 = (GPIOD_PDIR & 0xFF) & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit6 = (GPIOD_PDIR & 0xFF) & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit5 = (GPIOD_PDIR & 0xFF) & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit4 = (GPIOD_PDIR & 0xFF) & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    // bit3 = (GPIOD_PDIR & 0xFF) & 0b00000100;
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    bit2 = (GPIOD_PDIR & 0xFF) & 0b00000100;
    if (bit2 != 0)  // Controller Reset
    {
          WAIT_FALLING_EDGE( N64_PIN );
          asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
          // bit1 = (GPIOD_PDIR & 0xFF) & 0b00000100;
          WAIT_FALLING_EDGE( N64_PIN );
          asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
          // bit0 = (GPIOD_PDIR & 0xFF) & 0b00000100;
          bits = 25;
          rawData[0] = 0xFF;
          goto read_loop;
    }
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    bit1 = (GPIOD_PDIR & 0xFF) & 0b00000100;
    if (bit1 != 0) // read or write to memory pack (this doesn't work correctly)
    {
          WAIT_FALLING_EDGE( N64_PIN );
          asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
          // bit0 = (GPIOD_PDIR & 0xFF) & 0b00000100;
          bits = 281;
          rawData[0] = 0x02;
          goto read_loop;
    }
    WAIT_FALLING_EDGE( N64_PIN );
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );
    bit0 = (GPIOD_PDIR & 0xFF) & 0b00000100;
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
    *rawDataPtr = (GPIOD_PDIR & 0xFF) & 0b00000100;
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

void read_cd32_controller()
{

    WAIT_FALLING_EDGE(CD32_LATCH);
    rawData[1] = (GPIOD_PDIR & 0xFF);

    for(int i = 2; i < 8; ++i)
    {
      WAIT_FALLING_EDGE(CD32_CLOCK);
      rawData[i] = (GPIOD_PDIR & 0xFF);
    }

    rawData[0] = (GPIOD_PDIR & 0xFF);
    rawData[8] = (GPIOB_PDIR & 0xFF);
    
}
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Sends a packet of controller data over the Arduino serial interface.
inline void sendRawDataCd32( )
{
    #ifndef DEBUG
    for( unsigned char i = 0 ; i < 9 ; i++ ) {
        Serial.write( (rawData[i] & 0b11111101) );
    }
    Serial.write( SPLIT );
    #else
    for( unsigned char i = 1 ; i < 8 ; i++ ) 
    { 
      Serial.print( (rawData[i] & 0b10000000) == 0 ? 0 : 1);
    }
    Serial.print( (rawData[8] &  0b00000001) == 0 ? 0 : 1);
    Serial.print( (rawData[0] &  0b00000100) == 0 ? 0 : 1);
    Serial.print( (rawData[0] &  0b00001000) == 0 ? 0 : 1);
    Serial.print( (rawData[0] & 0b00010000) == 0 ? 0 : 1);
    Serial.print("\n");
    #endif
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
    read_N64();
    interrupts();
    if( checkPrefixN64() ) {
        sendN64Data( 2 , N64_BITCOUNT);
    }
    delay(5);
}

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
  int j = 0;
  byte numFrames = 0;
  for(int i = 0; i < 4; ++i)
  {
    numFrames |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
    numFrames |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
    j += 2;
  }
  j += 16;
  byte dcCommand = 0;
  for (int i = 0; i < 4; ++i)
  {
    dcCommand |= (byte)(((p[j] & 0x02) != 0 ? 1 : 0) << (7 - (i * 2)));
    dcCommand |= (byte)(((p[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (i * 2)));
    j += 2;
  }
  if (dcCommand == 8 && numFrames >= 1)
  {
    uint controllerType = 0;
    for (int i = 0; i < 2; i++)
    {
        for (int k = 0; k < 4; ++k)
        {
            controllerType |= (uint)(((p[j] & 0x02) != 0 ? 1 : 0) << (7 - (k * 2) + (i * 8)));
            controllerType |= (uint)(((p[j + 1] & 0x01) != 0 ? 1 : 0) << (6 - (k * 2) + (i * 8)));
            j += 2;
        }
    }

    if ((controllerType == 1 && numFrames == 3) || (controllerType ==  0x200 && numFrames == 6))
    {
      rawData[byteCount++] = '\n';
      Serial.write(p, byteCount-6);    
    }
  }
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

void loop_wii(void)
{
  last_portb = current_portb;
  noInterrupts();
  current_portb = GPIOB_PDIR & 12;
  interrupts();
  bool bDataReady = current_portb != last_portb;

  if (bDataReady)
  {
    if ((last_portb == 0xC) && (current_portb == 0x8))
    {
      // START
      i2c_index = 0;
    }
    else if ((last_portb == 0x8) && (current_portb == 0xC))
    {
      // STOP
      i2c_index -= (i2c_index % 9);
#ifdef ANALYZE
      byte addr = 0;
      for (int i = 0; i < 7; ++i)
      {
        if (rawData[i] != 0)
          addr |= 1 << (6 - i);
      }
      if (addr != 0x52) return;
      
      Serial.print(i2c_index / 9);
      Serial.print(',');
      //Serial.printf("Addr=0x%02X,", addr);
      Serial.print(addr);
      if (rawData[7] == 0)
        Serial.print("W");
      else
        Serial.print("R");
      Serial.print(",");
      Serial.print(rawData[8] ? "N" : "A");
      Serial.print(",");

      int i = 9;
      byte numbytes = 1;
      while (i < i2c_index)
      {
        byte data = 0;
        for (int j = 0; j < 8; ++j)
        {
          if (rawData[j + i] != 0)
            data |= 1 << (7 - j);
        }
        ++numbytes;
        //Serial.printf("0x%02X,", data);
        Serial.print(data);
        Serial.print(rawData[i + 8] ? "N" : "A");
        Serial.print(" ");
        i += 9;
      }
      Serial.print("\n");
#else
      byte tempData[128];
      tempData[0] = 0;
      for (int i = 0; i < 7; ++i)
      {
        if (rawData[i] != 0)
          tempData[0] |= 1 << (6 - i);
      }

      if (tempData[0] != 0x52) return;

      bool _isControllerID = isControllerID;
      bool _isControllerPoll = isControllerPoll;
      isControllerID = false;
      isControllerPoll = false;
      
      if (rawData[8] != 0) return;

      bool isWrite = rawData[7] == 0; 

      int i = 9;
      byte numbytes = 1;
      while (i < i2c_index)
      {
        tempData[numbytes] = 0;
        for (int j = 0; j < 8; ++j)
        {
          if (rawData[j + i] != 0)
            tempData[numbytes] |= 1 << (7 - j);
        }
        ++numbytes;

        if (!isWrite && i + 8 == i2c_index - 1)
        {
          if (rawData[i + 8] == 0) return;  // Last byte of read ends with NACK
        }
        else
        {
          if (rawData[i + 8] != 0) return; // Every other byte ends with ACK
        }
        i += 9;
      }

      if (isWrite)
      {

        if (numbytes == 2 && tempData[1] == 0)
        {
          isControllerPoll = true;
        }
        else if (numbytes == 3 && tempData[1] == 0xF0 && tempData[2] == 0x55)
        {
          isEncrypted = false;
          cleanData[1] = -1;
        }
        else if (numbytes == 2 && tempData[1] == 0xFA)
        {
          isControllerID = true;
        }
        else if (numbytes == 2 && (tempData[1] == 0x20 || tempData[1] == 0x30))
        {
          isKeyThing = true;
        }
        else if (numbytes == 8 && tempData[1] == 0x40)
        {
          int j = 2;
          for (int i = 0; i < 6; i++)
          {
            cleanData[j] = (tempData[2 + i] & 0xF0);
            cleanData[j + 1] = ((tempData[2 + i] & 0x0F) << 4);
            j += 2;
          }
        }
        else if (numbytes == 8 && tempData[1] == 0x46)
        {
          int j = 14;
          for (int i = 0; i < 6; i++)
          {
            cleanData[j] = (tempData[2 + i] & 0xF0);
            cleanData[j + 1] = ((tempData[2 + i] & 0x0F) << 4);
            j += 2;
          }
        }
        else if (numbytes == 6 && tempData[1] == 0x4C)
        {

          int j = 26;
          for (int i = 0; i < 4; i++)
          {
            cleanData[j] = (tempData[2 + i] & 0xF0);
            cleanData[j + 1] = ((tempData[2 + i] & 0x0F) << 4);
            j += 2;
          }
          isEncrypted = true;
          encryptionKeySet = (encryptionKeySet + 1) % 255;
          if (encryptionKeySet == 10)
            encryptionKeySet = 11;
          cleanData[1] = encryptionKeySet;
        }
      }
      else
      {
        // This is a hack, to handle a problem I don't fully understand
        if (isKeyThing && numbytes == 9)
        {
          keyThing[7] = tempData[1];
          for(int i = 0; i < 7; ++i)
            keyThing[i] = tempData[i+2];
          isKeyThing = false;
        }
        else if (_isControllerID && (numbytes == 7 || numbytes == 9))
        {          
          if (tempData[numbytes - 2] == 0 && tempData[numbytes - 1] == 0)
          {
            cleanData[0] = 0;
          }
          else if (tempData[numbytes - 2] == 1 && tempData[numbytes - 1] == 1)
          {
            cleanData[0] = 1;
          }
          else
            cleanData[0] = 2;
        }
        else if (_isControllerPoll && numbytes >= 7)
        {
          // This is a hack, to handle a problem I don't fully understand
          int  numZeroes = 0;
          int  numMatch = 0;
          if (numbytes == 9)
          { 
            for(int i = 1; i < 9; ++i)
            {
              if (tempData[i] == keyThing[i-1])
                ++numMatch;
              if (tempData[i] == 0)
                ++numZeroes;
            }
            if (numZeroes == 8 || numMatch == 8) return;
          }
 
          int j = 34;
          // NES/SNES Classic return 22 bytes and have the data offset by 2 bytes
          int offset = numbytes == 22 ? 3 : 1;
          for (int i = 0; i < 6; i++)
          {
            cleanData[j] = (tempData[offset + i] & 0xF0);
            cleanData[j + 1] = (tempData[offset + i] << 4);
            j += 2;
          }             

#ifdef DEBUG
          Serial.print(cleanData[0]);
          Serial.print(' ');
          Serial.print(cleanData[1]);
          Serial.print(' ');
          j = 2;
          for (int i = 0; i < 22; ++i)
          {
            byte data = (cleanData[j] | (cleanData[j + 1] >> 4));
            Serial.print(data);
            Serial.print(' ');
            j += 2;
          }
          Serial.print('\n');
#else
          Serial.write(cleanData, 47);
#endif
        }
      }
#endif
    }
    else if ((last_portb == 0x4) && (current_portb == 0xC))
    {
      // ONE
      rawData[i2c_index++] = 1;
    }
    else if ((last_portb == 0x0) && (current_portb == 0x8))
    {
      // ZERO
      rawData[i2c_index++] = 0;
    }
  }
}

inline void loop_CD32()
{
    noInterrupts();
    read_cd32_controller();
    interrupts();
    sendRawDataCd32();
    delay(5);
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
#elif defined MODE_WII
  loop_Wii();
#elif defined MODE_CD32
  loop_CD32();
#elif defined MODE_DETECT
    if( !digitalReadFast( MODEPIN_SNES ) ) {
        loop_SNES();
    } else if( !digitalReadFast( MODEPIN_N64 ) ) {
        loop_N64();
    } else if( !digitalReadFast( MODEPIN_GC ) ) {
        loop_GC();
    } else if( !digitalReadFast( MODEPIN_DREAMCAST ) ) {
        loop_Dreamcast();
    } else if( !digitalReadFast( MODEPIN_WII ) ) {
        loop_Wii();
    } else {
        loop_NES();
    }
#endif

}
