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

#include "ThreeDO.h"

#include "BoosterGrip.h"
#include "Genesis.h"
#include "SMS.h"
#include "Saturn.h"
#include "Saturn3D.h"

#include "ColecoVision.h"
#include "FMTowns.h"
#include "Jaguar.h"
#include "Intellivision.h"
#include "NeoGeo.h"
#include "PCFX.h"
#include "PlayStation.h"
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
#elif defined(MODE_BOOSTER_GRIP)
BoosterGripSpy BoosterGripSpy;
#elif defined(MODE_GENESIS)
GenesisSpy GenesisSpy;
#elif defined(MODE_GENESIS_MOUSE)
GenesisMouseSpy GenesisMouseSpy;
#elif defined(MODE_SMS)
SMSSpy SMSSpy;
#elif defined(MODE_SMS_ON_GENESIS)
SMSSpy SMSOnGenesisSpy;
#elif defined(MODE_SATURN)
SaturnSpy SaturnSpy;
#elif defined(MODE_SATURN3D)
Saturn3DSpy Saturn3DSpy;
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
#elif defined(MODE_BOOSTER_GRIP)
    BoosterGripSpy.setup();
#elif defined(MODE_GENESIS)
    GenesisSpy.setup();
#elif defined(MODE_GENESIS_MOUSE)
    GenesisMouseSpy.setup();
#elif defined(MODE_SMS)
    SMSSpy.setup();
#elif defined(MODE_SMS_ON_GENESIS)
    SMSOnGenesisSpy.setup(SMSOnGenesisSpy::OUTPUT_GENESIS);
#elif defined(MODE_SATURN)
    SaturnSpy.setup();
#elif defined(MODE_DETECT)
    if (false /* read SNES_MODEPIN */) {
        SNESSpy.setup();
    } else if (false /* read N64_MODEPIN */) {
        N64Spy.setup();
    } else if (false /* read GC_MODEPIN */) {
        GCSpy.setup();
    } else {
        NESSpy.setup();
    }
#endif

    Serial.begin( 115200 );
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Arduino sketch main loop definition.
void loop()
{
#if defined(MODE_GC)
    GCSpy.loop();
#elif defined(MODE_N64)
    N64Spy.loop();
#elif defined(MODE_SNES)
    SNESSpy.loop();
#elif defined(MODE_NES)
    NESSpy.loop();
#elif defined(MODE_ThreeDO)
    ThreeDO.loop();
#elif defined(MODE_BOOSTER_GRIP)
    BoosterGripSpy.loop();
#elif defined(MODE_GENESIS)
    GenesisSpy.loop();
#elif defined(MODE_GENESIS_MOUSE)
    GenesisMouseSpy.loop();
#elif defined(MODE_SMS)
    SMSSpy.loop();
#elif defined(MODE_SMS_ON_GENESIS)
    SMSOnGenesisSpy.loop();
#elif defined(MODE_SATURN)
    SaturnSpy.loop();
#elif defined(MODE_SATURN3D)
    Saturn3DSpy.loop();
#elif defined(MODE_PLAYSTATION)
    loop_Playstation();
#elif defined(MODE_TG16)
    loop_TG16();
#elif defined(MODE_NEOGEO)
    loop_NeoGeo();
#elif defined(MODE_INTELLIVISION)
    loop_Intellivision();
#elif defined(MODE_JAGUAR)
    loop_Jaguar();
#elif defined(MODE_COLECOVISION)
    loop_ColecoVision();
#elif defined(MODE_PCFX)
    loop_PCFX();
#elif defined(MODE_FMTOWNS)
    loop_FMTowns();
#elif defined(MODE_DETECT)
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
