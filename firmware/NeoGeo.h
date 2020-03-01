#ifndef NeoGeoSpy_h
#define NeoGeoSpy_h

#include "ControllerSpy.h"

class NeoGeoSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        unsigned char rawData[10];
};

#endif

