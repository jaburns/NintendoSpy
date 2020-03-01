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
#include "GenesisMouse.h"
#include "SMS.h"
#include "Saturn.h"
#include "Saturn3D.h"

#include "ColecoVision.h"
#include "FMTowns.h"
#include "Intellivision.h"
#include "Jaguar.h"
#include "NeoGeo.h"
#include "PCFX.h"
#include "PlayStation.h"
#include "TG16.h"
#include "ThreeDO.h"

#if defined(MODE_NES)
NESSpy NESSpy;
#elif defined(MODE_SNES)
SNESSpy SNESSpy;
#elif defined(MODE_N64)
N64Spy N64Spy;
#elif defined(MODE_GC)
GCSpy GCSpy;
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
#elif defined(MODE_COLECOVISION)
ColecoVisionSpy ColecoVisionSpy;
#elif defined(MODE_FMTOWNS)
FMTownsSpy FMTownsSpy;
#elif defined(MODE_INTELLIVISION)
IntellivisionSpy IntelliVisionSpy;
#elif defined(MODE_JAGUAR)
JaguarSpy JaguarSpy;
#elif defined(MODE_NEOGEO)
NeoGeoSpy NeoGeoSpy;
#elif defined(MODE_PCFX)
PCFXSpy PCFXSpy;
#elif defined(MODE_PLAYSTATION)
PlayStationSpy PlayStationSpy;
#elif defined(MODE_TG16)
TG16Spy TG16Spy;
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
#elif defined(MODE_BOOSTER_GRIP)
    BoosterGripSpy.setup();
#elif defined(MODE_GENESIS)
    GenesisSpy.setup();
#elif defined(MODE_GENESIS_MOUSE)
    GenesisMouseSpy.setup();
#elif defined(MODE_SMS)
    SMSSpy.setup();
#elif defined(MODE_SMS_ON_GENESIS)
    SMSOnGenesisSpy.setup(SMSSpy::OUTPUT_GENESIS);
#elif defined(MODE_SATURN)
    SaturnSpy.setup();
#elif defined(MODE_SATURN3D)
    Saturn3DSpy.setup();
#elif defined(MODE_COLECOVISION)
    ColecoVisionSpy.setup();
#elif defined(MODE_FMTOWNS)
    FMTownsSpy.setup();
#elif defined(MODE_INTELLIVISION)
    IntelliVisionSpy.setup();
#elif defined(MODE_JAGUAR)
    JaguarSpy.setup();
#elif defined(MODE_NEOGEO)
    NeoGeoSpy.setup();
#elif defined(MODE_PCFX)
    PCFXSpy.setup();
#elif defined(MODE_PLAYSTATION)
    PlayStationSpy.setup();
#elif defined(MODE_TG16)
    TG16Spy.setup();
#elif defined(MODE_ThreeDO)
    ThreeDOSpy.setup();
#elif defined(MODE_DETECT)
    if ( !PINC_READ(MODEPIN_SNES)) {
        SNESSpy.setup();
    } else if ( !PINC_READ(MODEPIN_N64))  {
        N64Spy.setup();
    } else if ( !PINC_READ(MODEPIN_GC)) {
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
#elif defined(MODE_COLECOVISION)
    ColecoVisionSpy.loop();
#elif defined(MODE_FMTOWNS)
    FMTownsSpy.loop();
#elif defined(MODE_INTELLIVISION)
    IntelliVisionSpy.loop();
#elif defined(MODE_JAGUAR)
    JaguarSpy.loop();
#elif defined(MODE_NEOGEO)
    NeoGeoSpy.loop();
#elif defined(MODE_PCFX)
    PCFXSpy.loop();
#elif defined(MODE_PlayStation)
    PlayStationSpy.loop();
#elif defined(MODE_TG16)
    TG16Spy.loop();
#elif defined(MODE_ThreeDO)
    ThreeDOSpy.loop();
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
