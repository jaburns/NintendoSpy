#ifndef TG16Spy_h
#define TG16Spy_h

#include "ControllerSpy.h"

class TG16Spy : public ControllerSpy {
    public:
        void loop();
        void writeSerial();
        void updateState();

    private:
        word lastDirections = 0;
        word lastHighButtons = 0;
        word lastButtons = 0;
        bool highButtons = true;
        bool seenHighButtons = false;
};

#endif
