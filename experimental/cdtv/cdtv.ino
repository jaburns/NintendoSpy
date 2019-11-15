/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Commodore CDTV Remote Controller Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// ---------------------------------------------------------------------------------
// Uncomment this for serial debugging output
//#define DEBUG

#include <IRremote.h>

#define WIRELESS_TIMEOUT 100

int RECV_PIN = 2;

IRrecv irrecv(RECV_PIN);

unsigned long GetBitMask_RemoteKeys(unsigned long received_code)
{
  byte bit = 0;
  switch(received_code)
  {
    // 1
    case 0x1ffe:
      bit = 6;
      break;  
    // 2
    case 0x21fde:
      bit = 7;
      break;  
    // 3
    case 0x11fee:
      bit = 8;
      break;  
    // 4
    case 0x9ff6:
      bit = 9;
      break;  
    // 5
    case 0x29fd6:
      bit = 10;
      break;  
    // 6
    case 0x19fe6:
      bit = 11;
      break;  
    // 7
    case 0x5ffa:
      bit = 12;
      break;  
    // 8
    case 0x25fda:
      bit = 13;
      break;  
    // 9
    case 0x15fea:
      bit = 14;
      break;  
    // 0
    case 0x39fc6:
      bit = 15;
      break;  

    // Escape
    case 0x31fce:
      bit = 16;
      break;  
    // Enter
    case 0x35fca:
      bit = 17;
      break;  

    // Genlock
    case 0x22fdd:
      bit = 18;
      break;  
    // CD/TV
    case 0x2ffd:
      bit = 19;
      break;      
    // Power
    case 0x12fed:
      bit = 20;
      break;      

    // Rew
    case 0x32fcd:
      bit = 21;
      break;  
    // Play/Pause
    case 0xaff5:
      bit = 22;
      break;      
    // FF
    case 0x1afe5:
      bit = 23;
      break;  
    // Stop
    case 0x2afd5:
      bit = 24;
      break; 

    // Volume Up
    case 0x6ff9:
      bit = 25;
      break;    
    // Volume Down
    case 0x3afc5:
      bit = 26;
      break;
    default:
      return 0;
      break;     
  }
  return ((unsigned long long)1 << bit);
  
}

unsigned long long rawData;
unsigned long wireless_timeout;

void setup()
{
  rawData = 0;
  Serial.begin(115200);
  // In case the interrupt driver crashes on setup, give a clue
  // to the user what's going on.
  //Serial.println("Enabling IRin");
  irrecv.enableIRIn(); // Start the receiver
  //Serial.println("Enabled IRin");
}

unsigned long long GetBitMask_ControllerKeys(unsigned long received_code)
{
  unsigned long long retVal = 0;

  int offset = ((received_code & 0b00000000000000000000100000000000) == 0) ? 27 : 0;
  
  if ((received_code & 0b00000000000000000000000000000100) == 0)
    retVal |= ((unsigned long long)1 << (2+offset));
     
  if ((received_code & 0b00000000000000000000000000001000) == 0)
    retVal |= ((unsigned long long)1 << (0+offset));

  if ((received_code & 0b00000000000000000000000000010000) == 0)
    retVal |= ((unsigned long long)1 << (3+offset));

  if ((received_code & 0b00000000000000000000000000100000) == 0)
    retVal |= ((unsigned long long)1 << (1+offset));

  if ((received_code & 0b00000000000000000000000001000000) == 0)
    retVal |= ((unsigned long long)1 << (4+offset));

  if ((received_code & 0b00000000000000000000000010000000) == 0)
    retVal |= ((unsigned long long)1 << (5+offset));

  return retVal;
  
}

void loop() {
  decode_results results;
  if (irrecv.decode(&results)) 
  {
    if (results.decode_type == CDTV && results.bits == 24)
    {
      // Handle "remote control" keys
      if ((results.value & 0b0000000000000000000000000000011) != 0x03)
        rawData = GetBitMask_RemoteKeys(results.value);
      else
        rawData = GetBitMask_ControllerKeys(results.value);
      wireless_timeout = millis();
    }
    else if ((results.decode_type == NEC && results.bits == 0 && results.value == 0xFFFFFFFF)
              || (results.decode_type == CDTV && results.bits == 4 && results.value == 0xFFFFFF))
    {
      wireless_timeout = millis();
    }
    irrecv.resume(); // Receive the next value
  }
  else if (((millis() - wireless_timeout) >= WIRELESS_TIMEOUT))
  {
    rawData = 0;
  }

#ifdef DEBUG
  for(int i = 0; i < 33; ++i)
    Serial.print((rawData & ((unsigned long long)1 << i)) != 0 ? "1" : "0");
  Serial.print("\n");
#else
  for(int i = 0; i < 33; ++i)
    Serial.write((rawData & ((unsigned long long)1 << i)) != 0 ? 1 : 0);
  Serial.write("\n");
#endif
}
