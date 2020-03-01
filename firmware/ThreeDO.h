#ifndef ThreeDOSpy_h
#define ThreeDOSpy_h

#include "ControllerSpy.h"

class ThreeDOSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void updateState();

    private:
        unsigned char rawData[32];
	unsigned char bytesToReturn = 0;
};

#endif
