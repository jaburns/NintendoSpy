#include "common.h"
#include "Intellivision.h"

byte intRawData;

void read_IntellivisionData()
{
    intRawData = 0x00;

    intRawData |= PIN_READ(INTPIN1) == 0 ? 0x080 : 0x00;
    intRawData |= PIN_READ(INTPIN2) == 0 ? 0x040 : 0x00;
    intRawData |= PIN_READ(INTPIN3) == 0 ? 0x020 : 0x00;
    intRawData |= PIN_READ(INTPIN4) == 0 ? 0x010 : 0x00;
    intRawData |= PIN_READ(INTPIN6) == 0 ? 0x008 : 0x00;
    intRawData |= PINB_READ(INTPIN7) == 0 ? 0x004 : 0x00;
    intRawData |= PINB_READ(INTPIN8) == 0 ? 0x002 : 0x00;
    intRawData |= PINB_READ(INTPIN9) == 0 ? 0x001 : 0x00;
}

void writeIntellivisionDataToSerial()
{
    #ifndef DEBUG
    for (unsigned char i = 0; i < 32; ++i)
    {
      Serial.write( rawData[i] );
    }
    Serial.write( SPLIT );
    #else
    Serial.print(intRawData);
    Serial.print("|");
    for(int i = 0; i < 16; ++i)
    {
      Serial.print(rawData[i]);
    }
    Serial.print("|");
    for(int i = 0; i < 12; ++i)
    {
      Serial.print(rawData[i+16]);
    }
    Serial.print("|");   
    for(int i = 0; i < 4; ++i)
    {
      Serial.print(rawData[i+28]);
    }
    Serial.print("\n");
    #endif
}

void sendIntellivisionData_Sane()
{
  // This is an interpretted display method that tries to emulate what most
  // games would actually do.

  // If a shoulder button is pressed, keypad is ignored
  // 1 shoulder button and 1 disc direction can be hit at the same time
  // 2 or 3 shoulder buttons at the same time basically negates all pushes 
  // keypad pressed ignores disc

    // Check shoulder buttons first
    rawData[28] = rawData[29] = (intRawData & 0b00001010) == 0b00001010 ? 1 : 0;
    rawData[30] = (intRawData & 0b00000110) == 0b00000110 ? 1 : 0;
    rawData[31] = (intRawData & 0b00001100) == 0b00001100 ? 1 : 0;

    byte numPushed = 0;
    for(int i = 29; i < 32; ++i)
    {
      if (rawData[i])
        numPushed++;
    }

    if (numPushed > 1)
    {
      for(int i = 0; i < 32; ++i)
        rawData[i] = 0;
    }
    else
    {
      bool keyPadPushed = false;
      if (numPushed == 0)
      {
        rawData[16] = (intRawData & 0b00011000) == 0b00011000 ? 1 : 0;
        rawData[17] = (intRawData & 0b00010100) == 0b00010100 ? 1 : 0;
        rawData[18] = (intRawData & 0b00010010) == 0b00010010 ? 1 : 0;
        rawData[19] = (intRawData & 0b00101000) == 0b00101000 ? 1 : 0;
        rawData[20] = (intRawData & 0b00100100) == 0b00100100 ? 1 : 0;
        rawData[21] = (intRawData & 0b00100010) == 0b00100010 ? 1 : 0;
        rawData[22] = (intRawData & 0b01001000) == 0b01001000 ? 1 : 0;
        rawData[23] = (intRawData & 0b01000100) == 0b01000100 ? 1 : 0;
        rawData[24] = (intRawData & 0b01000010) == 0b01000010 ? 1 : 0;
        rawData[25] = (intRawData & 0b10001000) == 0b10001000 ? 1 : 0;
        rawData[26] = (intRawData & 0b10000100) == 0b10000100 ? 1 : 0;
        rawData[27] = (intRawData & 0b10000010) == 0b10000010 ? 1 : 0;
  
        for(int i = 16; i < 28; ++i)
          if (rawData[i])
          {
            keyPadPushed = true;
            break;
          }
      }
      else
      {
        for(int i = 16; i < 28; ++i)
        rawData[i] = 0;      
      }
  
      if (!keyPadPushed)
      {
        byte mask = 0b11110001;
      
        rawData[0]  = (intRawData & mask) == 0b01000000 ? 1 : 0;
        rawData[1]  = (intRawData & mask) == 0b01000001 ? 1 : 0;
        rawData[2]  = (intRawData & mask) == 0b01100001 ? 1 : 0;
        rawData[3]  = (intRawData & mask) == 0b01100000 ? 1 : 0;
        rawData[4]  = (intRawData & mask) == 0b00100000 ? 1 : 0;
        rawData[5]  = (intRawData & mask) == 0b00100001 ? 1 : 0;
        rawData[6]  = (intRawData & mask) == 0b00110001 ? 1 : 0;
        rawData[7]  = (intRawData & mask) == 0b00110000 ? 1 : 0;
        rawData[8]  = (intRawData & mask) == 0b00010000 ? 1 : 0;
        rawData[9]  = (intRawData & mask) == 0b00010001 ? 1 : 0;
        rawData[10] = (intRawData & mask) == 0b10010001 ? 1 : 0;
        rawData[11] = (intRawData & mask) == 0b10010000 ? 1 : 0;
        rawData[12] = (intRawData & mask) == 0b10000000 ? 1 : 0;
        rawData[13] = (intRawData & mask) == 0b10000001 ? 1 : 0;
        rawData[14] = (intRawData & mask) == 0b11000001 ? 1 : 0;
        rawData[15] = (intRawData & mask) == 0b11000000 ? 1 : 0;  
      }
      else
      {
        for(int i = 0; i < 16; ++i)
          rawData[i] = 0;          
      }
    }

    writeIntellivisionDataToSerial();
}

