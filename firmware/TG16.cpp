#include "common.h"
#include "TG16.h"

word lastDirections = 0;
word lastHighButtons = 0x0000;
word lastButtons = 0;
bool highButtons = true;
bool seenHighButtons = false;

inline void read_TgData()
{
  currentState = 0x0000;
  while((PIND & 0b01000000) == 0){}    
  asm volatile(
        "nop\nnop\n");
  word temp = 0;
  temp |= ((PIND & 0b00111100) >> 2);
  if ((temp & 0b00001111) == 0b00000000)
  {
    currentState |= lastDirections;
    highButtons = false;
    seenHighButtons = true;
  }
  else
  {
    currentState |= temp;
    lastDirections = temp;
    highButtons = true;
  }
  
  while((PIND & 0b01000000) != 0){}    
    asm volatile(MICROSECOND_NOPS);
      temp = 0;
      temp |= ((PIND & 0b00111100) << 2);
      if (highButtons == true && seenHighButtons == true)
      {
        currentState |= (temp << 4);
        lastHighButtons = temp;
        currentState |= lastButtons;   
      }
      else
      {
        currentState |= temp;
        lastButtons = temp;
        if (seenHighButtons)
          currentState |= (lastHighButtons << 4);
        else
          currentState |= 0b0000111100000000;
      }
    currentState = ~currentState;
}

inline void sendRawTgData()
{
    #ifndef DEBUG
    for (unsigned char i = 0; i < 12; ++i)
    {
      Serial.write (currentState & (1 << i) ? ONE : ZERO );
    }
    Serial.write( SPLIT );
    #else 
    Serial.print((currentState & 0b0000000000000001)    ? "U" : "0");
    Serial.print((currentState & 0b0000000000000010)    ? "R" : "0");
    Serial.print((currentState & 0b0000000000000100)    ? "D" : "0");
    Serial.print((currentState & 0b0000000000001000)    ? "L" : "0");
    Serial.print((currentState & 0b0000000000010000)    ? "A" : "0");
    Serial.print((currentState & 0b0000000000100000)    ? "B" : "0");
    Serial.print((currentState & 0b0000000001000000)    ? "S" : "0");
    Serial.print((currentState & 0b0000000010000000)    ? "R" : "0");
    Serial.print((currentState & 0b0000000100000000)    ? "3" : "0");
    Serial.print((currentState & 0b0000001000000000)    ? "4" : "0");
    Serial.print((currentState & 0b0000010000000000)    ? "5" : "0");
    Serial.print((currentState & 0b0000100000000000)    ? "6" : "0");
    Serial.print("\n");
    #endif
}

inline void loop_TG16()
{
  noInterrupts();
  read_TgData();
  interrupts();
  sendRawTgData();
  delay(1);
}
