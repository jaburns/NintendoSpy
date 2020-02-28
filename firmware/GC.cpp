#include "common.h"
#include "GC.h"

bool seenGC2N64 = false;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Performs a read cycle from one of Nintendo's one-wire interface based controllers.
//     pin  = Pin index on Port D where the data wire is attached.
//     bits = Number of bits to read from the line.
template< unsigned char pin >
void read_GC( unsigned char bits )
{
    unsigned char *rawDataPtr = rawData;

read_loop:

    // Wait for the line to go high then low.
    WAIT_FALLING_EDGE( pin );

    // Wait ~2us between line reads
    
    asm volatile( MICROSECOND_NOPS MICROSECOND_NOPS );

    // Read a bit from the line and store as a byte in "rawData"
    *rawDataPtr = PIN_READ(pin);
    ++rawDataPtr;
    if( --bits == 0 ) return;

    goto read_loop;
}

inline bool checkPrefixGBA()
{
    if( rawData[0] != 0 ) return false; // 0
    if( rawData[1] != 0 ) return false; // 0
    if( rawData[2] != 0 ) return false; // 0
    if( rawData[3] == 0 ) return false; // 1
    if( rawData[4] != 0 ) return false; // 0
    if( rawData[5] == 0 ) return false; // 1
    if( rawData[6] != 0 ) return false; // 0
    if( rawData[7] != 0 ) return false; // 0
    if( rawData[8] == 0 ) return false; // 1
    if( rawData[9] != 0 ) return false; // 0
    if( rawData[10] != 0 ) return false; // 0
    if( rawData[11] == 0 ) return false; // 1
    seenGC2N64 = false;
    return true;
}

inline void sendRawGBAData()
{
  #ifndef DEBUG
  Serial.write(ZERO );
  Serial.write(ZERO );
  Serial.write(ZERO );
  Serial.write(rawData[21] ? ONE : ZERO);
  Serial.write(ZERO );
  Serial.write(ZERO );
  Serial.write(rawData[23] ? ONE : ZERO);
  Serial.write(rawData[24] ? ONE : ZERO);
  Serial.write(ZERO );
  Serial.write(rawData[31] ? ONE : ZERO);
  Serial.write(rawData[32] ? ONE : ZERO);
  Serial.write(rawData[22] ? ONE : ZERO);
  Serial.write(rawData[18] ? ONE : ZERO);
  Serial.write(rawData[17] ? ONE : ZERO);
  Serial.write(rawData[20] ? ONE : ZERO);
  Serial.write(rawData[19] ? ONE : ZERO);
  Serial.write(ONE);
  for(int i = 0; i < 7; ++i)
    Serial.write(ZERO );
  Serial.write(ONE);
  for(int i = 0; i < 7; ++i)
    Serial.write(ZERO );
  Serial.write(ONE);
  for(int i = 0; i < 7; ++i)
    Serial.write(ZERO );
  Serial.write(ONE);
  for(int i = 0; i < 7; ++i)
    Serial.write(ZERO );
  for(int i = 0; i < 8; ++i)
    Serial.write(ZERO );
  for(int i = 0; i < 8; ++i)
    Serial.write(ZERO );
  Serial.write( SPLIT );
  #else
  Serial.print("0");
  Serial.print("0");
  Serial.print("0");
  Serial.print(rawData[21] ? "t" : "0");
  Serial.print("0" );
  Serial.print("0" );
  Serial.print(rawData[23] ? "b" : "0");
  Serial.print(rawData[24] ? "a" : "0");
  Serial.print("0" );
  Serial.print(rawData[31] ? "L": "0");
  Serial.print(rawData[32] ? "R": "0");
  Serial.print(rawData[22] ? "s" : "0");
  Serial.print(rawData[18] ? "u" : "0");
  Serial.print(rawData[17] ? "d" : "0");
  Serial.print(rawData[20] ? "l" : "0");
  Serial.print(rawData[19] ? "r" : "0");
  Serial.print(128);
  Serial.print(128);
  Serial.print(128);
  Serial.print(128);
  Serial.print(0);
  Serial.print(0);
  Serial.print( "\n" );
  #endif
}

inline bool checkPrefixGC ()
{
    if( rawData[0] != 0 ) return false; // 0
    if( rawData[1] == 0 ) return false; // 1
    if( rawData[2] != 0 ) return false; // 0
    if( rawData[3] != 0 ) return false; // 0
    if( rawData[4] != 0 ) return false; // 0
    if( rawData[5] != 0 ) return false; // 0
    if( rawData[6] != 0 ) return false; // 0
    if( rawData[7] != 0 ) return false; // 0
    if( rawData[8] != 0 ) return false; // 0
    if( rawData[9] != 0 ) return false; // 0
    if( rawData[10] != 0 ) return false; // 0
    if( rawData[11] != 0 ) return false; // 0
    if( rawData[12] != 0 ) return false; // 0
    if( rawData[13] != 0 ) return false; // 0
    if( rawData[14] == 0 ) return false; // 1
    if( rawData[15] == 0 ) return false; // 1
    if( rawData[16] != 0 ) return false; // 0
    if( rawData[17] != 0 ) return false; // 0
    if( rawData[18] != 0 ) return false; // 0
    if( rawData[19] != 0 ) return false; // 0
    if( rawData[20] != 0 ) return false; // 0
    if( rawData[21] != 0 ) return false; // 0
    //if( rawData[22] != 0 ) return false; // 0 or 1
    if( rawData[23] != 0 ) return false; // 0
    if( rawData[24] == 0 ) return false; // 1
    seenGC2N64 = false;
    return true;
}

