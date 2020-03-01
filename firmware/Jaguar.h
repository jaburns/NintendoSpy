#ifndef JaguarSpy_h
#define JaguarSpy_h

#include "ControllerSpy.h"

class JaguarSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void updateState();

    private:
        unsigned char rawData[4];
};

#endif
