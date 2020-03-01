#ifndef JaguarSpy_h
#define JaguarSpy_h

#include "ControllerSpy.h"

class JaguarSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void updateState();

    private:
        unsigned char rawData[4];
};

#endif
