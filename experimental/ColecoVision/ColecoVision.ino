/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy ColecoVision Super Action Controller Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// Requires this package: https://github.com/NicoHood/PinChangeInterrupt

// ---------- Uncomment these for debugging modes --------------
//#define DEBUG

// PINOUT
// ColecoVision Pin 1 -> Digital Pin 2
// ColecoVision Pin 2 -> Digital Pin 3
// ColecoVision Pin 3 -> Digital Pin 4
// ColecoVision Pin 4 -> Digital Pin 5
// ColecoVision Pin 5 -> Digital Pin 9
// ColecoVision Pin 6 -> Digital Pin 6
// ColecoVision Pin 7 -> Digital Pin 7
// ColecoVision Pin 8 -> Digital Pin 10
// ColecoVision Pin 9 -> Digital Pin 8

#include <PinChangeInterrupt.h>
#include <PinChangeInterruptBoards.h>
#include <PinChangeInterruptPins.h>
#include <PinChangeInterruptSettings.h>

#define PIN_READ( pin )  (PIND&(1<<(pin)))
#define PINB_READ( pin )  (PINB&(1<<(pin)))

#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

#define ZERO  '\0'  // Use a byte value of 0x00 to represent a bit with value 0.
#define ONE    '1'  // Use an ASCII one to represent a bit with value 1.  This makes Arduino debugging easier.
#define SPLIT '\n'  // Use a new-line character to split up the controller state packets.

volatile byte rawData[2];
volatile byte currentState;
volatile byte currentEncoderValue;
volatile byte lastPosition;
volatile byte bit1_isr_cnt = 0;
volatile byte bit2_isr_cnt = 0;

byte lastRawData = 0;

static void pin5bithigh_isr()
{
  asm volatile ("nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n");
  noInterrupts();
  rawData[0] = (PIND & 0b01111100);
  interrupts();
}

static void pin8bithigh_isr()
{  
  asm volatile ("nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n");
  noInterrupts();
  rawData[1] = (PIND & 0b01111100);
  interrupts();
}


static void bitchange_isr()
{
    byte encoderValue = 0;
    encoderValue |= ((PINB & 0b00000001)== 0 ? 0b00000000 : 0b0000001) | ((PIND & 0b10000000) ==  0 ? 0b00000000 : 0b0000010);

    if ((currentEncoderValue == 0x3 && encoderValue == 0x2)
      || currentEncoderValue == 0x2 && encoderValue == 0x0
      || currentEncoderValue == 0x0 && encoderValue == 0x1
      || currentEncoderValue == 0x1 && encoderValue == 0x3)
    {
      if (currentState == 63)
        currentState = 0;
      else
        currentState = (currentState+1)%64;
    }
    else if (currentEncoderValue == 0x2 && encoderValue == 0x3
        || currentEncoderValue == 0x3 && encoderValue == 0x1
        || currentEncoderValue == 0x1 && encoderValue == 0x0
        || currentEncoderValue == 0x0 && encoderValue == 0x2)
    {
      if (currentState == 0)
        currentState = 63;
      else
        currentState = (currentState-1)%64;
    } 
    currentEncoderValue = encoderValue;
  
}

void setup() {

  for (int i = 2; i <= 8; ++i)
    pinMode(i, INPUT_PULLUP);

  currentEncoderValue = 0;
  currentEncoderValue |= ((PINB & 0b00000001)== 0 ? 0b00000000 : 0b0000001) | ((PIND & 0b10000000) ==  0 ? 0b00000000 : 0b0000010);

  currentState = 0;

  Serial.begin( 115200 );

  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(7), bitchange_isr, CHANGE);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(8), bitchange_isr, CHANGE);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(9), pin5bithigh_isr, RISING);
  attachPinChangeInterrupt(digitalPinToPinChangeInterrupt(10), pin8bithigh_isr, RISING);
}

void loop() {

  #ifndef DEBUG
  for(unsigned char i = 0; i < 2; ++i)
  {
    for (unsigned char j = 2; j < 7; ++j)  
    {
      Serial.write ((rawData[i] & (1 << j)) != 0 ? ZERO : ONE );
    }
  }
  Serial.write(currentState + 11);
  Serial.write( SPLIT );
  #else 
  Serial.print((rawData[0] & 0b00000100 ) != 0 ? "0" : "U");
  Serial.print((rawData[0] & 0b00001000 ) != 0 ? "0" : "D");
  Serial.print((rawData[0] & 0b00010000 ) != 0 ? "0" : "L");
  Serial.print((rawData[0] & 0b00100000 ) != 0 ? "0" : "R");
  Serial.print((rawData[0] & 0b01000000 ) != 0 ? "0" : "1");
  Serial.print((rawData[1] & 0b00000100 ) != 0 ? "0" : "A");
  Serial.print((rawData[1] & 0b00001000 ) != 0 ? "0" : "B");
  Serial.print((rawData[1] & 0b00010000 ) != 0 ? "0" : "C");
  Serial.print((rawData[1] & 0b00100000 ) != 0 ? "0" : "D");
  Serial.print((rawData[1] & 0b01000000 ) != 0 ? "0" : "2");
  Serial.print("|");
  Serial.print(currentState);
  Serial.print("\n");
  #endif

  delay(5);
}
