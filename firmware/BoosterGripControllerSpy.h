//
// BoosterGripControllerSpy.h
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

#ifndef BoosterGripControllerSpy_h
#define BoosterGripControllerSpy_h

enum
{
    BG_BTN_UP    = 1,
    BG_BTN_DOWN  = 2,
    BG_BTN_LEFT  = 4,
    BG_BTN_RIGHT = 8,
    BG_BTN_1     = 16,
    BG_BTN_2     = 32,
	BG_BTN_3	 = 64
};

const byte BG_INPUT_PINS = 7;

const unsigned long BG_READ_DELAY_MS = 5;

class BoosterGripControllerSpy {
    public:
        BoosterGripControllerSpy(byte db9_pin_1, byte db9_pin_2, byte db9_pin_3, byte db9_pin_4, byte db9_pin_5, byte db9_pin_6, byte db9_pin_9);

        word getState();

    private:
        void readCycle();

        word _currentState;

        unsigned long _lastReadTime;

        byte _inputPins[BG_INPUT_PINS];
};

#endif
