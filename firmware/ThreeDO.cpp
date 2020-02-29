#include "ThreeDO.h"

void ThreeDOSpy::setup() {
}

void ThreeDOSpy::loop() {
    noInterrupts();
    updateState();
    interrupts();
    writeSerial();
}

void ThreeDOSpy::updateState() {
    unsigned char *rawDataPtr = rawData;

    unsigned char bits = 0;
    WAIT_FALLING_EDGE(ThreeDO_LATCH);

    do {
        WAIT_LEADING_EDGE(ThreeDO_CLOCK);
        *rawDataPtr = PIN_READ(ThreeDO_DATA);

        if (bits == 0 && *rawDataPtr != 0)
            bytesToReturn = bits = 32;
        else if (bits == 0)
            bytesToReturn = bits = 16;

        ++rawDataPtr;
    }
    while(--bits > 0);
}

void ThreeDOSpy::writeSerial() {
    sendRawData(rawData, 0, bytesToReturn);
}
