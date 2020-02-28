#include "common.h"
#include "SMS.h"

// Specify the Arduino pins that are connected to
// DB9 Pin 1, DB9 Pin 2, DB9 Pin 3, DB9 Pin 4, DB9 Pin 5, DB9 Pin 6, DB9 Pin 9
SMSControllerSpy SMSController(2, 3, 4, 5, 7, 8);
SMSControllerSpy SMSOnGenesisController(2, 3, 4, 5, 6, 7);

void sms_pin_setup()
{
  for(int i = 2; i <= 6; ++i)
    pinMode(i, INPUT_PULLUP);
}

inline void sendSMSOnGenesisData()
{
  #ifndef DEBUG
      Serial.write(ZERO);
      Serial.write((currentState & CC_BTN_UP)    ? 1 : 0);
      Serial.write((currentState & CC_BTN_DOWN)  ? 1 : 0);
      Serial.write((currentState & CC_BTN_LEFT)  ? 1 : 0);
      Serial.write((currentState & CC_BTN_RIGHT) ? 1 : 0);
      Serial.write((currentState & CC_BTN_1)     ? 1 : 0);
      Serial.write((currentState & CC_BTN_2)     ? 1 : 0);   
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(ZERO);
      Serial.write(SPLIT);  
  #else
  if (currentState != lastState)
  {
      Serial.print("-");
      Serial.print((currentState & CS_BTN_UP)    ? "U" : "0");
      Serial.print((currentState & CS_BTN_DOWN)  ? "D" : "0");
      Serial.print((currentState & CS_BTN_LEFT)  ? "L" : "0");
      Serial.print((currentState & CS_BTN_RIGHT) ? "R" : "0");
      Serial.print("0");
      Serial.print("0");
      Serial.print((currentState & CS_BTN_1)     ? "1" : "0");
      Serial.print((currentState & CS_BTN_2)     ? "2" : "0");
      Serial.print("0");
      Serial.print("0");
      Serial.print("0");
      Serial.print("0");
      Serial.print("\n");
      lastState = currentState;
  }
  #endif  
}

inline void sendRawSMSData()
{
  #ifndef DEBUG
  for (unsigned char i = 0; i < 6; ++i)
  {
    Serial.write (currentState & (1 << i) ? ONE : ZERO );
  }
  Serial.write( SPLIT );
  #else
  if (currentState != lastState)
  {
      Serial.print((currentState & CC_BTN_UP)    ? "U" : "0");
      Serial.print((currentState & CC_BTN_DOWN)  ? "D" : "0");
      Serial.print((currentState & CC_BTN_LEFT)  ? "L" : "0");
      Serial.print((currentState & CC_BTN_RIGHT) ? "R" : "0");
      Serial.print((currentState & CC_BTN_1)     ? "1" : "0");
      Serial.print((currentState & CC_BTN_2)     ? "2" : "0");
      Serial.print("\n");
      lastState = currentState;
  } 
  #endif
}

inline void loop_SMS()
{
  currentState = SMSController.getState();
  sendRawSMSData();
}

inline void loop_SMS_on_Genesis()
{
  currentState = SMSOnGenesisController.getState();
  sendSMSOnGenesisData();
}

