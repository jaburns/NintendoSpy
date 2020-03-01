#ifndef ControllerSpy_h
#define ControllerSpy_h

#include "common.h"

class ControllerSpy {
	public:
		virtual void setup()
    {
      common_pin_setup();
    }
		virtual void loop();
		virtual void writeSerial();
		virtual void updateState();
};

#endif
