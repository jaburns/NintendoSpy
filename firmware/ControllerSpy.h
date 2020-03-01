#ifndef ControllerSpy_h
#define ControllerSpy_h

#include "common.h"

class ControllerSpy {
    public:
        virtual void setup();
        virtual void loop();
        virtual void writeSerial();
        virtual void debugSerial();
        virtual void updateState();
};

#endif
