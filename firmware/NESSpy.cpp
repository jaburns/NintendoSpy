#include "NESSpy.h"

void NESSpy::setup() {
}

void NESSpy::loop() {
    noInterrupts();
#ifdef MODE_2WIRE_NES
    read_shiftRegister_2wire(NES_LATCH, NES_DATA, true, NES_BITCOUNT);
#else
    updateState();
#endif
    interrupts();
    writeSerial();
}

void NESSpy::writeSerial() {
    sendRawData(rawData, 0, NES_BITCOUNT * 3);
}

void NESSpy::updateState() {
    unsigned char bits = NES_BITCOUNT;
    unsigned char *rawDataPtr = rawData;

    WAIT_FALLING_EDGE(NES_LATCH);

    do {
        WAIT_FALLING_EDGE(NES_CLOCK);
        *rawDataPtr = !PIN_READ(NES_DATA);
        *(rawDataPtr+8) = !PIN_READ(NES_DATA0);
        *(rawDataPtr+16) = !PIN_READ(NES_DATA1);
        ++rawDataPtr;
    }
    while(--bits > 0);
}
