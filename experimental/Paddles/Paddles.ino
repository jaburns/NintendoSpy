/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Atari Paddles Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// --- These numbers will likely need modified ---

// **nominal_min** is the minimum value the paddle is capable of hitting.  
// If the display is never hitting all the way left the value needs to be increased.  
// If its hitting left too soon it needs to be decreased.  
// The minimum value that can be selected is 0 and the maximum value should be less than **nominal_left_max/nominal_right_max**. 

// **nominal_max** is the maximum value the paddle is capable of hitting.  
// If the display is never hitting all the way right the value needs to be decreased.  
// If its hitting right too soon it needs to be increased.  
// The maximum value that can be selected is 1023 and the minimum value should be more than **nominal_left_min/nominal_right_min**. 

// **EMA_a** is the number of samples to average in order to smooth the signal.  1.0 is absolutely no smoothing.  
// The larger the number the more smoothing will occur at the cost of latency to the display.

int nominal_min = 213;
int nominal_max = 1004;
float EMA_a = 5;   

// ---------- Uncomment for debugging output --------------
//#define DEBUG

// PINOUTS for Right Paddle
// Atari Pin 4 -> Digital Pin 5
// Atari Pin 5 -> Analog Pin 0
// Atari Pin 8 -> Arduino GND

// PINOUTS for Left Paddle
// Atari Pin 3 -> Digital Pin 5
// Atari Pin 8 -> Arduino GND
// Atari Pin 9 -> Analog Pin 0

#define PIN_READ( pin )  (PIND&(1<<(pin)))

volatile int lastVal = 0;
volatile int currentVal = 0;
volatile int analogVal = 0;
volatile int readFlag;

float EMA_S = 0;          //initialization of EMA_S 

static int ScaleInteger(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
{
  float newValue = ((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin) + newMin;
  if (newValue > newMax)
    return newMax;
  if (newValue < newMin)
    return newMin;

  return newValue;
}

// Interrupt service routine for the ADC completion
ISR(ADC_vect)
{
  // Must read low first
  analogVal = ADCL | (ADCH << 8);

  if (analogVal < lastVal && (lastVal - analogVal) > 20)
  {
    currentVal = lastVal;
    lastVal = analogVal;
    readFlag = 1;
  }
  else
  {
    lastVal = analogVal;
  }
 
  // Not needed because free-running mode is enabled.
  // Set ADSC in ADCSRA (0x7A) to start another ADC conversion
  // ADCSRA |= B01000000;
}

void setup() {
  
  for (int i = 2; i <= 8; ++i)
    pinMode(i, INPUT_PULLUP);

  EMA_S = analogRead(0);

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

void loop() {

  if (readFlag == 1)
  {

	byte pins = 0;
	pins |= (PIND >> 2);
      
	byte fire2 = ((pins & 0b0000000000001000) == 0);
	float mult = 2.0/(EMA_a + 1.0);
	EMA_S = (((float)currentVal) - EMA_S) * mult + EMA_S;      
#ifdef DEBUG
    Serial.print("-");
    Serial.print(fire2 ? "4" : "-");
    Serial.print("|");
    Serial.print(EMA_S);
    Serial.print("|");
    Serial.print(ScaleInteger(EMA_S, nominal_min, nominal_max, 0, 255));
    Serial.print("|");
    Serial.print(0);
    Serial.print("|");
    Serial.print(0);
    Serial.print("\n");
#else
    int sil = ScaleInteger(EMA_S, nominal_min, nominal_max, 0, 255);
    Serial.write(0);
    Serial.write(fire2);
    Serial.write(sil);
    Serial.write(0);
    Serial.write(5);
    Serial.write(11);
    Serial.write('\n');
#endif
	readFlag = 0;
}
}
