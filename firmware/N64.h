#ifndef N64Spy_h
#define N64Spy_h

#include "ControllerSpy.h"

class N64Spy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void updateState();

    private:
        bool checkPrefixN64();
        unsigned char rawData[512]; // This can probably be lowered.
};

#endif
