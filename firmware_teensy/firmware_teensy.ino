/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Teensy 3.5
// v2.0
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns


// ---------- Uncomment one of these options to select operation mode --------------
//#define MODE_GC
//#define MODE_N64
//#define MODE_SNES
#define MODE_NES

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

#define PIN_READ( pin )  ((GPIOD_PDIR & 0xFF)&(1<<(pin)))

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );

#define SNES_LATCH           3
#define SNES_DATA            4
#define SNES_CLOCK           6
#define SNES_BITCOUNT       16
#define SNES_BITCOUNT_EXT   32
#define NES_BITCOUNT         8

#define ZERO  '\0'  // Use a byte value of 0x00 to represent a bit with value 0.
#define ONE    '1'  // Use an ASCII one to represent a bit with value 1.  This makes Arduino debugging easier.
#define SPLIT '\n'  // Use a new-line character to split up the controller state packets.

// Declare some space to store the bits we read from a controller.
unsigned char rawData[ 256 ];

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
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

    delay(5000);

    Serial.begin( 115200 );
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

inline void loop_NES()
{
    noInterrupts();
    read_shiftRegister< SNES_LATCH , SNES_DATA , SNES_CLOCK >( NES_BITCOUNT );
    interrupts();
    sendRawData( 0 , NES_BITCOUNT );
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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch main loop definition.
void loop()
{
#ifdef MODE_NES
    loop_NES();
#endif

}
