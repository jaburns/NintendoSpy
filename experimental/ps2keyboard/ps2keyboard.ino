#include "PS2Keyboard.h"

const int DataPin = 4;
const int IRQpin =  3;

PS2Keyboard keyboard;

void setup() {
  delay(1000);
  keyboard.begin(DataPin, IRQpin);
  Serial.begin(115200);
}

void loop() {
  if (keyboard.available()) {    
    char c = keyboard.read();
  }
}