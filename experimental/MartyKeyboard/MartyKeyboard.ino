/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Fujitsu FM Towns Marty Keyboard/Mouse Firmware for Teensy 3.5
// Tested Settings: Teensy 3.5, 168 MHz (overclock), Fastest + pure-code with LTO, US English
// v1.0
// RetroSpy written by zoggins
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

// ---------- Uncomment this for debugging ouput --------------
//#define DEBUG
// ---------- Uncomment this for a mostly complete general purpose 9200 baud 8E1 serial sniffer  --------------
//#define SNIFFER

byte rawData[32];

void setup() {

  for(int i = 0; i < 32; ++i)
    rawData[i] = 0;
  
  Serial.begin(115200);
  Serial1.begin(9600, SERIAL_8E1);
}

byte controlModeOffset = 0;

void loop() {
  byte incomingByte;

#ifdef SNIFFER
  if (Serial1.available() > 1) 
  {
    incomingByte = Serial1.read();
    Serial.print("UART received: ");
    Serial.println(incomingByte, HEX);
  }
#else
  if (Serial1.available() > 1) 
  {
    incomingByte = Serial1.read();
    if (incomingByte == 0xA0)
    {
      incomingByte = Serial1.read();
      if (incomingByte < 0x7E)
      {
        incomingByte += controlModeOffset;
        rawData[incomingByte/8] |= (1 << (incomingByte % 8));
      }
      else if (incomingByte == 0x7E)
        controlModeOffset = 0x80;
    }
    else if (incomingByte == 0xB0)
    {
      incomingByte = Serial1.read();
      if (incomingByte < 0x7E)
      {
        incomingByte += controlModeOffset;
        rawData[incomingByte/8] &= ~(1 << (incomingByte % 8));
      }
      else if (incomingByte == 0x7E)
        controlModeOffset = 0;
      else if (incomingByte == 0x7F)
      {
        for(int i = 0; i < 32; ++i)
          rawData[i] = 0;
      }
    }
  }

#ifndef DEBUG
  byte expandedRawData[65];
  for(int i = 0; i < 32; ++i)
  {
    expandedRawData[i*2] = ((rawData[i] & 0x0F) << 4);
    expandedRawData[i*2+1] = (rawData[i] & 0xF0);
  }
  expandedRawData[64] = '\n';
  Serial.write(expandedRawData, 65);
#else
  for(int i = 0; i < 32; ++i)
  {  
    Serial.print(rawData[i], HEX);
    Serial.print(" ");
  }
  Serial.print("\n");
#endif
  delay(5);
#endif
}
