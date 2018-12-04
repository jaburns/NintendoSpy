/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Atari Keyboard Controller Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// Requires this package: https://github.com/NicoHood/PinChangeInterrupt

// ---------- Uncomment this for debugging mode --------------
//#define DEBUG

// PINOUT
// Atari Pin 1 -> Digital Pin 2
// Atari Pin 2 -> Digital Pin 3
// Atari Pin 3 -> Digital Pin 4
// Atari Pin 4 -> Digital Pin 5
// Atari Pin 5 -> Digital Pin 6
// Atari Pin 6 -> Digital Pin 7
// Atari Pin 9 -> Digital Pin 8

#include <PinChangeInterrupt.h>
#include <PinChangeInterruptBoards.h>
#include <PinChangeInterruptPins.h>
#include <PinChangeInterruptSettings.h>

#define PIN_READ( pin )  (PIND&(1<<(pin)))

volatile byte currentState[2];
volatile byte currentEncoderValue;
volatile byte lastPosition;

byte lastRawData = 0;

void bit1_isr()
{
  byte rawData = 0;
  rawData |= (PIND >> 2);

  byte encoderValue = 0;
  encoderValue |= ((PIN_READ(2) & 0b00000100) >> 1) | ((PIN_READ(4) & 0b00010000) >> 4);

  if ((encoderValue == 0x0 && currentEncoderValue == 0x1)
      || (encoderValue == 0x1 && currentEncoderValue == 0x3)
      || (encoderValue == 0x3 && currentEncoderValue == 0x2)
      || (encoderValue == 0x2 && currentEncoderValue == 0x0))
    lastPosition = (lastPosition + 1) % 16;
  else if ((encoderValue == 0x0 && currentEncoderValue == 0x2)
      || (encoderValue == 0x2 && currentEncoderValue == 0x3)
      || (encoderValue == 0x3 && currentEncoderValue == 0x1)
      || (encoderValue == 0x1 && currentEncoderValue == 0x0))
    lastPosition = (lastPosition - 1) % 16;

  currentEncoderValue = encoderValue;
  currentState[1] = (byte)lastPosition;
  currentState[0] = (byte)((rawData & 0b0100000) == 0);

}

void bit2_isr()
{
  byte rawData = 0;
  rawData |= (PIND >> 2);

  byte encoderValue = 0;
  encoderValue |= ((PIN_READ(2) & 0b00000100) >> 1) | ((PIN_READ(4) & 0b00010000) >> 4);

  if ((encoderValue == 0x0 && currentEncoderValue == 0x1)
      || (encoderValue == 0x1 && currentEncoderValue == 0x3)
      || (encoderValue == 0x3 && currentEncoderValue == 0x2)
      || (encoderValue == 0x2 && currentEncoderValue == 0x0))
    lastPosition = (lastPosition - 1) % 16;
  else if ((encoderValue == 0x0 && currentEncoderValue == 0x2)
      || (encoderValue == 0x2 && currentEncoderValue == 0x3)
      || (encoderValue == 0x3 && currentEncoderValue == 0x1)
      || (encoderValue == 0x1 && currentEncoderValue == 0x0))
    lastPosition = (lastPosition + 1) % 16;

  currentEncoderValue = encoderValue;
  currentState[1] = (byte)lastPosition;
  currentState[0] = (byte)((rawData & 0b0100000) == 0);
}

void setup() {

  for (int i = 2; i <= 8; ++i)
    pinMode(i, INPUT_PULLUP);

  currentEncoderValue = 0;
  currentEncoderValue |= ((PIN_READ(2) & 0b00000100) >> 1);
  currentEncoderValue |= ((PIN_READ(4) & 0b00010000) >> 4);

  currentState[0] = 0;
  currentState[1] = 0;

  Serial.begin( 115200 );

  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(3), bit1_isr, CHANGE);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(5), bit2_isr, CHANGE);
}


byte rawData;

void loop() {
  // put your main code here, to run repeatedly:
  
#ifdef DEBUG
  noInterrupts();
  rawData = 0;
  rawData |= (PIND >> 2);
  interrupts();
#endif

#ifdef DEBUG
  if (rawData != lastRawData)
  {
    Serial.print((rawData & 0b0000000000000010) != 0 ? "-" : "2");
    Serial.print((rawData & 0b0000000000001000) != 0 ? "-" : "4");
    Serial.print((rawData & 0b0000000000100000) != 0 ? "-" : "6");
    Serial.print("\n");
    lastRawData = rawData;
  }
#else
  Serial.print(currentState[0]);
  Serial.print(currentState[1]);
  Serial.print("\n");
#endif
}
