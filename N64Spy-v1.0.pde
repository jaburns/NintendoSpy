/**************************************************************************
 * N64Spy Arduino Sketch v1.0
 *------------------------------------------------------------
 * Gamecube controller to Nintendo 64 adapter by Andrew Brown
 * Rewritten for N64 to HID by Peter Den Hartog
 * Hacked to read from live N64 controller by Jeremy Burns
 */

#define N64_PIN 2
#define N64_PIN_DIR DDRD
// these two macros set arduino pin 2 to input or output, which with an
// external 1K pull-up resistor to the 3.3V rail, is like pulling it high or
// low.  These operations translate to 1 op code, which takes 2 cycles
#define N64_HIGH DDRD &= ~0x04
#define N64_LOW DDRD |= 0x04
#define N64_QUERY (PIND & 0x04)
#define BIT_COUNT 32+8 // (One byte from N64, 4 bytes back from controller)

unsigned char N64_raw_dump[BIT_COUNT]; // 1 received bit per byte

void setup()
{
  digitalWrite(N64_PIN, LOW);  
  pinMode(N64_PIN, INPUT);
  Serial.begin(115200);
}

void N64_get()
{
    asm volatile (";Starting to listen");
    unsigned char timeout;
    unsigned char bitcount = BIT_COUNT;
    unsigned char *bitbin = N64_raw_dump;

	// Wait til the line is low for an iteration of this loop so we capture a message.
    #define SILENCE_WAIT_TIME 1
    unsigned char silenceTimer = SILENCE_WAIT_TIME;
    do {
        while(!N64_QUERY) { 
            if( silenceTimer > 0 ) silenceTimer--;
        }
        if( silenceTimer > 0 ) silenceTimer = SILENCE_WAIT_TIME;
    }
    while( silenceTimer );


    // Again, using gotos here to make the assembly more predictable and
    // optimization easier (please don't kill me)
read_loop:
    timeout = 0x3f;
    while (N64_QUERY) {
        if (!--timeout)
            return;
    }
    // wait approx 2us and poll the line
    asm volatile (
                  "nop\nnop\nnop\nnop\nnop\n"  
                  "nop\nnop\nnop\nnop\nnop\n"  
                  "nop\nnop\nnop\nnop\nnop\n"  
                  "nop\nnop\nnop\nnop\nnop\n"  
                  "nop\nnop\nnop\nnop\nnop\n"  
                  "nop\nnop\nnop\nnop\nnop\n"  
            );
    *bitbin = N64_QUERY;
    ++bitbin;
    --bitcount;
    if (bitcount == 0)
        return;

    // wait for line to go high again
    // it may already be high, so this should just drop through
    timeout = 0x3f;
    while (!N64_QUERY) {
        if (!--timeout)
            return;
    }
    goto read_loop;
}

void loop()
{
    int i;
    unsigned char data, addr;

    noInterrupts();
    N64_get();
    interrupts();

    for (i=8; i<BIT_COUNT; i++) {
       Serial.print(N64_raw_dump[i], DEC);
    }
    Serial.print(" \n");
}
