#ifndef NESSpy_h
#define NESSpy_h

#include "ControllerSpy.h"

class NESSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void updateState();

    private:
        unsigned char rawData[NES_BITCOUNT * 3];
};

#endif
