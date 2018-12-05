/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Atari Paddles Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// ---------- Uncomment for debugging output --------------
//#define DEBUG

// PINOUTS
// Atari Pin 3 -> Digital Pin 4
// Atari Pin 4 -> Digital Pin 5
// Atari Pin 5 -> Analog Pin 0
// Atari Pin 8 -> Arduino GND
// Atari Pin 9 -> Analog Pin 1

#define PIN_READ( pin )  (PIND&(1<<(pin)))

// The below values are not scientific. These will need to be tuned for different sets of paddles.
float EMA_al = .08;      //initialization of EMA alpha left
float EMA_ar = .08;      //initialization of EMA alpha right
int nominal_left_min = 87;
int nominal_right_min = 110;
int nominal_left_max = 1007;
int nominal_right_max = 1009;
int left_min_threshold = 5;
int right_min_threshold = 5;
int left_max_threshold = 253;
int right_max_threshold = 253;

int EMA_Sl = 0;          //initialization of EMA S left
int EMA_Sr = 0;          //initialization of EMA S right

static int ScaleInteger(float oldValue, float oldMin, float oldMax, float newMin, float newMax, float minThreshold, float maxThreshold)
{
  float newValue = ((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin) + newMin;
  if (newValue > maxThreshold)
    return newMax;
  if (newValue < minThreshold)
    return newMin;

  return newValue;
}

void setup() {

  for (int i = 2; i <= 8; ++i)
    pinMode(i, INPUT_PULLUP);

  EMA_Sl = analogRead(0);
  EMA_Sr = analogRead(1);

  Serial.begin( 115200 );
}

void loop() {

  noInterrupts();
  byte pins = 0;
  pins |= (PIND >> 2);
  int leftPaddle = analogRead(0);
  int rightPaddle = analogRead(1);
  interrupts();

  byte fire1 = ((pins & 0b0000000000000100) == 0);
  byte fire2 = ((pins & 0b0000000000001000) == 0);
  EMA_Sl = (EMA_al*leftPaddle) + ((1-EMA_al)*EMA_Sl); 
  EMA_Sr = (EMA_ar*rightPaddle) + ((1-EMA_ar)*EMA_Sr);

#ifdef DEBUG
    Serial.print(fire1 ? "3" : "-");
    Serial.print(fire2 ? "4" : "-");
    Serial.print("|");
    Serial.print(EMA_Sl);
    Serial.print("|");
    Serial.print(ScaleInteger(EMA_Sl, nominal_left_min, nominal_left_max, 0, 255, left_min_threshold, left_max_threshold));
    Serial.print("|");
    Serial.print(EMA_Sr);
    Serial.print("|");
    Serial.print(ScaleInteger(EMA_Sr, nominal_right_min, nominal_right_max, 0, 255, right_min_threshold, right_max_threshold));
    Serial.print("\n");
#else
    Serial.write(fire1);
    Serial.write(fire2);
    Serial.write(ScaleInteger(EMA_Sl, nominal_left_min, nominal_left_max, 0, 255, left_min_threshold, left_max_threshold));
    Serial.write(ScaleInteger(EMA_Sr, nominal_right_min, nominal_right_max, 0, 255, right_min_threshold, right_max_threshold));
    Serial.write('\n');
#endif

  delay(2);
}
