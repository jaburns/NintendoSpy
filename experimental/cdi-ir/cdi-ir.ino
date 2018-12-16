/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy CD-i IR Remote Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// Requires this package: https://github.com/cyborg5/IRLib2
// Requires an IR receiver attached to Arduino digital pin #2

#include "IRLibAll.h"

// These values may need adjusting based on the controller used.
#define TIMEOUT 100
#define NORMALIZE_ANALOG true
#define MAX_LEFT   173
#define MAX_RIGHT   45
#define MAX_UP     173
#define MAX_DOWN    45

// Uncomment for 2 levels of debugging output
//#define PRETTY_PRINT
//#define DEBUG
 
//Create a receiver object to listen on pin 2
IRrecvPCI myReceiver(2);
 
//Create a decoder object 
IRdecode myDecoder;   
unsigned long timeout;
byte rawData[6];

void setup() {
  Serial.begin(115200);
  delay(2000); while (!Serial); //delay for Leonardo
  myReceiver.enableIRIn(); // Start the receiver
#ifdef DEBUG
  Serial.println(F("Ready to receive IR signals"));
#endif
  timeout = millis();
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
    if (i == 5)
      Serial.print("\n");
    else
      Serial.print("|"); 
  }
#endif
}

void loop() {


  if (myReceiver.getResults()) {
      myDecoder.decode();   //Decode it
      if (myDecoder.protocolNum == 4 
          && myDecoder.address == 0 
          && myDecoder.bits == 32 
          && (myDecoder.value & 0b11111111111111000000000000000000) == 0x19100000)
      {
#ifdef DEBUG
        for(int32_t i = 31; i >=0;--i)
          Serial.print((myDecoder.value & ((int32_t)1 << i)) != 0 ? "1" : "0");
        Serial.print("\n");
      }
  }
#else
        byte yaxis = 0;
        for (int i = 0; i < 8;++i)
        {
            yaxis |= (myDecoder.value & (1 << i));
        }
        if (yaxis == 0 || yaxis == 128)
        {
          rawData[0] = 0;
          rawData[1] = 0;
        }
        else if (yaxis > 128)
        {
          rawData[0] =  ScaleInteger(yaxis, 129, NORMALIZE_ANALOG ? MAX_UP : 255, 0, 255);
          rawData[1] = 0;
        }
        else
        {
          rawData[1] =  ScaleInteger(yaxis, 1, NORMALIZE_ANALOG ? MAX_DOWN : 127, 0, 255);
          rawData[0] = 0;
        }
        
        byte xaxis = 0;
        for (int i = 8; i < 16;++i)
        {
            xaxis |= ((myDecoder.value & (1 << i)) >> 8);
        } 
        if (xaxis == 0 || xaxis == 128)
        {
          rawData[2] = 0;
          rawData[3] = 0;
        }
        else if (xaxis > 128)
        {
          rawData[2] = ScaleInteger(xaxis, 129, NORMALIZE_ANALOG ? MAX_LEFT : 255, 0, 255);
          rawData[3] = 0;
        }
        else
        {
          rawData[3] = ScaleInteger(xaxis, 1, NORMALIZE_ANALOG ? MAX_RIGHT : 127, 0, 255);
          rawData[2] = 0;
        }

        rawData[5] = (myDecoder.value & 0b00000000000000100000000000000000) != 0 ? 1 : 0;
        rawData[4] = (myDecoder.value & 0b00000000000000010000000000000000) != 0 ? 1 : 0;
                      
        timeout = millis();
        printRawData();
      }
    }
    else if (((millis() - timeout) >= TIMEOUT))
    {
      for(int i = 0; i < 6; ++i)
        rawData[i] = 0;

      printRawData();
    }
#endif
    myReceiver.enableIRIn();      //Restart receiver
 
}