void sendIntellivisionData_Raw()
{
    // The raw behavior of multiple buttons pushes is completely bizarre.
    // I am trying to replicate how bizare it is!
    
    // Check shoulder buttons first
    rawData[28] = rawData[29] = (intRawData & 0b00001010) == 0b00001010 ? 1 : 0;
    rawData[30] = (intRawData & 0b00000110) == 0b00000110 ? 1 : 0;
    rawData[31] = (intRawData & 0b00001100) == 0b00001100 ? 1 : 0;

    bool shouldersPushed = false;
    if (rawData[28] == 1 || rawData[30] == 1 || rawData[31] == 1)
      shouldersPushed = true;

    rawData[16] = (intRawData & 0b00011000) == 0b00011000 ? 1 : 0;
    rawData[17] = (intRawData & 0b00010100) == 0b00010100 ? 1 : 0;
    rawData[18] = (intRawData & 0b00010010) == 0b00010010 ? 1 : 0;
    rawData[19] = (intRawData & 0b00101000) == 0b00101000 ? 1 : 0;
    rawData[20] = (intRawData & 0b00100100) == 0b00100100 ? 1 : 0;
    rawData[21] = (intRawData & 0b00100010) == 0b00100010 ? 1 : 0;
    rawData[22] = (intRawData & 0b01001000) == 0b01001000 ? 1 : 0;
    rawData[23] = (intRawData & 0b01000100) == 0b01000100 ? 1 : 0;
    rawData[24] = (intRawData & 0b01000010) == 0b01000010 ? 1 : 0;
    rawData[25] = (intRawData & 0b10001000) == 0b10001000 ? 1 : 0;
    rawData[26] = (intRawData & 0b10000100) == 0b10000100 ? 1 : 0;
    rawData[27] = (intRawData & 0b10000010) == 0b10000010 ? 1 : 0;

    // Keypad takes precedent, as long as all pushed buttons are in
    // the same column and shoulders are not "active"
    bool keypadPressed = false;
    if ((rawData[16] && rawData[19] && rawData[22] && rawData[25])
        || (rawData[17] && rawData[20] && rawData[23] && rawData[26])
        || (rawData[18] && rawData[21] && rawData[24] && rawData[27]))
      keypadPressed = !shouldersPushed;
    
    if (!keypadPressed)
    {
      // If shoulders are pushed its a bit check, if shoulders
      // are not active its a full equal check.
      byte mask = shouldersPushed ? 0b11110001 : 0xFF;
    
      rawData[0]  = (intRawData & mask) == 0b01000000 ? 1 : 0;
      rawData[1]  = (intRawData & mask) == 0b01000001 ? 1 : 0;
      rawData[2]  = (intRawData & mask) == 0b01100001 ? 1 : 0;
      rawData[3]  = (intRawData & mask) == 0b01100000 ? 1 : 0;
      rawData[4]  = (intRawData & mask) == 0b00100000 ? 1 : 0;
      rawData[5]  = (intRawData & mask) == 0b00100001 ? 1 : 0;
      rawData[6]  = (intRawData & mask) == 0b00110001 ? 1 : 0;
      rawData[7]  = (intRawData & mask) == 0b00110000 ? 1 : 0;
      rawData[8]  = (intRawData & mask) == 0b00010000 ? 1 : 0;
      rawData[9]  = (intRawData & mask) == 0b00010001 ? 1 : 0;
      rawData[10] = (intRawData & mask) == 0b10010001 ? 1 : 0;
      rawData[11] = (intRawData & mask) == 0b10010000 ? 1 : 0;
      rawData[12] = (intRawData & mask) == 0b10000000 ? 1 : 0;
      rawData[13] = (intRawData & mask) == 0b10000001 ? 1 : 0;
      rawData[14] = (intRawData & mask) == 0b11000001 ? 1 : 0;
      rawData[15] = (intRawData & mask) == 0b11000000 ? 1 : 0;  
    }
    else  // disk is inactive on a "typical" keypad push
    {
      for(int i = 0; i < 16; ++i)
        rawData[i] = 0;
    }

    writeIntellivisionDataToSerial();
}

inline void loop_Intellivision()
{
    noInterrupts();
    read_IntellivisionData();
    interrupts();
    if(INT_SANE_BEHAVIOR)
      sendIntellivisionData_Sane();
    else
      sendIntellivisionData_Raw();
}
