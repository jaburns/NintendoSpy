/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// RetroSpy Atari Paddles Firmware for Arduino
// v1.0
// RetroSpy written by zoggins

// --- These numbers will likely need modified ---

// **percent_change_required_to_move** is the percentage of change required to move the paddle display.  
// The lower the number the more jitter the higher the number the less jitter, 
// but you will get less fluid movement in the display.  This number needs to be between 0 and 100.

// **nominal_left_min/nominal_right_min** is the minimum value the left/right paddle is capable of hitting.  
// If the display is never hitting all the way left the value needs to be increased.  
// If its hitting left too soon it needs to be decreased.  
// The minimum value that can be selected is 0 and the maximum value should be less than **nominal_left_max/nominal_right_max**. 

// **nominal_left_max/nominal_right_max** is the maximum value the left/right paddle is capable of hitting.  
// If the display is never hitting all the way right the value needs to be decreased.  
// If its hitting right too soon it needs to be increased.  
// The maximum value that can be selected is 1023 and the minimum value should be more than **nominal_left_min/nominal_right_min**. 

int percent_change_required_to_move = 10;
int nominal_left_min = 213;
int nominal_right_min = 207;
int nominal_left_max = 1004;
int nominal_right_max = 1003;
float EMA_al = 1;      //initialization of EMA alpha left
float EMA_ar = 1;      //initialization of EMA alpha right
float numAvgValues = 5;
// ---------- Uncomment for debugging output --------------
//#define DEBUG

// PINOUTS
// Atari Pin 3 -> Digital Pin 4
// Atari Pin 4 -> Digital Pin 5
// Atari Pin 5 -> Analog Pin 0
// Atari Pin 8 -> Arduino GND
// Atari Pin 9 -> Analog Pin 1

#define PIN_READ( pin )  (PIND&(1<<(pin)))

int lastLeft = 0;
bool leftAscending = false;
int currentLeft = 0;

int lastRight = 0;
bool rightAscending = false;
int currentRight = 0;

float EMA_Sl = 0;          //initialization of EMA S left
float EMA_Sr = 0;          //initialization of EMA S right


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
  float mult_l = 2.0/(EMA_al + 1.0);
  float mult_r = 2.0/(EMA_ar + 1.0);
  EMA_Sl = (((float)leftPaddle) - EMA_Sl) * mult_l + EMA_Sl;
  EMA_Sr = (((float)rightPaddle) - EMA_Sr) * mult_r + EMA_Sr;       
  //EMA_Sl = (EMA_al*leftPaddle) + ((1-EMA_al)*EMA_Sl); 
  //EMA_Sr = (EMA_ar*rightPaddle) + ((1-EMA_ar)*EMA_Sr);
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
    Serial.write(percent_change_required_to_move + 11);
    Serial.write('\n');
#endif

}
