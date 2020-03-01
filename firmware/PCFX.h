#ifndef PCFXSpy_h
#define PCFXSpy_h

#include "ControllerSpy.h"

class PCFXSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void updateState();

    private:
        unsigned char rawData[8];
};

#endif
