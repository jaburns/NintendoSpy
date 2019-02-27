

byte rawData[1024];
byte cleanData[274];

void setup(void)
{
  pinMode(18, INPUT);
  pinMode(19, INPUT);
  cleanData[0] = 2;
  cleanData[1] = -1;
  cleanData[46] = '\n';
  Serial.begin(115200);
  delay(5);
  //while (!Serial);
}


uint8_t   current_portb = 0;
uint8_t   last_portb;
int       i2c_index  = 0;
bool      isControllerPoll = false;
bool      isControllerID = false;
bool      isEncrypted = false;
byte      encryptionKeySet = 0;

//#define ANALYZE
//#define DEBUG

void loop(void)
{

  last_portb = current_portb;
  noInterrupts();
  current_portb = GPIOB_PDIR & 12;
  interrupts();
  bool bDataReady = current_portb != last_portb;


  if (bDataReady)
  {
    bool sendOutput = false;
    if ((last_portb == 0xC) && (current_portb == 0x8))
    {
      // START
      i2c_index = 0;
    }
    else if ((last_portb == 0x8) && (current_portb == 0xC))
    {
      // STOP
      i2c_index -= (i2c_index % 9);
#ifdef ANALYZE
      byte addr = 0;
      for (int i = 0; i < 7; ++i)
      {
        if (rawData[i] != 0)
          addr |= 1 << (6 - i);
      }
      Serial.print(i2c_index / 9);
      Serial.print(',');
      Serial.printf("Addr=0x%02X,", addr);
      if (rawData[7] == 0)
        Serial.print("W");
      else
        Serial.print("R");
      Serial.print(",");
      Serial.print(rawData[8] ? "N" : "A");
      Serial.print(",");

      int i = 9;
      byte numbytes = 1;
      while (i < i2c_index)
      {
        byte data = 0;
        for (int j = 0; j < 8; ++j)
        {
          if (rawData[j + i] != 0)
            data |= 1 << (7 - j);
        }
        ++numbytes;
        Serial.printf("0x%02X,", data);
        Serial.print(rawData[i + 8] ? "N" : "A");
        Serial.print(" ");
        i += 9;
      }
      Serial.print("\n");
#else
      byte tempData[128];
      tempData[0] = 0;
      for (int i = 0; i < 7; ++i)
      {
        if (rawData[i] != 0)
          tempData[0] |= 1 << (6 - i);
      }

      if (rawData[8] != 0) return;

      bool isWrite = rawData[7] == 0;

      if (tempData[0] != 0x52) return;


      int i = 9;
      byte numbytes = 1;
      while (i < i2c_index)
      {
        tempData[numbytes] = 0;
        for (int j = 0; j < 8; ++j)
        {
          if (rawData[j + i] != 0)
            tempData[numbytes] |= 1 << (7 - j);
        }
        ++numbytes;

        if (!isWrite && i + 8 == i2c_index - 1)
        {
          if (rawData[i + 8] == 0) return;
        }
        else
        {
          if (rawData[i + 8] != 0) return;
        }
        i += 9;
      }

      if (isWrite)
      {

        if (numbytes == 2 && tempData[1] == 0)
        {
          isControllerPoll = true;
        }
        else if (numbytes == 3 && tempData[1] == 0xF0 && tempData[2] == 0x55)
        {
          isEncrypted = false;
          cleanData[1] = -1;
        }
        else if (numbytes == 2 && tempData[1] == 0xFA)
        {
          isControllerID = true;
        }
        else if (numbytes == 8 && tempData[1] == 0x40)
        {
          int j = 2;
          for (int i = 0; i < 6; i++)
          {
            cleanData[j] = (tempData[2 + i] & 0xF0);
            cleanData[j + 1] = ((tempData[2 + i] & 0x0F) << 4);
            j += 2;
          }
        }
        else if (numbytes == 8 && tempData[1] == 0x46)
        {
          int j = 14;
          for (int i = 0; i < 6; i++)
          {
            cleanData[j] = (tempData[2 + i] & 0xF0);
            cleanData[j + 1] = ((tempData[2 + i] & 0x0F) << 4);
            j += 2;
          }
        }
        else if (numbytes == 6 && tempData[1] == 0x4C)
        {

          int j = 26;
          for (int i = 0; i < 4; i++)
          {
            cleanData[j] = (tempData[2 + i] & 0xF0);
            cleanData[j + 1] = ((tempData[2 + i] & 0x0F) << 4);
            j += 2;
          }
          isEncrypted = true;
          encryptionKeySet = (encryptionKeySet + 1) % 255;
          if (encryptionKeySet == 10)
            encryptionKeySet = 11;
          cleanData[1] = encryptionKeySet;
        }
      }
      else
      {
        if (isControllerID && (numbytes == 7 || numbytes == 9))
        {
          if (tempData[numbytes - 2] == 0 && tempData[numbytes - 1] == 0)
          {
            cleanData[0] = 0;
          }
          else if (tempData[numbytes - 2] == 1 && tempData[numbytes - 1] == 1)
          {
            cleanData[0] = 1;
          }
          else
            cleanData[0] = 2;

          isControllerID = false;

        }
        else if (isControllerPoll && (numbytes == 7 || numbytes == 17))
        {

          int j = 34;
          for (int i = 0; i < 6; i++)
          {
            cleanData[j] = (tempData[1 + i] & 0xF0);
            cleanData[j + 1] = (tempData[1 + i] << 4);
            j += 2;
          }
          isControllerPoll = false;
#ifdef DEBUG
          Serial.print(cleanData[0]);
          Serial.print(' ');
          Serial.print(cleanData[1]);
          Serial.print(' ');
          j = 2;
          for (int i = 0; i < 22; ++i)
          {
            byte data = (cleanData[j] | (cleanData[j + 1] >> 4));
            Serial.print(data);
            Serial.print(' ');
            j += 2;
          }
          Serial.print('\n');
#else
          Serial.write(cleanData, 47);
#endif
        }
      }
#endif
    }
    else if ((last_portb == 0x4) && (current_portb == 0xC))
    {
      // ONE
      rawData[i2c_index++] = 1;
    }
    else if ((last_portb == 0x0) && (current_portb == 0x8))
    {
      // ZERO
      rawData[i2c_index++] = 0;
    }
  }
}
