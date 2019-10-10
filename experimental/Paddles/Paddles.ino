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
float EMA_al = 1;      //initialization of EMA alpha left
float EMA_ar = 1;      //initialization of EMA alpha right
int nominal_left_min = 213;
int nominal_right_min = 207;
int nominal_left_max = 1004;
int nominal_right_max = 1003;

int lastLeft = 0;
bool leftAscending = false;
int currentLeft = 0;

int lastRight = 0;
bool rightAscending = false;
int currentRight = 0;

int EMA_Sl = 0;          //initialization of EMA S left
int EMA_Sr = 0;          //initialization of EMA S right


static int ScaleInteger(float oldValue, float oldMin, float oldMax, float newMin, float newMax)
{
  float newValue = ((oldValue - oldMin) * (newMax - newMin)) / (oldMax - oldMin) + newMin;
  if (newValue > newMax)
    return newMax;
  if (newValue < newMin)
    return newMin;

  return newValue;
}

void setup() {

  for (int i = 2; i <= 8; ++i)
    pinMode(i, INPUT_PULLUP);

  EMA_Sl = currentLeft = analogRead(0);
  EMA_Sr = currentRight = analogRead(1);

  Serial.begin( 115200 );
}

void loop() {


  noInterrupts();
  byte pins = 0;
  pins |= (PIND >> 2);
  int leftPaddle = analogRead(0);
  int rightPaddle = analogRead(1);
  interrupts();

  if (leftAscending && leftPaddle < lastLeft)
  {
    leftAscending = false;
    currentLeft = lastLeft;
    lastLeft = leftPaddle;
    leftPaddle = currentLeft;
  }
  else if (!leftAscending && leftPaddle > lastLeft)
  {
    leftAscending = true;
    lastLeft = leftPaddle;
    leftPaddle = currentLeft;
  }
  else
  {
    lastLeft = leftPaddle;
    leftPaddle = currentLeft;
  }

  if (rightAscending && rightPaddle < lastRight)
  {
    rightAscending = false;
    currentRight = lastRight;
    lastRight = rightPaddle;
    rightPaddle = currentRight;
  }
  else if (!rightAscending && leftPaddle > lastLeft)
  {
    rightAscending = true;
    lastRight = rightPaddle;
    rightPaddle = currentRight;
  }
  else
  {
    lastRight = rightPaddle;
    rightPaddle = currentRight;
  }
      
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
    Serial.print(ScaleInteger(EMA_Sl, nominal_left_min, nominal_left_max, 0, 255));
    Serial.print("|");
    Serial.print(EMA_Sr);
    Serial.print("|");
    Serial.print(ScaleInteger(EMA_Sr, nominal_right_min, nominal_right_max, 0, 255));
    Serial.print("\n");
#else
    int sil = ScaleInteger(EMA_Sl, nominal_left_min, nominal_left_max, 0, 255);
    int sir = ScaleInteger(EMA_Sr, nominal_right_min, nominal_right_max, 0, 255);
    Serial.write(fire1);
    Serial.write(fire2);
    Serial.write(sil);
    Serial.write(sir);
    Serial.write(5);
    Serial.write('\n');
#endif

}
