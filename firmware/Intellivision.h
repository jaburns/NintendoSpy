#ifndef IntellivisionSpy_h
#define IntellivisionSpy_h

#include "ControllerSpy.h"

class IntellivisionSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void updateState();

    private:
        byte intRawData;
        unsigned char rawData[32];

	static const unsigned char buttonMasks[32] = {
            0b01000000, 0b01000001, 0b01100001, 0b01100000, 0b00100000, 0b00100001, 0b00110001, 0b00110000,
            0b00010000, 0b00010001, 0b10010001, 0b10010000, 0b10000000, 0b10000001, 0b11000001, 0b11000000,
            0b00011000, 0b00010100, 0b00010010, 0b00101000, 0b00100100, 0b00100010, 0b01001000, 0b01000100,
            0b01000010, 0b10001000, 0b10000100, 0b10000010, 0b00001010, 0b00001010, 0b00000110, 0b00001100
        };
};

#endif
