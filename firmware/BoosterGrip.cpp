//
// BoosterGrip.cpp
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

#include "BoosterGrip.h"

void BoosterGripSpy::setup() {
    // TODO: Move these pin numbers to config.h
    // Set pins
    inputPins[0] = 2;
    inputPins[1] = 3;
    inputPins[2] = 4;
    inputPins[3] = 5;
    inputPins[4] = 6;
    inputPins[5] = 7;
    inputPins[6] = 8;

    // Setup input pins
    for (byte i = 0; i < BG_INPUT_PINS; i++) {
        if (i == 4 || i == 6) {
            pinMode(inputPins[i], INPUT);
        } else {
            pinMode(inputPins[i], INPUT_PULLUP);
        }
    }

    lastReadTime = millis();
}

void BoosterGripSpy::loop() {
    if (max(millis() - lastReadTime, 0) < BG_READ_DELAY_MS)
    {
        // Not enough time has elapsed, return
        return;
    }

    noInterrupts();
    updateState();
    interrupts();
#if !defined(DEBUG)
    writeSerial();
#else
    debugSerial();
#endif

    lastReadTime = millis();
}

void BoosterGripSpy::updateState() {
    // Read input pins for Up, Down, Left, Right, 1, 2, 3
    if (digitalRead(inputPins[0]) == LOW) { currentState |= BG_BTN_UP; }
    if (digitalRead(inputPins[1]) == LOW) { currentState |= BG_BTN_DOWN; }
    if (digitalRead(inputPins[2]) == LOW) { currentState |= BG_BTN_LEFT; }
    if (digitalRead(inputPins[3]) == LOW) { currentState |= BG_BTN_RIGHT; }
    if (digitalRead(inputPins[4]) == HIGH) { currentState |= BG_BTN_3; }
    if (digitalRead(inputPins[5]) == LOW)  { currentState |= BG_BTN_1; }
    if (digitalRead(inputPins[6]) == HIGH) { currentState |= BG_BTN_2; }
}

void BoosterGripSpy::writeSerial() {
    for (unsigned char i = 0; i < 7; ++i) {
        Serial.write (currentState & (1 << i) ? ONE : ZERO );
    }
    Serial.write( SPLIT );
}

void BoosterGripSpy::debugSerial() {
    if (currentState != lastState) {
        Serial.print((currentState & BG_BTN_UP)    ? "U" : "0");
        Serial.print((currentState & BG_BTN_DOWN)  ? "D" : "0");
        Serial.print((currentState & BG_BTN_LEFT)  ? "L" : "0");
        Serial.print((currentState & BG_BTN_RIGHT) ? "R" : "0");
        Serial.print((currentState & BG_BTN_1)     ? "1" : "0");
        Serial.print((currentState & BG_BTN_2)     ? "2" : "0");
        Serial.print((currentState & BG_BTN_3)     ? "3" : "0");
        Serial.print("\n");
        lastState = currentState;
    }
}
