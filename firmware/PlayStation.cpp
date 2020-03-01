#include "PlayStation.h"

void PlayStationSpy::loop() {
    noInterrupts();
    updateState();
    interrupts();
#if !defined(DEBUG)
    writeSerial();
#else
    debugSerial();
#endif
}

void PlayStationSpy::updateState() {
    byte numBits = 0;
    WAIT_FALLING_EDGE(PS_ATT);

    unsigned char bits = 0;
    do {
       WAIT_LEADING_EDGE(PS_CLOCK);
    }
    while( ++bits < 8 );

    bits = 0;
    do {
        WAIT_LEADING_EDGE(PS_CLOCK);
        byte pins = PIND;

        rawData[numBits] = pins & 0b01000000;
        playstationCommand[numBits] = pins & 0b00100000;
        numBits++;
    }
    while( ++bits < 8 );

    bits = 0;
    do {
        WAIT_LEADING_EDGE(PS_CLOCK);
    }
    while( ++bits < 8 );

    bits = 0;
    do {
        WAIT_LEADING_EDGE(PS_CLOCK);
        rawData[numBits++] = !PIN_READ(PS_DATA);
    }
    while( ++bits < 16 );

    //Read analog sticks for Analog Controller in Red Mode
    if (rawData[0] != 0 && rawData[1] != 0 && rawData[2] == 0 && rawData[3] == 0 && rawData[4] != 0 && rawData[5] != 0 && rawData[6] != 0 && rawData[7] == 0) {
        // controllerType == 0x73 (DualShock 1)
        for(int i = 0; i < 4; ++i)
        {
            bits = 0;
            do {
                WAIT_LEADING_EDGE(PS_CLOCK);
                rawData[numBits++] = PIN_READ(PS_DATA);
            }
            while( ++bits < 8 );
        }
    } else if (rawData[0] == 0 && rawData[1] != 0 && rawData[2] == 0 && rawData[3] == 0 && rawData[4] != 0 && rawData[5] == 0  && rawData[6] == 0 && rawData[7] == 0) {
        // controllerType == 0x12 (mouse)
        for(int i = 0; i < 2; ++i) {
            bits = 0;
            do {
                WAIT_LEADING_EDGE(PS_CLOCK);
                rawData[numBits++] = PIN_READ(PS_DATA);
            }
            while( ++bits < 8 );
        }
    } else if (rawData[0] != 0 && rawData[1] == 0 && rawData[2] == 0 && rawData[3] != 0 && rawData[4] != 0 && rawData[5] != 0  && rawData[6] != 0 && rawData[7] == 0) {
        // controllerType == 0x79 (DualShock 2)
        for(int i = 0; i < 16; ++i) {
            bits = 0;
            do {
                WAIT_LEADING_EDGE(PS_CLOCK);
                rawData[numBits++] = PIN_READ(PS_DATA);
            }
            while( ++bits < 8 );
        }
    }
}

void PlayStationSpy::writeSerial() {
    if (playstationCommand[0] == 0 && playstationCommand[1] != 0 && playstationCommand[2] == 0 && playstationCommand[3] == 0 && playstationCommand[4] == 0 && playstationCommand[5] == 0  && playstationCommand[6] != 0 && playstationCommand[7] == 0) {
        // playstationCommand=0x42 (Controller Poll)
        for (unsigned char i = 0; i < 152; ++i) {
            Serial.write( rawData[i] ? ONE : ZERO );
        }
        Serial.write( SPLIT );
    }
}

void PlayStationSpy::debugSerial() {
    for(int i = 0; i < 152; ++i) {
        if (i % 8 == 0) {
            Serial.print("|");
        }
        Serial.print(rawData[i] ? "1" : "0");
    }
    Serial.print("\n");
}
