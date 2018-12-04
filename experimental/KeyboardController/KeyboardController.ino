/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Atari Keyboard Controller Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// Requires this package: https://github.com/NicoHood/PinChangeInterrupt

// ---------- Uncomment one of these options to select operation mode --------------
//#define DEBUG
//#define BIG_BIRD
//#define STAR_RAIDERS

// PINOUTS
// For every other game, besides Star Raiders and Big Bird's Egg Catch the pinout is straightforward.  This is the same as the standard Atari controller pinout.
// Atari Pin 1 -> Digital Pin 2
// Atari Pin 2 -> Digital Pin 3
// Atari Pin 3 -> Digital Pin 4
// Atari Pin 4 -> Digital Pin 5
// Atari Pin 5 -> Digital Pin 6
// Atari Pin 6 -> Digital Pin 7
// Atari Pin 9 -> Digital Pin 8
//
// For Star Raiders and Big Bird's Egg Catch the pinout involves the analog pins.
// Atari Pin 1 -> Digital Pin 2
// Atari Pin 2 -> Digital Pin 3
// Atari Pin 3 -> Digital Pin 4
// Atari Pin 4 -> Digital Pin 5
// Atari Pin 5 -> Analog Pin 0
// Atari Pin 6 -> Digital Pin 7
// Atari Pin 9 -> Analog Pin 1

#include <PinChangeInterrupt.h>
#include <PinChangeInterruptBoards.h>
#include <PinChangeInterruptPins.h>
#include <PinChangeInterruptSettings.h>

#define PIN_READ( pin )  (PIND&(1<<(pin)))
#define PIN_READB( pin )  (PINB&(1<<(pin)))

// The below values are not scientific, but they seem to work.  These may need to be tuned for different systems.
#define LINE_WAIT 200
#define DIGITAL_HIGH_THRESHOLD 50

volatile byte currentState = 0;
byte lastState = 0xFF;
byte lastRawData = 0;

#ifndef BIG_BIRD
#ifndef STAR_RAIDERS
void row1_isr()
{
  delayMicroseconds(LINE_WAIT);
  byte cachedCurrentState = currentState;
  if (currentState > 3)
    return;
  else if (PIN_READ(7) == 0)
    currentState = 3;
  else if (PIN_READB(0) == 0)
    currentState = 2;
  else if (PIN_READ(6) == 0)
    currentState = 1;
  else if (cachedCurrentState >= 1 && cachedCurrentState <= 3)
    currentState = 0;
}

void row2_isr()
{
  delayMicroseconds(LINE_WAIT);
  byte cachedCurrentState = currentState;
  if (currentState > 6)
    return;
  else if (PIN_READ(7) == 0)
    currentState = 6;
  else if (PIN_READB(0) == 0)
    currentState = 5;
  else if (PIN_READ(6) == 0)
    currentState = 4;
  else if (cachedCurrentState >= 4 && cachedCurrentState <= 6)
    currentState = 0;
}

void row3_isr()
{
  delayMicroseconds(LINE_WAIT);
  byte cachedCurrentState = currentState;
  if (currentState > 9)
    return;
  else if (PIN_READ(7) == 0)
    currentState = 9;
  else if (PIN_READB(0) == 0)
    currentState = 8;
  else if (PIN_READ(6) == 0)
    currentState = 7;
  else if (cachedCurrentState >= 7 && cachedCurrentState <= 9)
    currentState = 0;
}

void row4_isr()
{
  delayMicroseconds(LINE_WAIT);
  byte cachedCurrentState = currentState;
  if (PIN_READ(7) == 0)
    currentState = 12;
  else if (PIN_READB(0) == 0)
    currentState = 11;
  else if (PIN_READ(6) == 0)
    currentState = 10;
  else if (cachedCurrentState >= 10)
    currentState = 0;
}
#endif
#endif

#ifdef STAR_RAIDERS
void row1sr_isr()
{
  delayMicroseconds(LINE_WAIT);
  byte cachedCurrentState = currentState;
  if (currentState > 3)
    return;
  else if (PIN_READ(7) == 0)
    currentState = 3;
  else if (analogRead(1) < DIGITAL_HIGH_THRESHOLD)
    currentState = 2;
  else if (analogRead(0) < DIGITAL_HIGH_THRESHOLD)
    currentState = 1;
  else if (cachedCurrentState >= 1 && cachedCurrentState <= 3)
    currentState = 0;
}

void row2sr_isr()
{
  delayMicroseconds(LINE_WAIT);
  byte cachedCurrentState = currentState;
  if (currentState > 6)
    return;
  else if (PIN_READ(7) == 0)
    currentState = 6;
  else if (analogRead(1) < 50)
    currentState = 5;
  else if (analogRead(0) < 50) 
    currentState = 4;
  else if (cachedCurrentState >= 4 && cachedCurrentState <= 6)
    currentState = 0;
}
#endif

void setup() {

  currentState = 0;
  lastState = 0xFF;
  for (int i = 2; i <= 8; ++i)
    pinMode(i, INPUT_PULLUP);

  Serial.begin( 115200 );
#ifndef DEBUG
#ifndef BIG_BIRD
#ifndef STAR_RAIDERS
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(2), row1_isr, FALLING);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(3), row2_isr, FALLING);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(4), row3_isr, FALLING);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(5), row4_isr, FALLING);
#else
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(2), row1sr_isr, FALLING);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(3), row2sr_isr, FALLING);
#endif
#endif
#endif
}


byte rawData;

void loop() {
  // put your main code here, to run repeatedly:
  
#ifdef DEBUG
  noInterrupts();
  rawData = 0;
  rawData |= (PIND >> 2) | (PINB << 6);
  int analog0 =  analogRead(0);
  int analog1 =  analogRead(1);
  interrupts();
#elif defined BIG_BIRD
  noInterrupts();
  byte pin6 = PIN_READ(7);
  int pin9 =  analogRead(1);
  interrupts();
  if ((pin6 & 0b10000000) == 0)
      currentState = 6;
  else if (pin9 < DIGITAL_HIGH_THRESHOLD)
      currentState = 5;
  else
    currentState = 0;
#endif

#ifdef DEBUG
  if (rawData != lastRawData)
  {
    Serial.print((rawData & 0b0000000000000001) != 0 ? "-" : "1");
    Serial.print((rawData & 0b0000000000000010) != 0 ? "-" : "2");
    Serial.print((rawData & 0b0000000000000100) != 0 ? "-" : "3");
    Serial.print((rawData & 0b0000000000001000) != 0 ? "-" : "4");
    Serial.print((rawData & 0b0000000000010000) != 0 ? "-" : "5");
    Serial.print((rawData & 0b0000000000100000) != 0 ? "-" : "6");
    Serial.print((rawData & 0b0000000001000000) != 0 ? "-" : "7");
    Serial.print("|");
    Serial.print(analog0);
    Serial.print("|");
    Serial.print(analog1);
    Serial.print("\n");
    lastRawData = rawData;
  }
#else
  if (currentState != lastState)
  {
    Serial.print(currentState);
    Serial.print("\n");
    lastState = currentState;
  }
#endif
}
