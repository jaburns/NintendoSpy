#ifndef IntellivisionSpy_h
#define IntellivisionSpy_h

#include "ControllerSpy.h"

class IntellivisionSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void updateState();

    private:
        byte intRawData;
        unsigned char rawData[32];

	  static const unsigned char buttonMasks[32];
};

#endif
