#ifndef PlayStationSpy_h
#define PlayStationSpy_h

#include "ControllerSpy.h"

class PlayStationSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void updateState();

    private:
        unsigned char rawData[152]; // 8 + 16 + 128
#if defined(DEBUG)
        unsigned char playstationCommand[8];
#endif
};

#endif
