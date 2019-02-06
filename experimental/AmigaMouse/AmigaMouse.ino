/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Amiga Mouse Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// ---------- Uncomment these for debugging modes --------------
//#define DEBUG

byte buttons[3];
byte currentX;
byte currentY;
byte lastX;
byte lastY;
byte lastEncoderX;
byte lastEncoderY;

void setup() 
{
  currentX = lastX = 0;
  currentY = lastY = 0;

  // ~16ms timer interrupt
  TIMSK2 = (TIMSK2 & B11111110) | 0x01;
  TCCR2B = (TCCR2B & B11111000) | 0x07;

  Serial.begin( 115200 );
}

ISR(TIMER2_OVF_vect){
  int8_t x = lastX - currentX;
  int8_t y = lastY - currentY; 

  if (x > 127)
    x = (255 - x) * -1;
  if (y > 127)
    y = (255 - y) * -1;

  if (x < -127)
    x = (255 + x);
  if (y < -127)
    y = (255 + y);
    
  lastX = currentX;
  lastY = currentY;
  
#ifdef DEBUG
  for(int i = 0; i < 3; ++i)
    Serial.print(buttons[i] == 0 ? "0" : "1");  
  Serial.print('|');
  Serial.print(x);
  Serial.print('|');
  Serial.println(y);
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
