#ifndef FMTownsSpy_h
#define FMTownsSpy_h

#include "ControllerSpy.h"

class FMTownsSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        unsigned char rawData[9];
};

#endif
