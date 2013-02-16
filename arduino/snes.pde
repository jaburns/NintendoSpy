/**************************************************************************
 * SnesSpy Arduino Sketch v1.0
 *------------------------------------------------------------
 * Written by Jeremy Burns
 */
 
#define LATCH_PIN 3
#define DATA_PIN  4

#define LATCH_QUERY (PIND & (1 << LATCH_PIN))
#define DATA_QUERY  (PIND & (1 << DATA_PIN ))

#define BIT_COUNT 16

unsigned char rawData[ BIT_COUNT ];
 
void setup()
{
  // Prepare the pins on port D for reading.
  digitalWrite( LATCH_PIN, LOW );
  digitalWrite( DATA_PIN,  LOW );
  pinMode( LATCH_PIN, INPUT );
  pinMode( DATA_PIN,  INPUT );
  
  Serial.begin( 115200 );
}

void loop()
{
  noInterrupts();
  getNextControllerState();
  interrupts();
  
  for (unsigned char i=0; i<BIT_COUNT; i++) {
    Serial.print(rawData[i] ? 1 : 0, DEC);
  }
  Serial.print(" \n");
}

// Waits for a controller poll from the SNES and returns when it has perceived a full controller state response.
// Stores the latest controller state in "rawData"
void getNextControllerState()
{ 
  // Wait for a falling edge on the latch pin.
  while( !LATCH_QUERY ) {}
  while(  LATCH_QUERY ) {}
    
  // Now we need to make 16 queries on the data pin with ~12us between each.
  
  unsigned char *rawDataPtr = rawData;
  unsigned char remainingBits = BIT_COUNT;
  
read_loop:
  
  // Read the data from the line and store in "rawData"
  *rawDataPtr = DATA_QUERY;
  ++rawDataPtr;
  if( --remainingBits == 0 ) return; 
  
  // Wait until the next button value is on the data line.
  asm volatile (
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
    "nop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\nnop\n"
  );
  goto read_loop;
}