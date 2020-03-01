#ifndef PlayStationSpy_h
#define PlayStationSpy_h

#include "ControllerSpy.h"

class PlayStationSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        unsigned char rawData[152]; // 8 + 16 + 128
        unsigned char playstationCommand[8];
};

#endif
