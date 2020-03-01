//
// Genesis.h
//
// Author:
//       Christopher Mallery <christopher.mallery@gmail.com>
//
// Copyright (c) 2018 Christopher Mallery <christopher.mallery@gmail.com>
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#ifndef GenesisSpy_h
#define GenesisSpy_h

#include "ControllerSpy.h"

#define SHIFT_A_AND_START (TWOC_MASK_A_AND_START_CTRL | (PIND << 1))
#define SHIFT_UDLRBC (TWOC_MASK_UPLRBC | (PIND >> 1))
#define SHIFT_ZYXM (TWOC_MASK_XYXM | (PIND << 7))

#define STATE_TWO (PINB & 1) == 0 && (PIND & MASK_PINS_FOUR_AND_FIVE) == 0 && (PIND & MASK_PINS_TWO_AND_THREE) != 0
#define WAIT_FOR_STATE_TWO (PINB & 1) != 0 || (PIND & MASK_PINS_FOUR_AND_FIVE) != 0 || (PIND & MASK_PINS_TWO_AND_THREE) == 0
#define STATE_THREE (PINB & 1) == 1 && (PIND & MASK_PINS_FOUR_AND_FIVE) != 0
#define WAIT_FOR_STATE_THREE (PINB & 1) != 1 || (PIND & MASK_PINS_FOUR_AND_FIVE) == 0
#define STATE_FOUR_OR_SIX (PINB & 1) == 0 && (PIND & MASK_PINS_FOUR_AND_FIVE) == 0
#define WAIT_FOR_STATE_FOUR_OR_SIX (PINB & 1) != 0 || (PIND & MASK_PINS_FOUR_AND_FIVE) != 0
#define STATE_SIX (PINB & 1) == 0 && (PIND & MASK_PINS_TWO_THREE_FOUR_FIVE) == 0
#define NOT_STATE_SIX (PIND & MASK_PINS_TWO_THREE_FOUR_FIVE) != 0
#define STATE_SEVEN (PINB & 1) == 1 && (PIND & MASK_PINS_TWO_THREE_FOUR_FIVE) != 0
#define WAIT_FOR_STATE_SEVEN (PINB & 1) != 1 || (PIND & MASK_PINS_TWO_THREE_FOUR_FIVE) == 0

class GenesisSpy : public ControllerSpy {
    public:
        void setup();
        void loop();
        void writeSerial();
        void debugSerial();
        void updateState();

    private:
        enum buttonTypes {
            SCS_CTL_ON    = 1, // The controller is connected
            SCS_BTN_UP    = 2,
            SCS_BTN_DOWN  = 4,
            SCS_BTN_LEFT  = 8,
            SCS_BTN_RIGHT = 16,
            SCS_BTN_B     = 32,
            SCS_BTN_C     = 64,
            SCS_BTN_A     = 128,
            SCS_BTN_START = 256,
            SCS_BTN_Z     = 512,
            SCS_BTN_Y     = 1024,
            SCS_BTN_X     = 2048,
            SCS_BTN_MODE  = 4096
	};

        static const uint16_t MASK_PINS_FOUR_AND_FIVE       = 0x0030 // 0b0000000000110000
        static const uint16_t MASK_PINS_TWO_AND_THREE       = 0x000C // 0b0000000000001100
        static const uint16_t MASK_PINS_TWO_THREE_FOUR_FIVE = 0x003C // 0b0000000000111100

        static const uint16_t TWOC_MASK_A_AND_START_CTRL    = 0xFE7E // 0b1111111001111110
        static const uint16_t TWOC_MASK_UPLRBC              = 0xFF81 // 0b1111111110000001
        static const uint16_t TWOC_MASK_XYXM                = 0xE1FF // 0b1110000111111111

        unsigned long last6buttonCheck;
        bool sixButtonConnected;

	word currentState = 0;
	word lastState = -1;
};

#endif
