/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Atari Driving Controller Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// Requires this package: https://github.com/NicoHood/PinChangeInterrupt

// ---------- Uncomment these for debugging modes --------------
//#define DEBUG
//#define PRETTY_PRINT

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
volatile byte bit1_isr_cnt = 0;
volatile byte bit2_isr_cnt = 0;

byte lastRawData = 0;

static void bitchange_isr()
{
  byte encoderValue = 0;
  encoderValue |= ((PIND & 0b00001100)) >> 2;

  if ((currentEncoderValue == 0x3 && encoderValue == 0x2)
      || currentEncoderValue == 0x2 && encoderValue == 0x0
      || currentEncoderValue == 0x0 && encoderValue == 0x1
      || currentEncoderValue == 0x1 && encoderValue == 0x3)
    {
      if (currentState[1] == 0)
        currentState[1] = 15;
      else
        currentState[1] = (currentState[1]-1)%16;
    }
  else if (currentEncoderValue == 0x2 && encoderValue == 0x3
      || currentEncoderValue == 0x3 && encoderValue == 0x1
      || currentEncoderValue == 0x1 && encoderValue == 0x0
      || currentEncoderValue == 0x0 && encoderValue == 0x2)
  {
    if (currentState[1] == 15)
      currentState[1] = 0;
    else
      currentState[1] = (currentState[1]+1)%16;
  } 
  currentEncoderValue = encoderValue;
  
}

void setup() {

  for (int i = 2; i <= 8; ++i)
    pinMode(i, INPUT_PULLUP);

  currentEncoderValue = 0;
  currentEncoderValue |= ((PIND & 0b01001100)) >> 2;

  currentState[0] = 0;
  currentState[1] = 0;

  Serial.begin( 115200 );

#ifndef DEBUG
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(2), bitchange_isr, CHANGE);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(3), bitchange_isr, CHANGE);
#endif
}

void loop() {

  noInterrupts();
  byte rawData = 0;
  rawData |= (PIND >> 2);
  interrupts();

#ifndef DEBUG
  currentState[0] = (byte)((rawData & 0b00100000) == 0);
#endif

#ifdef DEBUG
  if (rawData != lastRawData)
  {
    Serial.print((rawData & 0b00000001) != 0 ? "-" : "1");
    Serial.print((rawData & 0b00000010) != 0 ? "-" : "1");
    Serial.print((rawData & 0b00100000) != 0 ? "-" : "6");
    Serial.print("\n");
    lastRawData = rawData;
  }
#else
#ifdef PRETTY_PRINT
  Serial.print(currentState[0]);  
  Serial.print("|");
  Serial.print(currentState[1]);
  Serial.print("|");
  Serial.print((rawData & 0b00000001) != 0 ? "-" : "1");
  Serial.print((rawData & 0b00000010) != 0 ? "-" : "1");
  Serial.print("|");
  Serial.print(currentEncoderValue);
  Serial.print("\n");
#else
  Serial.write(currentState[0]); 
  Serial.write(currentState[1]); 
  Serial.write('\n'); 
#endif
#endif
}
