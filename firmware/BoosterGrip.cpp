#include "common.h"
#include "BoosterGrip.h"

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
BoosterGripControllerSpy boosterGrip(2, 3, 4, 5, 6, 7, 8);

inline void sendRawBoosterGripData()
{
  #ifndef DEBUG
  for (unsigned char i = 0; i < 7; ++i)
  {
    Serial.write (currentState & (1 << i) ? ONE : ZERO );
  }
  Serial.write( SPLIT );
  #else
  if (currentState != lastState)
  {
      Serial.print((currentState & BG_BTN_UP)    ? "U" : "0");
      Serial.print((currentState & BG_BTN_DOWN)  ? "D" : "0");
      Serial.print((currentState & BG_BTN_LEFT)  ? "L" : "0");
      Serial.print((currentState & BG_BTN_RIGHT) ? "R" : "0");
      Serial.print((currentState & BG_BTN_1)     ? "1" : "0");
      Serial.print((currentState & BG_BTN_2)     ? "2" : "0");
      Serial.print((currentState & BG_BTN_3)     ? "3" : "0");
      Serial.print("\n");
      lastState = currentState;
  } 
  #endif
}

inline void loop_BoosterGrip()
{
  currentState = boosterGrip.getState();
  sendRawBoosterGripData();
}

