#include "SNES.h"

void SNESSpy::loop() {
    noInterrupts();
    updateState();
    interrupts();
#if !defined(DEBUG)
    writeSerial();
#else
    debugSerial();
#endif
}

void SNESSpy::writeSerial() {
    sendRawData(rawData, 0, bytesToReturn);
}

void SNESSpy::debugSerial() {
    sendRawDataDebug(rawData, 0, bytesToReturn);
}

void SNESSpy::updateState() {
#ifdef MODE_2WIRE_SNES
    read_shiftRegister_2wire(rawData, SNES_LATCH, SNES_DATA, false, SNES_BITCOUNT);
#else
    unsigned char position = 0;
    unsigned char bits = 0;

    bytesToReturn = SNES_BITCOUNT;

    WAIT_FALLING_EDGE(SNES_LATCH);

    do {
        WAIT_FALLING_EDGE(SNES_CLOCK);
        rawData[position++] = !PIN_READ(SNES_DATA);
    }
    while(++bits < SNES_BITCOUNT);

    if (rawData[15] != 0x0)
    {
        bits = 0;
        do {
            WAIT_FALLING_EDGE(SNES_CLOCK);
            rawData[position++] = !PIN_READ(SNES_DATA);
        }
        while(++bits < SNES_BITCOUNT);

        bytesToReturn = SNES_BITCOUNT_EXT;
    }
#endif
}