inline bool checkBothGCPrefixOnRaphnet()
{
    if( rawData[0] != 0 ) return false; // 0
    if( rawData[1] != 0 ) return false; // 0
    if( rawData[2] != 0 ) return false; // 0
    if( rawData[3] != 0 ) return false; // 0
    if( rawData[4] != 0 ) return false; // 0
    if( rawData[5] != 0 ) return false; // 0
    if( rawData[6] != 0 ) return false; // 0
    if( rawData[7] != 0 ) return false; // 0
    if( rawData[8] == 0 ) return false; // 1
    if( rawData[9] != 0 ) return false; // 0
    if( rawData[10] != 0 ) return false; // 0
    if( rawData[11] != 0 ) return false; // 0
    if( rawData[12] != 0 ) return false; // 0
    if( rawData[13] == 0 ) return false; // 1
    if( rawData[14] != 0 ) return false; // 0
    if( rawData[15] != 0 ) return false; // 0
    if( rawData[16] == 0 ) return false; // 1
    if( rawData[17] != 0 ) return false; // 0
    if( rawData[18] != 0 ) return false; // 0
    if( rawData[19] != 0 ) return false; // 0
    if( rawData[20] != 0 ) return false; // 0
    if( rawData[21] != 0 ) return false; // 0
    if( rawData[22] != 0 ) return false; // 0
    if( rawData[23] != 0 ) return false; // 0
    if( rawData[24] != 0 ) return false; // 0
    if( rawData[25] != 0 ) return false; // 0 
    //if( rawData[26] != 0 ) return false; // 0 or 1
    if( rawData[27] == 0 ) return false; // 1
    if( rawData[28] != 0 ) return false; // 0
    if( rawData[29] != 0 ) return false; // 0
    if( rawData[30] != 0 ) return false; // 0
    if( rawData[31] == 0 ) return false; // 1
    if( rawData[32] == 0 ) return false; // 1
    if( rawData[33] != 0 ) return false; // 0
    if( rawData[34] != 0 ) return false; // 0
    if( rawData[35] == 0 ) return false; // 1
    if( rawData[36] != 0 ) return false; // 0
    if( rawData[37] != 0 ) return false; // 0
    if( rawData[38] != 0 ) return false; // 0
    if( rawData[39] != 0 ) return false; // 0
    if( rawData[40] != 0 ) return false; // 0
    if( rawData[41] != 0 ) return false; // 0
    if( rawData[42] != 0 ) return false; // 0
    if( rawData[43] != 0 ) return false; // 0
    if( rawData[44] != 0 ) return false; // 0
    if( rawData[45] != 0 ) return false; // 0
    if( rawData[46] != 0 ) return false; // 0
    if( rawData[47] != 0 ) return false; // 0
    if( rawData[48] == 0 ) return false; // 1
    if( rawData[49] == 0 ) return false; // 1
    if( rawData[50] != 0 ) return false; // 0
    if( rawData[51] != 0 ) return false; // 0
    if( rawData[52] != 0 ) return false; // 0
    if( rawData[53] != 0 ) return false; // 0
    if( rawData[54] != 0 ) return false; // 0
    if( rawData[55] != 0 ) return false; // 0
    if( rawData[56] != 0 ) return false; // 0 
    if( rawData[57] != 0 ) return false; // 0
    if( rawData[58] == 0 ) return false; // 1
    seenGC2N64 = true;
    return true;
}

inline void loop_GC()
{
    if (!seenGC2N64)
    {
      noInterrupts();
      read_GC< GC_PIN >( GC_PREFIX + GC_BITCOUNT );
      interrupts();
      if (checkPrefixGC()) {
        sendRawData( GC_PREFIX, GC_BITCOUNT );
      }
      else if (checkPrefixGBA() ) {
        sendRawGBAData();
      }
      else if (checkBothGCPrefixOnRaphnet()) {
         // Sets seenGC2N64RaphnetAdapter to true
      }
    }
    else
    {
      noInterrupts();
      read_GC< GC_PIN >( 34+GC_PREFIX + GC_BITCOUNT );
      interrupts();
      if (checkBothGCPrefixOnRaphnet())
        sendRawData(34+GC_PREFIX, GC_BITCOUNT);
      else if (checkPrefixGC())
        sendRawData( GC_PREFIX, GC_BITCOUNT );
      else if (checkPrefixGBA() )
        sendRawGBAData();
    }
}
