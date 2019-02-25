//
// ClassicControllerSpy.h
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

#ifndef ClassicControllerSpy_h
#define ClassicControllerSpy_h

enum
{
    CC_BTN_UP    = 1,
    CC_BTN_DOWN  = 2,
    CC_BTN_LEFT  = 4,
    CC_BTN_RIGHT = 8,
    CC_BTN_1     = 16,
    CC_BTN_2     = 32
};

const byte CC_INPUT_PINS = 6;

const unsigned long CC_READ_DELAY_MS = 5;

class ClassicControllerSpy {
    public:
        ClassicControllerSpy(byte db9_pin_1, byte db9_pin_2, byte db9_pin_3, byte db9_pin_4, byte db9_pin_6, byte db9_pin_9);

        word getState();

    private:
        void readCycle();

        word _currentState;

        unsigned long _lastReadTime;

        byte _inputPins[CC_INPUT_PINS];
};

#endif