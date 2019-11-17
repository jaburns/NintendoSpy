/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Amiga Keyboard Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// ---------- Uncomment these for debugging modes --------------
//#define DEBUG
//#define SNIFFER

#define BUFFER_SIZE 45
static volatile uint8_t buffer[BUFFER_SIZE];
static volatile uint8_t head, tail;

static int DataPin = 4;
const int IRQpin   = 3;

// The ISR for the external interrupt
void clockInterrupt(void)
{
  static uint8_t bitcount=0;
  static uint8_t incoming=0;
  static uint32_t prev_ms=0;
  uint32_t now_ms;
  uint8_t val;

  val = digitalRead(DataPin);
  now_ms = micros();
  if (now_ms - prev_ms > 100) {
    bitcount = 0;
    incoming = 0;
  }
  prev_ms = now_ms;
  if (bitcount < 7) {
    incoming |= (val << (6-bitcount));
  }
  else
  {
    incoming |= (val << 7);
  }
  bitcount++;
  if (bitcount == 8) {
    uint8_t i = head + 1;
    if (i >= BUFFER_SIZE) i = 0;
    if (i != tail) {
      buffer[i] = ~incoming;
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

byte rawData[16];

void setup() {
  delay(1000);

  for(int i = 0; i < 16; ++i)
    rawData[i] = 0;

  // initialize the pins
  pinMode(IRQpin, INPUT_PULLUP);
  pinMode(DataPin, INPUT_PULLUP);
  
  head = 0;
  tail = 0;
  attachInterrupt(1, clockInterrupt, FALLING);
  
  Serial.begin(115200);
}

void loop() {

  uint8_t s;

  while (1) {
    s = get_scan_code();
    if (s)
    {  
#ifdef SNIFFER
      Serial.print((s & 0b01111111), HEX);
      if ((s & 0b10000000) == 0)
        Serial.println(" depressed");
      else
        Serial.println(" pressed");
#endif
      if ((s & 0b10000000) == 0)
      {
        rawData[(s & 0b01111111)/8] |= (1 << ((s & 0b01111111)%8));
      }
      else
      {
        rawData[(s & 0b01111111)/8] &= ~(1 << ((s & 0b01111111)%8));
      }
    }
#ifndef SNIFFER
#ifndef DEBUG
  int checksum = 0;
  for(int i = 0 ; i < 16; i++)
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
#else
  for(int i = 0 ; i < 16; i++)
  {
    Serial.print(rawData[i]);
    Serial.print(" ");
  }  
  Serial.print("\n");
#endif
#endif
  }
}
