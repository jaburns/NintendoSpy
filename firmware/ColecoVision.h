#ifndef ColecoVisionSpy_h
#define ColecoVisionSpy_h

#include "ControllerSpy.h"

class ColecoVisionSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
};

#endif
