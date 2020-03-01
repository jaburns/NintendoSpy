#include "Jaguar.h"

void JaguarSpy::loop() {
    noInterrupts();
    updateState();
    interrupts();
#if !defined(DEBUG)
    writeSerial();
#else
    debugSerial();
#endif
    delay(1);
}

void JaguarSpy::updateState() {
    WAIT_FALLING_EDGEB(0);
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
    rawData[3] = (PIND & 0b11111000);

    WAIT_FALLING_EDGEB(1);
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
    rawData[2] = (PIND & 0b11111000);

    WAIT_FALLING_EDGEB(2);
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
    rawData[1] = (PIND & 0b11111000);

    WAIT_FALLING_EDGEB(3);
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS MICROSECOND_NOPS);
    rawData[0] = (PIND & 0b11111100);
}

void JaguarSpy::writeSerial() {
    Serial.write(rawData[0]);
    Serial.write(rawData[1]);
    Serial.write(rawData[2]);
    Serial.write(rawData[3]);
    Serial.write(SPLIT);
}

void JaguarSpy::debugSerial() {
    Serial.print((rawData[0] & 0b00000100) == 0 ? "P" : "0");
    Serial.print((rawData[0] & 0b00001000) == 0 ? "A" : "0");
    Serial.print((rawData[0] & 0b00010000) == 0 ? "R" : "0");
    Serial.print((rawData[0] & 0b00100000) == 0 ? "L" : "0");
    Serial.print((rawData[0] & 0b01000000) == 0 ? "D" : "0");
    Serial.print((rawData[0] & 0b10000000) == 0 ? "U" : "0");

    //Serial.print((rawData[1] & 0b00000100) == 0 ? "P" : "0");
    Serial.print((rawData[1] & 0b00001000) == 0 ? "B" : "0");
    Serial.print((rawData[1] & 0b00010000) == 0 ? "1" : "0");
    Serial.print((rawData[1] & 0b00100000) == 0 ? "4" : "0");
    Serial.print((rawData[1] & 0b01000000) == 0 ? "7" : "0");
    Serial.print((rawData[1] & 0b10000000) == 0 ? "*" : "0");

    //Serial.print((rawData[2] & 0b00000100) == 0 ? "P" : "0");
    Serial.print((rawData[2] & 0b00001000) == 0 ? "C" : "0");
    Serial.print((rawData[2] & 0b00010000) == 0 ? "2" : "0");
    Serial.print((rawData[2] & 0b00100000) == 0 ? "5" : "0");
    Serial.print((rawData[2] & 0b01000000) == 0 ? "8" : "0");
    Serial.print((rawData[2] & 0b10000000) == 0 ? "Z" : "0");

    //Serial.print((rawData[3] & 0b00000100) == 0 ? "P" : "0");
    Serial.print((rawData[3] & 0b00001000) == 0 ? "O" : "0");
    Serial.print((rawData[3] & 0b00010000) == 0 ? "3" : "0");
    Serial.print((rawData[3] & 0b00100000) == 0 ? "6" : "0");
    Serial.print((rawData[3] & 0b01000000) == 0 ? "9" : "0");
    Serial.print((rawData[3] & 0b10000000) == 0 ? "#" : "0");

    Serial.print("\n");
}
