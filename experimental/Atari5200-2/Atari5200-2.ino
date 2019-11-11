/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Atari 5200 (Y-Axis) Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// ---------- Uncomment for debugging output --------------
//#define DEBUG

// PINOUTS
// Atari Pin 13  -> Digital Pin 2
// Atari Pin 14  -> Digital Pin 3
// Atari Pin 11  -> Analog Pin 0
// Atari Pin 15  -> Arduino GND

#define PIN_READ( pin )  (PIND&(1<<(pin)))
#define PINB_READ( pin )  (PINB&(1<<(pin)))

#define WAIT_FALLING_EDGE( pin ) while( !PIN_READ(pin) ); while( PIN_READ(pin) );
#define WAIT_LEADING_EDGE( pin ) while( PIN_READ(pin) ); while( !PIN_READ(pin) );

#define WAIT_FALLING_EDGEB( pin ) while( !PINB_READ(pin) ); while( PINB_READ(pin) );
#define WAIT_LEADING_EDGEB( pin ) while( PINB_READ(pin) ); while( !PINB_READ(pin) );

#define MICROSECOND_NOPS "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"

volatile int lastVal = 0;
volatile int currentVal = 0;
volatile int analogVal = 0;
volatile int readFlag;

int nominal_min = 1023;
int nominal_max = 0;
int window[3];
int windowPosition = 0;
bool filledWindow = false;

static int ScaleInteger(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
{
  float newValue = ((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin) + newMin;
  if (newValue > newMax)
    return newMax;
  if (newValue < newMin)
    return newMin;

  return newValue;
}

volatile int count = 0;

// Interrupt service routine for the ADC completion
ISR(ADC_vect)
{
  // Must read low first
  analogVal = ADCL | (ADCH << 8);

  if ((analogVal < lastVal && (lastVal - analogVal) > 20 && count > 10) || count > 175)
  {
    currentVal = lastVal;
    lastVal = analogVal;
    readFlag = 1;
    //timeout = count + 10;
    count = 0;
  }
  else
  {
    count++;
    lastVal = analogVal;
  }
 
  // Not needed because free-running mode is enabled.
  // Set ADSC in ADCSRA (0x7A) to start another ADC conversion
  // ADCSRA |= B01000000;
}

void setup() {
  
  for (int i = 2; i <= 11; ++i)
    pinMode(i, INPUT_PULLUP);

  windowPosition = 0;

  // clear ADLAR in ADMUX (0x7C) to right-adjust the result
  // ADCL will contain lower 8 bits, ADCH upper 2 (in last two bits)
  ADMUX &= B11011111;
 
  // Set REFS1..0 in ADMUX (0x7C) to change reference voltage to the
  // proper source (01)
  ADMUX |= B01000000;
 
  // Clear MUX3..0 in ADMUX (0x7C) in preparation for setting the analog
  // input
  ADMUX &= B11110000;
 
  // Set MUX3..0 in ADMUX (0x7C) to read from AD8 (Internal temp)
  // Do not set above 15! You will overrun other parts of ADMUX. A full
  // list of possible inputs is available in Table 24-4 of the ATMega328
  // datasheet
  ADMUX |= 0;
  // ADMUX |= B00001000; // Binary equivalent
 
  // Set ADEN in ADCSRA (0x7A) to enable the ADC.
  // Note, this instruction takes 12 ADC clocks to execute
  ADCSRA |= B10000000;
 
  // Set ADATE in ADCSRA (0x7A) to enable auto-triggering.
  ADCSRA |= B00100000;
 
  // Clear ADTS2..0 in ADCSRB (0x7B) to set trigger mode to free running.
  // This means that as soon as an ADC has finished, the next will be
  // immediately started.
  ADCSRB &= B11111000;
 
  // Set the Prescaler to 128 (16000KHz/128 = 125KHz)
  // Above 200KHz 10-bit results are not reliable.
  ADCSRA |= B00000111;
 
  // Set ADIE in ADCSRA (0x7A) to enable the ADC interrupt.
  // Without this, the internal interrupt will not trigger.
  ADCSRA |= B00001000;
 
  // Enable global interrupts
  // AVR macro included in <avr/interrupts.h>, which the Arduino IDE
  // supplies by default.
  sei();
 
  // Kick off the first ADC
  readFlag = 0;
  // Set ADSC in ADCSRA (0x7A) to start the ADC conversion
  ADCSRA |=B01000000;

  Serial.begin( 115200 );
}

// Function to find the middle of three numbers 
int middleOfThree(int a, int b, int c) 
{ 
    // Compare each three number to find middle  
    // number. Enter only if a > b 
    if (a > b)  
    { 
        if (b > c) 
            return b; 
        else if (a > c) 
            return c; 
        else
            return a; 
    } 
    else 
    { 
        // Decided a is not greater than b. 
        if (a > c) 
            return a; 
        else if (b > c) 
            return c; 
        else
            return b; 
    } 
} 

byte rawData[2];

void loop() {

  if (readFlag == 1)
  {
    rawData[0] = PIN_READ(2);
    rawData[1] = PIN_READ(3);

    window[windowPosition] = currentVal;
    windowPosition += 1;
    windowPosition = (windowPosition % 3);
    if (!filledWindow && windowPosition == 2)
      filledWindow = true;

    int smoothedValue = middleOfThree(window[0], window[1], window[2]);
    if (filledWindow && smoothedValue < nominal_min)
      nominal_min = smoothedValue;
    if (filledWindow && smoothedValue > nominal_max)
      nominal_max = smoothedValue;
      
#ifdef DEBUG  
    Serial.print((rawData[0] == 0) ? "t" : "-");
    Serial.print((rawData[1] == 0) ? "f" : "-");
    Serial.print("|");
    Serial.print(ScaleInteger(smoothedValue, nominal_min, nominal_max, 0, 255));
    Serial.print("|");
    Serial.print(currentVal);
    Serial.print("|");
    Serial.print(smoothedValue);
    Serial.print("|");
    Serial.print(nominal_min);
    Serial.print("|");
    Serial.print(nominal_max);
    Serial.print("\n");
#else
    int sil = ScaleInteger(smoothedValue, nominal_min, nominal_max, 0, 255);
    Serial.write((rawData[0] == 0) ? 0 : 1);
    Serial.write((rawData[1] == 0) ? 0 : 1);
    Serial.write(((sil & 0x0F) << 4));
    Serial.write((sil & 0xF0));
    Serial.write('\n');
#endif
  readFlag = 0;
  delay(5);
}
}
