/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy PS/2 Keyboard Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// -------------------------------------------------------------
//    Special Handling Modes
// -------------------------------------------------------------
//#define MODE_LYRA3

// ---------- Uncomment these for debugging modes --------------
//#define DEBUG

#define BUFFER_SIZE 45
static volatile uint8_t buffer[BUFFER_SIZE];
static volatile uint8_t head, tail;

static int DataPin = 4;
const int IRQpin   =  3;

// The ISR for the external interrupt
void ps2interrupt(void)
{
  static uint8_t bitcount=0;
  static uint8_t incoming=0;
  static uint32_t prev_ms=0;
  uint32_t now_ms;
  uint8_t n, val;

  val = digitalRead(DataPin);
  now_ms = millis();
  if (now_ms - prev_ms > 250) {
    bitcount = 0;
    incoming = 0;
  }
  prev_ms = now_ms;
  n = bitcount - 1;
  if (n <= 7) {
    incoming |= (val << n);
  }
  bitcount++;
  if (bitcount == 11) {
    uint8_t i = head + 1;
    if (i >= BUFFER_SIZE) i = 0;
    if (i != tail) {
      buffer[i] = incoming;
      head = i;
    }
    bitcount = 0;
    incoming = 0;
  }
}

static inline uint8_t get_scan_code(void)
{
  uint8_t c, i;

  i = tail;
  if (i == head) return 0;
  i++;
  if (i >= BUFFER_SIZE) i = 0;
  c = buffer[i];
  tail = i;
  return c;
}

#define BREAK     0x01
#define MODIFIER  0x02
#define PAUSE     0x04
#ifdef MODE_LYRA3
#define LYRA3_CAPSLOCK 0x08
#define LYRA3_SCROLLLOCK 0x10
#endif
byte rawData[256];

void setup() {
  delay(1000);

  for(int i = 0; i < 256; ++i)
    rawData[i] = 0;

  // initialize the pins
  pinMode(IRQpin, INPUT_PULLUP);
  pinMode(DataPin, INPUT_PULLUP);
  
  head = 0;
  tail = 0;
  attachInterrupt(1, ps2interrupt, FALLING);
  
  Serial.begin(115200);
}

void loop() {
  static int FACount = 0;
  static int F4Count = 0;
  static uint8_t state=0;
  uint8_t s;

  while (1) {
    s = get_scan_code();
    if (!s) return;
    #ifdef DEBUG
    Serial.println(s, HEX);
    #endif
    if (s == 0xF0) 
    {
      state |= BREAK;
      continue;
    } 
    else if (s == 0xE0) 
    {
      state |= MODIFIER;
      continue;
    }
    else if (s == 0xE1) 
    {
      state |= PAUSE;
      continue;
    }
    else 
    {  
      if (state & BREAK) 
      {
        if (state & PAUSE)
        {
          if (s == 0x77)
          {
            rawData[225/8] &= ~(1 << (225%8));
            state = 0;
            break;
          }
          continue;
        }
        else if (state & MODIFIER)
        {
          rawData[(s+128)/8] &= ~(1 << ((s+128)%8));
          state = 0;
          break;
        }
        #ifdef MODE_LYRA3
        else if (s == 0x58 || (state & LYRA3_CAPSLOCK))
        {
          state |= LYRA3_CAPSLOCK;
          if (s == 0xFA)
          {
            FACount++;
          }
          if (FACount == 2)
          { 
            state = 0;
            FACount = 0;
            rawData[0x58/8] &= ~(1 << (0x58%8));
            break;
          }
          continue;
        }
        #endif
        else
        {
          rawData[s/8] &= ~(1 << (s%8));
          state = 0;
          break;  
        }
      }
      else if (state & MODIFIER)
      {
        #ifdef MODE_LYRA3
        if (s == 0xFC)
        {
          rawData[0x7e/8] &= ~(1 << (0x7e%8));
          state = 0;
          break;  
        }
        #endif
        rawData[(s+128)/8] |= (1 << ((s+128)%8));
        state = 0;
        break;
      }
      else if (state & PAUSE)
      {
        if (s == 0x77)
        {
          rawData[225/8] |= (1 << (225%8));
          state = 0;
          break;
        }
        continue;
      }
      #ifdef MODE_LYRA3
      else if (s == 0x7e || (state & LYRA3_SCROLLLOCK))
      {
        state |= LYRA3_SCROLLLOCK;
        if (s == 0xF4)
        {
          F4Count++;
        }
        if (F4Count == 2)
        { 
          state = 0;
          F4Count = 0;
          rawData[0x7e/8] |= (1 << (0x7e%8));
          break;
        }
        continue;
      }
      #endif
      else
      {
        rawData[s/8] |= (1 << (s%8));
        state = 0;
        break;
      }
    }
  }

  #ifndef DEBUG
  int checksum = 0;
  for(int i = 0 ; i < 32; i++)
  {
    checksum += rawData[i];
    Serial.write(((rawData[i] & 0x0F) << 4));
    Serial.write(rawData[i] & 0xF0);
  }
  Serial.write((checksum & 0x000f) << 4);
  Serial.write(checksum & 0x00f0);
  Serial.write((checksum & 0x0f00) >> 4);
  Serial.write((checksum & 0xf000) >> 8);
  Serial.write('\n');
  #endif
}
