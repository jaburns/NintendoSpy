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

#include "BoosterGrip.h"
#include "Genesis.h"
#include "SMS.h"

#include "ThreeDO.h"
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
#elif defined(MODE_SNES)
SNESSpy SNESSpy;
#elif defined(MODE_N64)
N64Spy N64Spy;
#elif defined(MODE_GC)
GCSpy GCSpy;
#elif defined(MODE_ThreeDO)
ThreeDOSpy ThreeDOSpy;
#elif defined(MODE_DETECT)
NESSpy NESSpy;
SNESSpy SNESSpy;
N64Spy N64Spy;
GCSpy GCSpy;
#endif

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// General initialization, just sets all pins to input and starts serial communication.
void setup()
{
    PORTC = 0xFF; // Set the pull-ups on the port we use to check operation mode.
    DDRC  = 0x00;

#if defined(MODE_NES)
    NESSpy.setup();
#elif defined(MODE_SNES)
    SNESSpy.setup();
#elif defined(MODE_N64)
    N64Spy.setup();
#elif defined(MODE_GC)
    GCSpy.setup();
#elif defined(MODE_ThreeDO)
    ThreeDOSpy.setup();
#elif defined(MODE_DETECT)
    if (false /* read SNES_MODEPIN */) {
        SNESSpy.setup();
    else if (false /* read N64_MODEPIN */) {
        N64Spy.setup();
    else if (false /* read GC_MODEPIN */) {
        GCSpy.setup();
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
    GCSpy.loop();
#elif defined MODE_N64
    N64Spy.loop();
#elif defined MODE_SNES
    SNESSpy.loop();
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
#elif defined MODE_ThreeDO
    ThreeDO.loop();
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
        SNESSpy.loop();
    } else if( !PINC_READ( MODEPIN_N64 ) ) {
        N64Spy.loop();
    } else if( !PINC_READ( MODEPIN_GC ) ) {
        GCSpy.loop();
    } else {
        NESSpy.loop();
    }
#endif
}
