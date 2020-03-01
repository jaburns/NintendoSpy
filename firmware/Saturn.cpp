#include "Saturn.h"

void SaturnSpy::loop() {
    noInterrupts();
    updateState();
    interrupts();
    writeSerial();
}

void SaturnSpy::updateState() {
    word pincache = 0;

    while((PIND & 0b11000000) != 0b10000000) {}
    asm volatile( MICROSECOND_NOPS );
    pincache |= PIND;
    if ((pincache & 0b11000000) == 0b10000000) {
        ssState3 = ~pincache;
    }

    pincache = 0;
    while((PIND & 0b11000000) != 0b01000000) {}
    asm volatile( MICROSECOND_NOPS );
    pincache |= PIND;
    if ((pincache & 0b11000000) == 0b01000000) {
        ssState2 = ~pincache;
    }

    pincache = 0;
    while((PIND & 0b11000000) != 0){}
    asm volatile( MICROSECOND_NOPS );
    pincache |= PIND;
    if ((pincache & 0b11000000) == 0) {
        ssState1 = ~pincache;
    }

    pincache = 0;
    while((PIND & 0b11000000) != 0b11000000) {}
    asm volatile( MICROSECOND_NOPS );
    pincache |= PIND;
    if ((pincache & 0b11000000) == 0b11000000) {
        ssState4 = ~pincache;
    }
}

void SaturnSpy::writeSerial() {
#ifndef DEBUG
    for(int i = 0; i < 8; ++i) {
        Serial.write(i == 6 ? ONE : ZERO);
    }
    Serial.write((ssState3 & 0b00100000) ? ZERO : ONE);
    Serial.write((ssState3 & 0b00010000) ? ZERO : ONE);
    Serial.write((ssState3 & 0b00001000) ? ZERO : ONE);
    Serial.write((ssState3 & 0b00000100) ? ZERO : ONE);

    Serial.write((ssState2 & 0b00100000) ? ZERO : ONE);
    Serial.write((ssState2 & 0b00010000) ? ZERO : ONE);
    Serial.write((ssState2 & 0b00001000) ? ZERO : ONE);
    Serial.write((ssState2 & 0b00000100) ? ZERO : ONE);

    Serial.write ((ssState1 & 0b00100000) ? ZERO : ONE );
    Serial.write ((ssState1 & 0b00010000) ? ZERO : ONE );
    Serial.write ((ssState1 & 0b00001000) ? ZERO : ONE );
    Serial.write ((ssState1 & 0b00000100)  ? ZERO : ONE );

    Serial.write ((ssState4 & 0b00100000) ? ZERO : ONE );
    Serial.write (ONE);
    Serial.write (ONE);
    Serial.write (ONE);

    for(int i = 0; i < 32; ++i) {
        Serial.write(ZERO);
    }

    Serial.write( SPLIT );
#else
    Serial.print((ssState1 & 0b00000100)    ? "Z" : "0");
    Serial.print((ssState1 & 0b00001000)    ? "Y" : "0");
    Serial.print((ssState1 & 0b00010000)    ? "X" : "0");
    Serial.print((ssState1 & 0b00100000)    ? "R" : "0");

    Serial.print((ssState2 & 0b00000100)    ? "B" : "0");
    Serial.print((ssState2 & 0b00001000)    ? "C" : "0");
    Serial.print((ssState2 & 0b00010000)    ? "A" : "0");
    Serial.print((ssState2 & 0b00100000)    ? "S" : "0");

    Serial.print((ssState3 & 0b00000100)    ? "u" : "0");
    Serial.print((ssState3 & 0b00001000)    ? "d" : "0");
    Serial.print((ssState3 & 0b00010000)    ? "l" : "0");
    Serial.print((ssState3 & 0b00100000)    ? "r" : "0");

    Serial.print((ssState4 & 0b00100000)    ? "L" : "0");

    Serial.print("\n");
#endif
}
