#ifndef NESSpy_h
#define NESSpy_h

#include "ControllerSpy.h"

class NESSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        unsigned char rawData[NES_BITCOUNT * 3];
};

#endif
