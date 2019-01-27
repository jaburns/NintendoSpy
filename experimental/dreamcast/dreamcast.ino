
#define DEBUG

byte rawData[16000];
byte* p;
int byteCount;

void setup() {
  pinMode(2, INPUT);     // DC Pin 1
  pinMode(14, INPUT);    // DC Pin 5
  pinMode(5, INPUT);     // Connect to Teensy GND
  pinMode(6, INPUT);     // Connect to Teensy GND
  pinMode(7, INPUT);     // Connect to Teensy GND
  pinMode(8, INPUT);     // Connect to Teensy GND
  pinMode(20, INPUT);    // Connect to Teensy GND
  pinMode(21, INPUT);    // Connect to Teensy GND

  p = &rawData[6];

  Serial.begin(115200);
}

#define DETECT_FALLING_EDGE rawData[byteCount] = GPIOD_PDIR; do { prevPin = rawData[byteCount]; rawData[byteCount] = GPIOD_PDIR; } while( rawData[byteCount] >= prevPin);

FASTRUN void loop() 
{

  byte prevPin;

start_state:    
state1:
  interrupts();
  byteCount = 0;
  DETECT_FALLING_EDGE
  if (prevPin != 0x3 || rawData[byteCount] != 0x2)   // Starts with both pins High and pin 1 drops
    goto state1;
  ++byteCount;

//state2:
  DETECT_FALLING_EDGE
  if (prevPin != 0x2 || rawData[byteCount] != 0x0)  // Pin 5 now drops, pin 1 stays low
    goto state1;
  ++byteCount;

state3:
  DETECT_FALLING_EDGE
  if (prevPin != 0x2 || rawData[byteCount] != 0x0)  // Pin 5 drops 3 more times, pin 1 says low
    goto state1;
  else if (++byteCount != 5)
    goto state3;

//state4:
  DETECT_FALLING_EDGE
  if (rawData[byteCount] != 0x01 || prevPin != 0x3) // Pin 5 drops 1 more times, this time Pin 1 is high
    goto state1;
  ++byteCount;

  noInterrupts();
state5:                             // Phase 1
  DETECT_FALLING_EDGE
  if (prevPin == 0x3 && rawData[byteCount] == 0x1)
    goto state7;
  else if (prevPin == 0x02 || prevPin == 0x0)
    goto state1;
  ++byteCount;
  
//state6:                           // Phase 2
  DETECT_FALLING_EDGE
  if (prevPin == 0x01 || prevPin == 0x0)
    goto state1;
  ++byteCount;
  goto state5;

state7:
  interrupts();
#ifndef DEBUG
  rawData[byteCount++] = '\n';
  Serial.write(rawData, byteCount-6);    
#else
  int j = 0;

  Serial.print(byteCount-6);
  Serial.print("|");

  byte numFrames = 0;
  for(int i = 0; i < 4; ++i)
  {
    numFrames |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
    numFrames |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
    j += 2;
  }
  Serial.print(numFrames);
  
  for(int i = 8; i < 16; i++)
  {
    if (j % 8 == 0 && j != 0)
      Serial.print("|");
    Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
    Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
    j += 2;
  }

  Serial.print("|");
  byte dcCommand = 0;
  for(int i = 0; i < 4; ++i)
  {
    dcCommand |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
    dcCommand |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
    j += 2;
  }
  Serial.print(dcCommand);

  Serial.print("|");
  unsigned int controllerType = 0;
  for(int i = 0; i < 2; i++)
  {
    for(int k = 0; k < 4; ++k)
    {
      controllerType |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(k*2)+(i*8));
      controllerType |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(k*2)+(i*8));
      j += 2;
    }
  }
  Serial.print(controllerType);
  
  j+=16;

  if (dcCommand == 8 && controllerType == 0x40)
  {
    byte keycode[6];

    keycode[1] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[1] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[1] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    
    keycode[0] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[0] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[0] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    Serial.print("|");
    for(int i = 0; i < 4; i++)
    {
      Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
      Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
      j += 2;
    } 

    Serial.print("|");
    for(int i = 0; i < 4; i++)
    {
      Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
      Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
      j += 2;
    } 

    keycode[5] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[5] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[5] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    keycode[4] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[4] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[4] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    keycode[3] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[3] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[3] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    keycode[2] = 0;
    for(int i = 0; i < 4; ++i)
    {
      keycode[2] |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      keycode[2] |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }

    for(int i = 0; i < 6; ++i)
    {
      Serial.print("|");
      Serial.print(keycode[i]);
    }

  }
  else if (dcCommand == 8 && controllerType == 0x200)
  {
    j += 24;
    Serial.print("|");
    for(int i = 0; i < 4; i++)
    {
      Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
      Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
      j += 2;
    } 

    Serial.print("|");
    unsigned int axis1 = 0;
    for(int i = 1; i >= 0; --i)
    {
      for(int k = 0; k < 4; ++k)
      {
        axis1 |= ((p[j] & 0x02) != 0 ? 1 : 0) << ((7-k*2)+(i*8));
        axis1 |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(k*2)+(i*8));
        j += 2;
      }
    }
    Serial.print(axis1);
    
    Serial.print("|");
    unsigned int axis2 = 0;
    for(int i = 1; i >= 0; --i)
    {
      for(int k = 0; k < 4; ++k)
      {
        axis2 |= ((p[j] & 0x02) != 0 ? 1 : 0) << ((7-k*2)+(i*8));
        axis2 |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(k*2)+(i*8));
        j += 2;
      }
    }
    Serial.print(axis2);

    j += 16;

    Serial.print("|");
    unsigned int axis3 = 0;
    for(int i = 1; i >= 0; --i)
    {
      for(int k = 0; k < 4; ++k)
      {
        axis3 |= ((p[j] & 0x02) != 0 ? 1 : 0) << ((7-k*2)+(i*8));
        axis3 |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(k*2)+(i*8));
        j += 2;
      }
    }
    Serial.print(axis3);
  
  }
  else if (dcCommand == 8 && controllerType == 1)
  {
    Serial.print("|");
    byte ltrigger = 0;
    for(int i = 0; i < 4; ++i)
    {
      ltrigger |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      ltrigger |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    Serial.print(ltrigger);
  
    Serial.print("|");
    byte rtrigger = 0;
    for(int i = 0; i < 4; ++i)
    {
      rtrigger |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      rtrigger |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    Serial.print(rtrigger);
  
    for(int i = 0; i < 8; i++)
    {
      if (j % 8 == 0 && j != 0)
        Serial.print("|");
      Serial.print((p[j] & 0x02) != 0 ? "1" : "0");
      Serial.print((p[j+1] & 0x01) != 0 ? "1" : "0");
      j += 2;
    } 
  
    j += 16;
  
    Serial.print("|");
    byte joyy = 0;
    for(int i = 0; i < 4; ++i)
    {
      joyy |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      joyy |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    Serial.print(joyy);
  
    Serial.print("|");
    byte joyx = 0;
    for(int i = 0; i < 4; ++i)
    {
      joyx |= ((p[j] & 0x02) != 0 ? 1 : 0) << (7-(i*2));
      joyx |= ((p[j+1] & 0x01) != 0 ? 1 : 0) << (6-(i*2));
      j += 2;
    }
    Serial.print(joyx);  
  }
  Serial.print("\n");
#endif
  
  goto start_state;
}
