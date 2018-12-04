/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Atari Paddles Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// ---------- Uncomment for debugging output --------------
//#define DEBUG

// PINOUTS
// Atari Pin 3 -> Digital Pin 2
// Atari Pin 4 -> Digital Pin 3
// Atari Pin 5 -> Analog Pin 0
// Atari Pin 9 -> Analog Pin 1

#define PIN_READ( pin )  (PIND&(1<<(pin)))

// The below values are not scientific, but they seem to work.  These may need to be tuned for different systems.
float EMA_al = 0.6;      //initialization of EMA alpha left
float EMA_ar = 0.6;      //initialization of EMA alpha right

int EMA_Sr = 0;          //initialization of EMA S right
int EMA_Sl = 0;          //initialization of EMA S left

void setup() {

  for (int i = 2; i <= 8; ++i)
    pinMode(i, INPUT_PULLUP);

  EMA_Sl = analogRead(0);
  EMA_Sr = analogRead(1);

  Serial.begin( 115200 );
}

void loop() {

  byte rawData[6];

  noInterrupts();
  byte pins = 0;
  pins |= (PIND >> 2);
  int leftPaddle = analogRead(0);
  int rightPaddle = analogRead(1);
  interrupts();

  rawData[0] = ((pins & 0b0000000000000100) == 0);
  rawData[1] = ((pins & 0b0000000000001000) == 0);
  rawData[2] = EMA_Sl = (int)((EMA_al*leftPaddle) + ((1-EMA_al)*EMA_Sl)); 
  rawData[4] = EMA_Sr = (int)((EMA_ar*rightPaddle) + ((1-EMA_ar)*EMA_Sr)); 
  
#ifdef DEBUG
    Serial.print(rawData[0] ? "3" : "-");
    Serial.print(rawData[1] ? "4" : "-");
    Serial.print("|");
    Serial.print(rawData[2]);
    Serial.print("|");
    Serial.print(rawData[4]);
    Serial.print("\n");
#else
    Serial.print(rawData[0]);
    Serial.print(rawData[1]);
    Serial.print((word)rawData[2]);
    Serial.print((word)rawData[4]);
    Serial.print("\n");
#endif
}
