/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy CD-i Controller Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// The board that I bought to "splice" the controller cable has PIN 2 marked as PIN 8. 
// The only thing that needs to connected is Controller Pin 2 to Arduino Digital Pin 10.
// Obviously this took me while as my board was marked wrong.

#include <SoftwareSerial.h>
SoftwareSerial vSerial(9, 10, true);

// Uncomment for 2 levels of debugging output
//#define PRETTY_PRINT
//#define DEBUG

// These values may need adjusting based on the controller used.
#define TIMEOUT 50
#define NORMALIZE_ANALOG true
#define MAX_LEFT  136
#define MAX_RIGHT 8
#define MAX_UP    136
#define MAX_DOWN  8

// You can normalize the analog values with the IR remote, which is way faster than a controller if desired.
// These values are based on my testing, but may be different for different remotes/controllers.
#define NORMALIZE_ANALOG_WITH_REMOTE false
#define MAX_LEFT_REMOTE   173
#define MAX_RIGHT_REMOTE   45
#define MAX_UP_REMOTE     173
#define MAX_DOWN_REMOTE    45

byte rawData[6];
byte yAxis;
byte xAxis;
unsigned long timeout;

void setup() {
  vSerial.begin(1200); 
  Serial.begin( 115200 );
  rawData[4] = rawData[5] = 0;
}

void printRawData()
{
#ifndef PRETTY_PRINT
  for(int i = 0; i < 6; ++i)
    Serial.write(rawData[i]);
  Serial.write("\n");
#else
  for(int i = 0; i < 6; ++i)
  {
    Serial.print(rawData[i]);
    Serial.print("|"); 
  }

  Serial.print(yAxis > 128 ? 256-yAxis+128 : yAxis);
  Serial.print("|"); 
  Serial.print(xAxis > 128 ? 256-xAxis+128 : xAxis);
  Serial.print("\n");
#endif
}

static int ScaleInteger(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
{
  float newValue = ((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin) + newMin;
  if (newValue > newMax)
    return newMax;
  if (newValue < newMin)
    return newMin;

  return newValue;
}

void loop() {
  
  // put your main code here, to run repeatedly:
  if (vSerial.available() >= 3) {

      char c = vSerial.read();
      if ((c & 0b11000000) != 0b11000000)
        return;
   
#ifndef DEBUG     

      rawData[4] = (c & 0b00100000) != 0 ? 1 : 0;  // Button 1
      rawData[5] = (c & 0b00010000) != 0 ? 1 : 0;  // Button 2

       yAxis = ((c & 0b00001100) << 4);
       xAxis = ((c & 0b00000011) << 6);

      c = vSerial.read();
      xAxis = xAxis + (byte)(c & 0b00111111);

      c = vSerial.read();
      yAxis = yAxis + (byte)(c & 0b00111111);

      if (yAxis == 0 || yAxis == 128)
      {
        rawData[0] = 0;
        rawData[1] = 0;
      }
      else if (yAxis > 128)
      {
        rawData[0] =  ScaleInteger(256-yAxis+128, 128,  NORMALIZE_ANALOG_WITH_REMOTE ? MAX_UP_REMOTE : NORMALIZE_ANALOG ? MAX_UP : 255, 0, 255);
        rawData[1] = 0;
      }
      else
      {
        rawData[1] =  ScaleInteger(yAxis, 0,  NORMALIZE_ANALOG_WITH_REMOTE ? MAX_DOWN_REMOTE : NORMALIZE_ANALOG ? MAX_DOWN : 127, 0, 255);
        rawData[0] = 0;
      }

      if (xAxis == 0 || xAxis == 128)
      {
        rawData[2] = 0;
        rawData[3] = 0;
      }
      else if (xAxis > 128)
      {
        rawData[2] = ScaleInteger(256-xAxis+128, 128, NORMALIZE_ANALOG_WITH_REMOTE ? MAX_LEFT_REMOTE : NORMALIZE_ANALOG ? MAX_LEFT : 255, 0, 255);
        rawData[3] = 0;
      }
      else
      {
        rawData[3] = ScaleInteger(xAxis, 0,  NORMALIZE_ANALOG_WITH_REMOTE ? MAX_RIGHT_REMOTE :NORMALIZE_ANALOG ? MAX_RIGHT : 127, 0, 255);
        rawData[2] = 0;
      }

      timeout = millis();
      printRawData();
#else     
     for(int i = 7; i >= 0; --i)
        Serial.print((c & (1 << i)) ? "1" : "0");
     Serial.print("|");
     
     for(int i = 0; i < 2; ++i)
     {
      char c = vSerial.read();
      for(int i = 7; i >= 0; --i)
        Serial.print((c & (1 << i)) ? "1" : "0");
      if (i == 1)
        Serial.print("\n");
      else
        Serial.print("|");
     }
#endif

  }
  else if (((millis() - timeout) >= TIMEOUT))
  {
    for(int i = 0; i < 4; ++i)
      rawData[i] = 0;
	xAxis = yAxis = 0;
    printRawData();
  }
}
