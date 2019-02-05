//
// SegaControllerSpy.h
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

#ifndef SegaControllerSpy_h
#define SegaControllerSpy_h

enum
{
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

class SegaControllerSpy {
    public:
        SegaControllerSpy();
        word getState();
        void getMouseState(byte data[3]);

   private:
      unsigned long last6buttonCheck;
      bool sixButtonConnected;    
};

#endif
