// ---------- Uncomment one of these options to select operation mode --------------
//#define MODE_GC
//#define MODE_N64
//#define MODE_SNES
//#define MODE_NES
//#define MODE_GENESIS
//#define MODE_SMS_ON_GENESIS   // For using a genesis retrospy cable and the genesis reader in the exe while playing SMS games.
//#define MODE_GENESIS_MOUSE
//#define MODE_SMS
//#define MODE_BOOSTER_GRIP
//#define MODE_PLAYSTATION
//#define MODE_TG16
//#define MODE_SATURN
//#define MODE_SATURN3D
//#define MODE_NEOGEO
//#define MODE_3DO
//#define MODE_INTELLIVISION
//#define MODE_JAGUAR
//#define MODE_FMTOWNS
//#define MODE_PCFX
//Bridge one of the analog GND to the right analog IN to enable your selected mode
//#define MODE_DETECT
// ---------------------------------------------------------------------------------
// The only reason you'd want to use 2-wire SNES mode is if you built a NintendoSpy
// before the 3-wire firmware was implemented.  This mode is for backwards
// compatibility only. 
//#define MODE_2WIRE_SNES
// ---------------------------------------------------------------------------------
// Uncomment this for serial debugging output
//#define DEBUG

#define MODEPIN_SNES 0
#define MODEPIN_N64  1
#define MODEPIN_GC   2

#define N64_PIN        2

#define SNES_LATCH           3
#define SNES_DATA            4
#define SNES_CLOCK           6
#define NES_DATA0            2
#define NES_DATA1            5

#define GC_PIN        5
#define GC_PREFIX    25

#define ThreeDO_LATCH      2
#define ThreeDO_DATA       4
#define ThreeDO_CLOCK      3   

#define PCFX_LATCH        3
#define PCFX_CLOCK        4
#define PCFX_DATA         5

#define SS_SELECT0 6
#define SS_SEL 6
#define SS_SELECT1 7
#define SS_REQ 7
#define SS_ACK 0 // 8 - 0 on PORTB 
#define SS_DATA0   2
#define SS_DATA1   3
#define SS_DATA2   4
#define SS_DATA3   5

#define TG_SELECT 6
#define TG_DATA1  2
#define TG_DATA2  3
#define TG_DATA3  4
#define TG_DATA4  5

#define PS_ATT 2
#define PS_CLOCK 3
#define PS_ACK 4
#define PS_CMD 5
#define PS_DATA 6

// PORTD
#define NEOGEO_SELECT 2
#define NEOGEO_D 3
#define NEOGEO_B 4
#define NEOGEO_RIGHT 5
#define NEOGEO_DOWN 6
#define NEOGEO_START 7
//PORTB
#define NEOGEO_C 0
#define NEOGEO_A 1
#define NEOGEO_LEFT 2
#define NEOGEO_UP 3

//PORT D PINS
#define INTPIN1 2  // Digital Pin 2
#define INTPIN2 3  // Digital Pin 3
#define INTPIN3 4  // Digital Pin 4
#define INTPIN4 5  // Digital Pin 5
#define INTPIN5 NA // Not Connected
#define INTPIN6 7  // Digital Pin 7
// PORT B PINS
#define INTPIN7 2  // Digital Pin 10
#define INTPIN8 3  // Digital Pin 11
#define INTPIN9 0  // Digital Pin 8

// The raw output of an Intellivision controller when
// multiple buttons is pressed is very strange and 
// no likely what you want to see, but set this to false
// if you do want the strange behavior
#define INT_SANE_BEHAVIOR true

#define AJ_COLUMN1   8
#define AJ_COLUMN2   9
#define AJ_COLUMN3  10
#define AJ_COLUMN4  11

