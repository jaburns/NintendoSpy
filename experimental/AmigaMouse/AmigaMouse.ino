/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Amiga Mouse Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// -------------------------------------------------------------
//    Mouse distance calculations are based on the video clock
//    Uncomment out PAL or NTSC as appropriate.
// -------------------------------------------------------------
//#define PAL
//#define NTSC

// ---------- Uncomment these for debugging modes --------------
//#define DEBUG

byte buttons[3];
int8_t currentX;
int8_t currentY;
int8_t lastX;
int8_t lastY;
byte lastEncoderX;
byte lastEncoderY;

void setup() 
{
  currentX = lastX = 0;
  currentY = lastY = 0;
  
  // TIMER 1 for interrupt frequency 50 or 60 Hz:
  cli(); // stop interrupts
  TCCR1A = 0; // set entire TCCR1A register to 0
  TCCR1B = 0; // same for TCCR1B
  TCNT1  = 0; // initialize counter value to 0
#ifdef NTSC
  // set compare match register for 60.00060000600006 Hz increments
  OCR1A = 33332; // = 16000000 / (8 * 60.00060000600006) - 1 (must be <65536)
#else // PAL
  // set compare match register for 50 Hz increments
  OCR1A = 39999; // = 16000000 / (8 * 50) - 1 (must be <65536)
#endif
  // turn on CTC mode
  TCCR1B |= (1 << WGM12);
  // Set CS12, CS11 and CS10 bits for 8 prescaler
  TCCR1B |= (0 << CS12) | (1 << CS11) | (0 << CS10);
  // enable timer compare interrupt
  TIMSK1 |= (1 << OCIE1A);
  sei(); // allow interrupts

  Serial.begin( 115200 );
}

ISR(TIMER1_COMPA_vect){
  int8_t x = lastX - currentX;
  int8_t y = lastY - currentY; 

  lastX = currentX;
  lastY = currentY;
  
#ifdef DEBUG
  for(int i = 0; i < 3; ++i)
    Serial.print(buttons[i] == 0 ? "0" : "1");  
  Serial.print('|');
  Serial.print(x);
  Serial.print('|');
  Serial.print(y);
  Serial.print('\n');
#else
  for(int i = 0; i < 3; ++i)
    Serial.write(buttons[i]);
  for(int i = 0; i < 8; ++i)
    Serial.write( (x & (1 << i)) == 0 ? 0 : 1);
  for(int i = 0; i < 8; ++i)
    Serial.write( (y & (1 << i)) == 0 ? 0 : 1);
  Serial.write('\n');
#endif

}

//Horiztonal Increments
//   0
//   8
//  40
//  32

// Vertical Increments
//  0
//  4
// 20
// 16

void loop() {

  noInterrupts();
  byte data = ~PIND;
  interrupts();

  byte xData = (data & 0b00101000);
  byte yData = (data & 0b00010100);

  if ((lastEncoderX == 0 && xData == 8)
       || (lastEncoderX == 8 && xData == 40)
       || (lastEncoderX == 40 && xData == 32)
       || (lastEncoderX == 32 && xData == 0))
    {
        ++currentX;
        lastEncoderX = xData;
    }
  else if ((lastEncoderX == 0 && xData == 32)
       || (lastEncoderX == 32 && xData == 40)
       || (lastEncoderX == 40 && xData == 8)
       || (lastEncoderX == 8 && xData == 0))
   {
      --currentX;
      lastEncoderX = xData;
   }
  if ((lastEncoderY == 0 && yData == 4)
       || (lastEncoderY == 4 && yData == 20)
       || (lastEncoderY == 20 && yData == 16)
       || (lastEncoderY == 16 && yData == 0))
  {
    ++currentY;
    lastEncoderY = yData;

  }
  else if ((lastEncoderY == 0 && yData == 16)
       || (lastEncoderY == 16 && yData == 20)
       || (lastEncoderY == 20 && yData == 4)
       || (lastEncoderY == 4 && yData == 0))
   {   
      --currentY;
      lastEncoderY = yData;
   }

  buttons[0] = (data & 0b10000000) != 0 ? 1 : 0;
  buttons[1] = (data & 0b01000000) != 0 ? 1 : 0;
  buttons[2] = (PINB & 0b00000001) != 0 ? 0 : 1;
  
}
