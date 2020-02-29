/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Firmware for Arduino
// v3.3
// RetroSpy written by zoggins
// NintendoSpy originally written by jaburns

#include "common.h"

#include "NESSpy.h"

#include "SNES.h"
#include "N64.h"
#include "GC.h"

#include "BoosterGrip.h"
#include "Genesis.h"
#include "SMS.h"

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

#if defined(MODE_NES)
NESSpy NESSpy;
#elif defined(MODE_DETECT)
NESSpy NESSpy;
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
{
    PORTC = 0xFF; // Set the pull-ups on the port we use to check operation mode.
    DDRC  = 0x00;

#if defined(MODE_NES)
    NESSpy.setup();
#elif defined(MODE_DETECT)
    if (false /* read SNES_PIN */) {
    } else {
        NESSpy.setup();
    }
#endif

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
    NESSpy.loop();
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
        NESSpy.loop();
    }
#endif
}
