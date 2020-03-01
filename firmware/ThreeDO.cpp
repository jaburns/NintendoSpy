#include "ThreeDO.h"

void ThreeDOSpy::loop() {
    noInterrupts();
    updateState();
    interrupts();
#if !defined(DEBUG)
    writeSerial();
#else
    debugSerial();
#endif
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

void ThreeDOSpy::debugSerial() {
    sendRawDataDebug(rawData, 0, bytesToReturn);
}
