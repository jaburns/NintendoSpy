#ifndef GCSpy_h
#define GCSpy_h

#include "ControllerSpy.h"

class GCSpy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void updateState();

    private:
	bool seenGC2N64 = false;
        bool checkPrefixGBA();
        bool checkPrefixGC();
        bool checkBothGCPrefixOnRaphnet();
        void sendRawGBAData();

        unsigned char rawData[34 + GC_PREFIX + GC_BITCOUNT];
        unsigned char readBits;
};

#endif
