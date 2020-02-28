/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Arduino
// v3.3
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns

#include "common.h"

#include "NES.h"
#include "SNES.h"
#include "N64.h"
#include "GC.h"

#include "3DO.h"
#include "ColecoVision.h"
#include "FMTowns.h"
#include "Jaguar.h"
#include "Intellivision.h"
#include "NeoGeo.h"
#include "PCFX.h"
#include "PlayStation.h"
#include "Saturn.h"
#include "TG16.h"

#include "GenesisControllerSpy.h"
#include "SMSControllerSpy.h"
#include "BoosterGripSpy.h"

GenesisControllerSpy genesisController;

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
SMSControllerSpy SMSController(2, 3, 4, 5, 7, 8);
SMSControllerSpy SMSOnGenesisController(2, 3, 4, 5, 6, 7);

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
BoosterGripSpy boosterGrip(2, 3, 4, 5, 6, 7, 8);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
{
    PORTC = 0xFF; // Set the pull-ups on the port we use to check operation mode.
    DDRC  = 0x00;

  #ifdef MODE_GENESIS
      genesis_pin_setup();
      goto setup1;
  #elif defined MODE_SMS
      sms_pin_setup();
      goto setup1;
  #elif defined MODE_DETECT
      #ifdef MODEPIN_GENESIS
      if( !PINC_READ( MODEPIN_GENESIS ) ) {
          genesis_pin_setup();
          goto setup1;
      }
      #endif
      #ifdef MODEPIN_SMS
      if( !PINC_READ( MODEPIN_SMS ) ) {
          sms_pin_setup();
          goto setup1;
      }
      #endif
  #endif

  common_pin_setup();

setup1:

    Serial.begin( 115200 );
}

void genesis_pin_setup()
{
  for(int i = 2; i <= 6; ++i)
    pinMode(i, INPUT_PULLUP);
}

void sms_pin_setup()
{
  for(int i = 2; i <= 6; ++i)
    pinMode(i, INPUT_PULLUP);
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

inline void sendSMSOnGenesisData()
{
  #ifndef DEBUG
      Serial.write(ZERO);
      Serial.write((currentState & CC_BTN_UP)    ? 1 : 0);
      Serial.write((currentState & CC_BTN_DOWN)  ? 1 : 0);
      Serial.write((currentState & CC_BTN_LEFT)  ? 1 : 0);
      Serial.write((currentState & CC_BTN_RIGHT) ? 1 : 0);
      Serial.write((currentState & CC_BTN_1)     ? 1 : 0);
      Serial.write((currentState & CC_BTN_2)     ? 1 : 0);   
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(SPLIT);  
  #else
  if (currentState != lastState)
  {
      Serial.print("-");
      Serial.print((currentState & SCS_BTN_UP)    ? "U" : "0");
      Serial.print((currentState & SCS_BTN_DOWN)  ? "D" : "0");
      Serial.print((currentState & SCS_BTN_LEFT)  ? "L" : "0");
      Serial.print((currentState & SCS_BTN_RIGHT) ? "R" : "0");
      Serial.print("0");
      Serial.print("0");
      Serial.print((currentState & SCS_BTN_B)     ? "1" : "0");
      Serial.print((currentState & SCS_BTN_C)     ? "2" : "0");
      Serial.print("0");
      Serial.print("0");
      Serial.print("0");
      Serial.print("0");
      Serial.print("\n");
      lastState = currentState;
  }
  #endif  
}

inline void sendRawGenesisData()
{
  #ifndef DEBUG
  for (unsigned char i = 0; i < 13; ++i)
  {
    Serial.write (currentState & (1 << i) ? ONE : ZERO );
  }
  Serial.write( SPLIT );
  #else
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
}

inline void sendRawSMSData()
{
  #ifndef DEBUG
  for (unsigned char i = 0; i < 6; ++i)
  {
    Serial.write (currentState & (1 << i) ? ONE : ZERO );
  }
  Serial.write( SPLIT );
  #else
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
}

inline void sendRawBoosterGripData()
{
  #ifndef DEBUG
  for (unsigned char i = 0; i < 17; ++i)
  {
    Serial.write (currentState & (1 << i) ? ONE : ZERO );
  }
  Serial.write( SPLIT );
  #else
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
}

inline void sendRawGenesisMouseData()
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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Update loop definitions for the various console modes.

inline void loop_Genesis()
{
  currentState = genesisController.getState();
  sendRawGenesisData();
}

inline void loop_GenesisMouse()
{
  genesisController.getMouseState(rawData);
  sendRawGenesisMouseData();
}

inline void loop_SMS()
{
  currentState = SMSController.getState();
  sendRawSMSData();
}

inline void loop_SMS_on_Genesis()
{
  currentState = SMSOnGenesisController.getState();
  sendSMSOnGenesisData();
}

inline void loop_BoosterGrip()
{
  currentState = boosterGrip.getState();
  sendRawBoosterGripData();
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
#elif defined MODE_GENESIS
    loop_Genesis();
#elif defined MODE_SMS
    loop_SMS();
#elif defined MODE_BOOSTER_GRIP
    loop_BoosterGrip();
#elif defined MODE_PLAYSTATION
    loop_Playstation();
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
    loop_GenesisMouse();
#elif defined MODE_JAGUAR
    loop_Jaguar();
#elif defined MODE_COLECOVISION
    loop_ColecoVision();
#elif defined MODE_SMS_ON_GENESIS
    loop_SMS_on_Genesis();
#elif defined MODE_PCFX
    loop_PCFX();
#elif defined MODE_FMTOWNS
    loop_FMTowns();
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
