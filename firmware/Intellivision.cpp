#include "Intellivision.h"


const unsigned char IntellivisionSpy::buttonMasks[32] = {
            0b01000000, 0b01000001, 0b01100001, 0b01100000, 0b00100000, 0b00100001, 0b00110001, 0b00110000,
            0b00010000, 0b00010001, 0b10010001, 0b10010000, 0b10000000, 0b10000001, 0b11000001, 0b11000000,
            0b00011000, 0b00010100, 0b00010010, 0b00101000, 0b00100100, 0b00100010, 0b01001000, 0b01000100,
            0b01000010, 0b10001000, 0b10000100, 0b10000010, 0b00001010, 0b00001010, 0b00000110, 0b00001100
        };

void IntellivisionSpy::loop() {
    noInterrupts();
    updateState();
    interrupts();
    writeSerial();
}

void IntellivisionSpy::updateState() {
    intRawData = 0x00;

    intRawData |= PIN_READ(INTPIN1) == 0 ? 0x080 : 0x00;
    intRawData |= PIN_READ(INTPIN2) == 0 ? 0x040 : 0x00;
    intRawData |= PIN_READ(INTPIN3) == 0 ? 0x020 : 0x00;
    intRawData |= PIN_READ(INTPIN4) == 0 ? 0x010 : 0x00;
    intRawData |= PIN_READ(INTPIN6) == 0 ? 0x008 : 0x00;
    intRawData |= PINB_READ(INTPIN7) == 0 ? 0x004 : 0x00;
    intRawData |= PINB_READ(INTPIN8) == 0 ? 0x002 : 0x00;
    intRawData |= PINB_READ(INTPIN9) == 0 ? 0x001 : 0x00;

    // Check shoulder buttons first
    byte numPushed = 0;
    for (int i = 28; i < 32; ++i) {
        rawData[i] = (intRawData & buttonMasks[i]) == buttonMasks[i] ? 1 : 0;

        if (i == 29) {
            // Don't bother checking the top button twice.
            // Both will always be true or false.
            continue;
        }

        if (rawData[i]) {
            numPushed++;
        }
    }

    bool keyPadPressed = false;
    byte mask = 0b11110001;
    if (INT_SANE_BEHAVIOR) {
        // This is an interpretted display method that tries to emulate what most
        // games would actually do.

        // If a shoulder button is pressed, keypad is ignored
        // 1 shoulder button and 1 disc direction can be hit at the same time
        // 2 or 3 shoulder buttons at the same time basically negates all pushes
        // keypad pressed ignores disc

        if (numPushed == 0) {
            for (int i = 16; i < 28; ++i) {
                rawData[i] = (intRawData & buttonMasks[i]) == buttonMasks[i] ? 1 : 0;
                if (rawData[i])
                {
                    keyPadPressed = true;
                }
            }
        } else if (numPushed > 1) {
            for (int i = 0; i < 32; ++i) {
                rawData[i] = 0;
            }
        } else {
            for (int i = 16; i < 28; ++i) {
                rawData[i] = 0;
            }
        }
    } else {
        // The raw behavior of multiple buttons pushes is completely bizarre.
        // I am trying to replicate how bizare it is!

        for (int i = 16; i < 28; ++i) {
            rawData[i] = (intRawData & buttonMasks[i]) == buttonMasks[i] ? 1 : 0;
        }

        // Keypad takes precedent, as long as all pushed buttons are in
        // the same column and shoulders are not "active"
        if ((rawData[16] && rawData[19] && rawData[22] && rawData[25])
            || (rawData[17] && rawData[20] && rawData[23] && rawData[26])
            || (rawData[18] && rawData[21] && rawData[24] && rawData[27])) {
            keyPadPressed = numPushed == 0 ? true : false;
        }

        // If shoulders are pushed its a bit check, if shoulders
        // are not active its a full equal check.
        mask = numPushed > 0 ? 0b11110001 : 0xFF;
    }

    if (keyPadPressed) {
        for (int i = 0; i < 16; ++i) {
            rawData[i] = 0;
        }
    } else {
        for (int i = 0; i < 16; ++i) {
            rawData[i] = (intRawData & mask) == buttonMasks[i] ? 1 : 0;
        }
    }
}

void IntellivisionSpy::writeSerial() {
#ifndef DEBUG
    for (unsigned char i = 0; i < 32; ++i) {
        Serial.write(rawData[i]);
    }
    Serial.write(SPLIT);
#else
    Serial.print(intRawData);
    Serial.print("|");
    for(int i = 0; i < 16; ++i) {
        Serial.print(rawData[i]);
    }
    Serial.print("|");
    for(int i = 0; i < 12; ++i) {
        Serial.print(rawData[i + 16]);
    }
    Serial.print("|");
    for(int i = 0; i < 4; ++i) {
        Serial.print(rawData[i + 28]);
    }
    Serial.print("\n");
#endif
}
