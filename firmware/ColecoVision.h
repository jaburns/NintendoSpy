#ifndef ColecoVisionSpy_h
#define ColecoVisionSpy_h

#include "ControllerSpy.h"

class ColecoVisionSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void updateState();

    private:
};

#endif
