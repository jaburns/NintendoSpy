
RetroSpy
======

#### [Download the latest RetroSpy release here.](https://github.com/zoggins/RetroSpy/releases/latest)

This is a fork of [NintendoSpy](https://github.com/jaburns/NintendoSpy).  NintendoSpy provides a general solution for live-streaming your controller inputs while speedrunning, or recording inputs for tutorials on how to perform tricks.  It supports tying in to NES, SNES, Nintendo 64, and GameCube controller signals to get a live view of them, as well as any gamepad connected to your PC for use with emulators.  XBox 360 controllers are supported with a skin out of the box, but other gamepads will require creating a skin.  My fork allows for the support of Atari/Commodore joysticks, Sega Genesis controllers, SMS controllers, the Atari 2600 Omega Race Booster Grip, Turbographx-16/PC Engine controllers, NeoGeo, Sega Saturn (both standard and 3D controllers) and PSX/PS2 controllers.  Additionally, it adds full support for the Super Gameboy SNES cartridge and the Gamecube Gameboy Player, which were not correctly supported in the original NintendoSpy.

The following documentation is in addition to the original NintendoSpy documentation found [here](https://github.com/zoggins/RetroSpy/blob/master/README-ORIG.md).  These instructions borrow heavily from evilash25's awesome NintendoSpy build guide, which can be found [here](https://github.com/zoggins/RetroSpy/blob/master/docs/guide-evilash25.md#wiring).

## Documentation

The general design of RetroSpy involves splicing the controller wire, and attaching the appropriate signal wires to an Arduino.  Then you just need to install the Arduino firmware packaged in the RetroSpy release, and run the viewer software.

## Components and Equipment needed for all types of cables 

1. [Arduino Uno](http://www.amazon.com/Arduino-UNO-board-DIP-ATmega328P/dp/B006H06TVG). You might be able to find this cheaper elsewhere.  A clone such as [Funduino](https://www.foxytronics.com/products/265-funduino-uno-r3) works just as well.
2. [USB cable to connect the Arduino to your computer](http://www.amazon.com/AmazonBasics-Hi-Speed-A-Male-B-Male-Meters/dp/B001TH7GUA/)

## Components and Equipment needed for a Nintendo/Playstation/TG16&PC-E/Saturn cable

1. Controller extension cable
2. Wires to solder into the controller extension cable to go to the Arduino (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well)
3. Wire cutters/strippers
4. Soldering iron and solder
5. Electrical tape
6. Digital multimeter or a cheap continuity tester 
7. (optional) [Shield Stacking Header Set for Arduino UNO R3](https://www.amazon.com/ADAFRUIT-INDUSTRIES-85-STACKING-COMPATIBLE/dp/B00LB76EVU)

While the preceding list of equipment can be used for any system, the following systems have components that make the process easier.

## Components and Equipment needed for a Genesis/SMS/Atari cable

1. Atari/Genesis Controller extension cable (a standard DB9 extension cable can be used, but you need to make sure it will fit in your console)
2. [DB9 Male Breakout Board to Screw Terminals](https://www.amazon.com/gp/product/B00CLTP2O2/ref=oh_aui_detailpage_o00_s00?ie=UTF8&psc=1)
3. [DB9 Male to 2 Female Splitter](https://www.amazon.com/gp/product/B007F2E188/ref=oh_aui_detailpage_o08_s00?ie=UTF8&psc=1)
4. Wires to insert into the breakout board to go to the Arduino (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well)
5. Wire cutters/strippers
6. Digital multimeter or a cheap continuity tester 
7. (optional) [Shield Stacking Header Set for Arduino UNO R3](https://www.amazon.com/ADAFRUIT-INDUSTRIES-85-STACKING-COMPATIBLE/dp/B00LB76EVU)

## Components and Equipment needed for a NeoGeo cable

1. NeoGeo Controller extension cable or (a standard DB15 extension cable can be used, but you need to make sure it will fit in your console)
2. [DB15 Male Breakout Board](https://www.amazon.com/DB15-Breakout-Connector-Pin-Male/dp/B073RGHNVD)
3. [DB15 Male to 2 Female Splitter](http://www.l-com.com/d-sub-db15-y-splitter-adapter-db15m-db15f-db15f)
4. Wires to insert into the breakout board to go to the Arduino (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well)
5. Wire cutters/strippers
6. Digital multimeter or a cheap continuity tester 
7. (optional) [Shield Stacking Header Set for Arduino UNO R3](https://www.amazon.com/ADAFRUIT-INDUSTRIES-85-STACKING-COMPATIBLE/dp/B00LB76EVU) 


## Software

1. [The latest Arduino software](http://arduino.cc/en/Main/Software)
2. [ClassicController Arduino Library](https://github.com/zoggins/ClassicControllerSpy/releases/latest)
3. Firmware for the Arduino
4. PC software to connect to the Arduino and display the controller

\#3 and #4 above are included in the release package of RetroSpy.  The firmware is located in the ``firmware`` folder and is called ``firmware.ino``.   Just run ``RetroSpy.exe`` to launch the display software.

## Instructions for Nintendo/Playstation/TG16&PC-E/Saturn cable

evilash25 made a very good [guide](https://github.com/zoggins/RetroSpy/blob/master/docs/guide-evilash25.md#wiring) for building splice cables, and can be applied to any of the following systems.

###	NES

|   NES   | Arduino Digital Pin |
|:-------:|:-------------------:|
|   GND   |    Not Connected    |
|  Clock  |          6          |
|  Latch  |          3          |
|   Data  |          4          |
|   5V    |    Not Connected    |
|  Zp.LS  |    Not Connected    |
|  Zp.TP  |    Not Connected    |

### SNES

|  SNES   | Arduino Digital Pin |
|:-------:|:-------------------:|
|    5V   |    Not Connected    |
|  Clock  |          6          |
|  Latch  |          3          |
|   Data  |          4          |
|   GND   |    Not Connected    |

### N64

|   N64   | Arduino Digital Pin |
|:-------:|:-------------------:|
|  3.3V   |    Not Connected    |
|  Data   |          2          |
|   GND   |         GND         |

### Gamecube

|   GCN   | Arduino Digital Pin |
|:-------:|:-------------------:|
|   5V    |    Not Connected    |
|   Data  |          5          |
|   GND   |         GND         |
|   3.3V  |    Not Connected    |

Only 1 GND from the GCN controller is required.

### Playstation 1/2

|  PSX/2  | Arduino Digital Pin |
|:-------:|:-------------------:|
|    1    |          6          |
|    2    |          5          |
|    3    |    Not Connected    |
|    4    |  GND/Not Connected  |
|    5    |    Not Connected    |
|    6    |          2          |
|    7    |          3          |
|    8    |    Not Connected    |
|    9    |          4          |

**NOTE/WARNING**:  PIN 4 usually does not need to be connected, but controllers have been encountered that will behave oddly without it connected to the Arduino's GND.

### Turbographx-16/PC Engine

|  TG16   | Arduino Digital Pin |
|:-------:|:-------------------:|
|    1    |    Not Connected    |
|    2    |          2          |
|    3    |          3          |
|    4    |          4          |
|    5    |          5          |
|    6    |          6          |
|    7    |    Not Connected    |
|    8    |    Not Connected    |

### Sega Saturn

|  Saturn   | Arduino Digital Pin |
|:---------:|:-------------------:|
|    1      |    Not Connected    |
|    2      |          3          |
|    3      |          2          |
|    4      |          6          |
|    5      |          7          |
|    6      |          8          |
|    7      |          5          |
|    8      |          4          |
|    9      |    Not Connected    |

## Instructions for Genesis/SMS/Atari cable

### Wiring

It is possible to simply solder 9 jumper wires onto each wire of the Atari extension cable and be done, but since Genesis/SMS/Atari/etc use a standarded DB9 port we can build a cable with no soldering required.

1.  Take the Atari extension cable, wires, DB9 Breakout Board, DB9 Y cable and optional headers you have acquired and put them together in this configuration:

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/ataricable.jpg)

### Hardware Setup

For Sega Genesis controllers you will need to make the following connections:

| DB9 Pin | Arduino Digital Pin |
|:-------:|:-------------------:|
|    1    |          2          |
|    2    |          3          |
|    3    |          4          |
|    4    |          5          |
|    5    |    Not Connected    |
|    6    |          6          |
|    7    |          8          |
|    8    |    Not Connected    |
|    9    |          7          |

For Atari Joysticks, Sega Master System controllers and the Atari Omega Race Booster Grip you will need to make the following connections:

| DB9 Pin | Arduino Digital Pin |
|:-------:|:-------------------:|
|    1    |          2          |
|    2    |          3          |
|    3    |          4          |
|    4    |          5          |
|    5    |          6          |
|    6    |          7          |
|    7    |    Not Connected    |
|    8    |    Not Connected    |
|    9    |          8          |

## Instructions for NeoGeo

### Wiring

It is possible to simply solder 10 jumper wires onto each wire of the NeoGeo extension cable and be done, but since NeoGeo uses a standarded DB15 port we can build a cable with no soldering required.

1.  Take the NeoGeo extension cable, wires, DB15 Breakout Board, DB15 Y adapter and optional headers you have acquired and put them together in this configuration (ignoring the fact that the cable pictured is actually an Atari cable):

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/ataricable.jpg)

### Hardware Setup

You will need to make the following connections:

| DB15 Pin | Arduino Digital Pin |
|:--------:|:-------------------:|
|    1     |    Not Connected    |
|    2     |    Not Connected    |
|    3     |          2          |
|    4     |          3          |
|    5     |          4          |
|    6     |          5          |
|    7     |          6          |
|    8     |    Not Connected    |
|    9     |    Not Connected    |
|    10    |    Not Connected    |
|    11    |          7          |
|    12    |          8          |
|    13    |          9          |
|    14    |          10         |
|    15    |          11         |

## Software and Setup

Once the wiring is done, hook everything up to your game system and computer, now for the easy part.

1. Plug in the USB connector to your Arduino and PC.

2. Install the [latest Arduino software](http://arduino.cc/en/Main/Software), download the Windows Installer option.

3. Once installed, open the Arduino software, you should see "Arduino Uno on COMX" at the bottom right corner if everything is working. If not, you may need to restart and/or replug the USB connector.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/arduinooncom3.jpg)

4. Install my Arduino ClassicControllerSpy library.  Installation directions are [here](https://github.com/zoggins/ClassicControllerSpy#installation).

4. Download and unzip the [latest release of RetroSpy somewhere](https://github.com/zoggins/RetroSpy/releases/latest).

5. Select File->Open and open the ``firmware.ino`` file from the firmware folder of the unzipped RetroSpy release.

6. Now uncomment the option for the operation mode (which controller) you will use. Note I am using a SNES controller here.  MODE_SEGA is for Genesis & MODE_CLASSIC is for SMS/Atari.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/uncomment_mode.jpg)

7. Hit the upload button (right pointing arrow) located just under the 'Edit' menu, this will upload and run the software on the Arduino. It should look like the following image. Once successfully uploaded, you won't have to upload software again to the Arduino again unless you want to change controller modes.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/doneuploading.jpg)

8. Run ``RetroSpy.exe``.

9. The selection here should be pretty straightforward, select the 'COMX' port that the Arduino is on, select the controller you are using, select a skin, and hit 'Go'. If everything is hooked up correctly you should see your controller and inputs displaying.
