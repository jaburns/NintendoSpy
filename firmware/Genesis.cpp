#include "common.h"
#include "Genesis.h"

GenesisControllerSpy genesisController;

void genesis_pin_setup()
{
  for(int i = 2; i <= 6; ++i)
    pinMode(i, INPUT_PULLUP);
}

inline void sendRawGenesisData()
{
  #ifndef DEBUG
  for (unsigned char i = 0; i < 13; ++i)
  {
    Serial.write (currentState & (1 << i) ? ONE : ZERO );
  }
  Serial.write( SPLIT );
  #else
  if (currentState != lastState)
  {
      Serial.print((currentState & SCS_CTL_ON)    ? "+" : "-");
      Serial.print((currentState & SCS_BTN_UP)    ? "U" : "0");
      Serial.print((currentState & SCS_BTN_DOWN)  ? "D" : "0");
      Serial.print((currentState & SCS_BTN_LEFT)  ? "L" : "0");
      Serial.print((currentState & SCS_BTN_RIGHT) ? "R" : "0");
      Serial.print((currentState & SCS_BTN_START) ? "S" : "0");
      Serial.print((currentState & SCS_BTN_A)     ? "A" : "0");
      Serial.print((currentState & SCS_BTN_B)     ? "B" : "0");
      Serial.print((currentState & SCS_BTN_C)     ? "C" : "0");
      Serial.print((currentState & SCS_BTN_X)     ? "X" : "0");
      Serial.print((currentState & SCS_BTN_Y)     ? "Y" : "0");
      Serial.print((currentState & SCS_BTN_Z)     ? "Z" : "0");
      Serial.print((currentState & SCS_BTN_MODE)  ? "M" : "0");
      Serial.print("\n");
      lastState = currentState;
  }
  #endif
}

inline void sendRawGenesisMouseData()
{
  #ifndef DEBUG
  for(int i = 0; i < 3; ++i)
    for(int j = 0; j < 8; ++j)
      Serial.write((rawData[i] & (1 << j)) == 0 ? ZERO : ONE);
  #else
    for(int i = 0; i < 3; ++i)
      for(int j = 0; j < 8; ++j)
        Serial.print((rawData[i] & (1 << j)) == 0 ? "0" : "1");
  #endif   
  Serial.print("\n");
}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Update loop definitions for the various console modes.

inline void loop_Genesis()
{
  currentState = genesisController.getState();
  sendRawGenesisData();
}

inline void loop_GenesisMouse()
{
  genesisController.getMouseState(rawData);
  sendRawGenesisMouseData();
}

