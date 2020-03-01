//
// SMS.cpp
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

#include "SMS.h"

void SMSSpy::setup(uint8_t outputType) {
    this->outputType = outputType;
    setup();
}

void SMSSpy::setup() {
    // Set pins
    // TODO: Move these to config.h
    switch (outputType) {
    case OUTPUT_SMS:
        inputPins[0] = 2;
        inputPins[1] = 3;
        inputPins[2] = 4;
        inputPins[3] = 5;
        inputPins[4] = 7;
        inputPins[5] = 8;
        break;
    case OUTPUT_GENESIS:
        // I don't know why these are different.
        inputPins[0] = 2;
        inputPins[1] = 3;
        inputPins[2] = 4;
        inputPins[3] = 5;
        inputPins[4] = 6;
        inputPins[5] = 7;
        break;
    }

    // Setup input pins
    for (byte i = 0; i < CC_INPUT_PINS; i++)
    {
        pinMode(inputPins[i], INPUT_PULLUP);
    }
}

void SMSSpy::loop() {
    updateState();
    writeSerial();
}

void SMSSpy::updateState() {
    if (max(millis() - lastReadTime, 0U) < CC_READ_DELAY_MS)
    {
        // Not enough time has elapsed, return
        return;
    }

    noInterrupts();

    // Clear current state
    currentState = 0;

    while(inputPins[2] == LOW && inputPins[3] == LOW) {}

    // Read input pins for Up, Down, Left, Right, 1, 2
    if (digitalRead(inputPins[0]) == LOW) { currentState |= CC_BTN_UP; }
    if (digitalRead(inputPins[1]) == LOW) { currentState |= CC_BTN_DOWN; }
    if (digitalRead(inputPins[2]) == LOW) { currentState |= CC_BTN_LEFT; }
    if (digitalRead(inputPins[3]) == LOW) { currentState |= CC_BTN_RIGHT; }
    if (digitalRead(inputPins[4]) == LOW) { currentState |= CC_BTN_1; }
    if (digitalRead(inputPins[5]) == LOW) { currentState |= CC_BTN_2; }

    interrupts();

    lastReadTime = millis();
}

void SMSSpy::writeSerial() {
    switch(outputType) {
    case OUTPUT_SMS:
#ifndef DEBUG
        for (unsigned char i = 0; i < 6; ++i)
        {
            Serial.write (currentState & (1 << i) ? ONE : ZERO );
        }
        Serial.write( SPLIT );
#else
        if (currentState == lastState)
        {
            return;
        }

        Serial.print((currentState & CC_BTN_UP)    ? "U" : "0");
        Serial.print((currentState & CC_BTN_DOWN)  ? "D" : "0");
        Serial.print((currentState & CC_BTN_LEFT)  ? "L" : "0");
        Serial.print((currentState & CC_BTN_RIGHT) ? "R" : "0");
        Serial.print((currentState & CC_BTN_1)     ? "1" : "0");
        Serial.print((currentState & CC_BTN_2)     ? "2" : "0");
        Serial.print("\n");
        lastState = currentState;
#endif
        break;
    case OUTPUT_GENESIS:
#ifndef DEBUG
        Serial.write(ZERO);
        Serial.write((currentState & CC_BTN_UP)    ? 1 : 0);
        Serial.write((currentState & CC_BTN_DOWN)  ? 1 : 0);
        Serial.write((currentState & CC_BTN_LEFT)  ? 1 : 0);
        Serial.write((currentState & CC_BTN_RIGHT) ? 1 : 0);
        Serial.write(ZERO);
        Serial.write(ZERO);
        Serial.write((currentState & CC_BTN_1)     ? 1 : 0);
        Serial.write((currentState & CC_BTN_2)     ? 1 : 0);
        Serial.write(ZERO);
        Serial.write(ZERO);
        Serial.write(ZERO);
        Serial.write(ZERO);
        Serial.write(SPLIT);
#else
        if (currentState == lastState)
        {
            return;
        }

        Serial.print("-");
        Serial.print((currentState & CS_BTN_UP)    ? "U" : "0");
        Serial.print((currentState & CS_BTN_DOWN)  ? "D" : "0");
        Serial.print((currentState & CS_BTN_LEFT)  ? "L" : "0");
        Serial.print((currentState & CS_BTN_RIGHT) ? "R" : "0");
        Serial.print("0");
        Serial.print("0");
        Serial.print((currentState & CS_BTN_1)     ? "1" : "0");
        Serial.print((currentState & CS_BTN_2)     ? "2" : "0");
        Serial.print("0");
        Serial.print("0");
        Serial.print("0");
        Serial.print("0");
        Serial.print("\n");
        lastState = currentState;
#endif
        break;
    }
}
