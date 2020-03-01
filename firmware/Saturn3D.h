#ifndef Saturn3DSpy_h
#define Saturn3DSpy_h

#include "ControllerSpy.h"

class Saturn3DSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void updateState();

    private:
        unsigned char rawData[64];
};

#endif
