
RetroSpy
======

#### [Download the latest RetroSpy release here.](https://github.com/zoggins/RetroSpy/releases/latest)

A fork of [NintendoSpy](https://github.com/jaburns/NintendoSpy), RetroSpy is designed to present controller inputs from a console or computer in a display window.  This allows you to show your controller inputs for things like speedrunning, game tutorials, and more.  You can also convert controller presses into keystrokes to control programs on your computer such as LiveSplit and OBS.  RetroSpy supports the following systems and their regional equivalents:

 - Atari 2600
 - Intellivision
 - NES
 - Sega Master System
 - Commodore 64/128/+4/Vic-20
 - Commodore Amiga (non-CD32 controllers)
 - Sega Genesis
 - TurboGraphx-16
 - Neo-Geo 
 - SNES
 - 3DO
 - Sega Saturn
 - PlayStation
 - Nintendo 64
 - PlayStation 2
 - GameCube 

Additionally, RetroSpy has experimental support for:

 - [Atari Paddles](https://github.com/zoggins/RetroSpy/blob/master/docs/experimental/Paddles.md)
 - [Atari Driving Controllers](https://github.com/zoggins/RetroSpy/blob/master/docs/experimental/driving.md)
 - [Atari Keyboard, Kid's & Video Touch Pad Controllers](https://github.com/zoggins/RetroSpy/blob/master/docs/experimental/keyboard.md)
 - [CD-i Infrared Remotes & Wired Controllers](https://github.com/zoggins/RetroSpy/blob/master/docs/experimental/cdi.md)
 - [Dreamcast Controllers](https://github.com/zoggins/RetroSpy/blob/master/docs/experimental/dreamcast.md)
 - [Amiga Mice](https://github.com/zoggins/RetroSpy/blob/master/docs/experimental/amigamouse.md)
 - [Wii Extension Controllers](https://github.com/zoggins/RetroSpy/blob/master/docs/experimental/wii.md)
## Documentation

The rest of the README will explain how to get RetroSpy up and running on an Arduino Uno. If you want to run RetroSpy on a Teensy 3.5 continue [here](https://github.com/zoggins/RetroSpy/blob/master/docs/README-TEENSY.md) (this is required for Wii, Dreamcast and Amiga CD32). If you want to run RetroSpy for USB-based controllers go [here](https://github.com/zoggins/RetroSpy/wiki/RetroSpy-USB-based-Controller-Getting-Started).  For more specific tutorials, check out the [docs](https://github.com/zoggins/RetroSpy/blob/master/docs/) folder in the repository.

The general design of RetroSpy involves splicing a controller extension cable, and attaching the appropriate signal wires to an Arduino.  Then you just need to install the Arduino firmware packaged in the RetroSpy release, and run the display software.

## Components and Equipment needed for all types of cables 

1. [Arduino Uno](http://www.amazon.com/Arduino-UNO-board-DIP-ATmega328P/dp/B006H06TVG)  You might be able to find this cheaper elsewhere.  A clone such as [Funduino](https://www.foxytronics.com/products/265-funduino-uno-r3) works just as well.
2. [USB cable](http://www.amazon.com/AmazonBasics-Hi-Speed-A-Male-B-Male-Meters/dp/B001TH7GUA/) to connect the Arduino to your computer]
3. Wire cutters/strippers
4. (optional) Digital multimeter or a cheap continuity tester 
5. (optional) [Shield Stacking Header Set for Arduino UNO R3](https://www.amazon.com/ADAFRUIT-INDUSTRIES-85-STACKING-COMPATIBLE/dp/B00LB76EVU) 

## Components and Equipment needed for a Nintendo/Playstation/TG16/Saturn cable

1. Controller extension cable
2. Wires to solder into the controller extension cable to go to the Arduino (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well)
3. Soldering iron and solder
4. Electrical tape and/or heat shrink tubing

While the preceding list of equipment can be used for any system, the following systems have components that make the process easier.

## Components and Equipment needed for a DB9 (Atari, SMS, etc.) cable

1. Atari/Genesis Controller extension cable (a standard DB9 extension cable can be used, but you need to make sure it fits in your console)
2. [DB9 Male Breakout Board to Screw Terminals](https://www.amazon.com/gp/product/B00CLTP2O2/ref=oh_aui_detailpage_o00_s00?ie=UTF8&psc=1)
3. [DB9 Male to 2 Female Splitter](https://www.amazon.com/gp/product/B007F2E188/ref=oh_aui_detailpage_o08_s00?ie=UTF8&psc=1)
4. Wires to insert into the breakout board to go to the Arduino (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well. For the breakout board, you can cut off one end and expose the wire)


## Components and Equipment needed for a Neo-Geo cable

1. Neo-Geo Controller extension cable (a standard DB15 extension cable can be used, but you need to make sure it fits in your console)
2. [DB15 Male Breakout Board](https://www.amazon.com/DB15-Breakout-Connector-Pin-Male/dp/B073RGHNVD)
3. [DB15 Male to 2 Female Splitter](http://www.l-com.com/d-sub-db15-y-splitter-adapter-db15m-db15f-db15f)
4. Wires to insert into the breakout board to go to the Arduino (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well. For the breakout board, you can cut off one end and expose the wire)


## Software

1. [The latest Arduino software](http://arduino.cc/en/Main/Software)
2. Firmware for the Arduino
3. PC software to connect to the Arduino and display the controller

\#2 is included in the release package of RetroSpy.  The firmware is located in the ``firmware`` folder and is called ``firmware.ino``.   Just run ``RetroSpy.exe`` to launch the display software.

## Instructions for Nintendo/Playstation/TG16/Saturn cables

You will need to make a splice cable, and connect the controller lines to the listed digital pin in the Arduino. evilash25 made a very good [guide](https://github.com/zoggins/RetroSpy/blob/master/docs/guide-evilash25.md#wiring) for building splice cables, and can be applied to any of the following systems:

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

### Nintendo 64

|   N64   | Arduino Digital Pin |
|:-------:|:-------------------:|
|  3.3V   |    Not Connected    |
|  Data   |          2          |
|   GND   |         GND         |

### GameCube

|   GCN   | Arduino Digital Pin |
|:-------:|:-------------------:|
|   5V    |    Not Connected    |
|   Data  |          5          |
|   GND   |         GND         |
|   3.3V  |    Not Connected    |

Only 1 GND from the GCN controller is required.

### PlayStation 1/2

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

### TurboGraphx-16

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

## Instructions for Atari/SMS/Genesis/3DO/Intellivision cables

### Wiring

It is possible to solder jumper wires onto each wire of the extension cable, but since Atari/SMS/Genesis/3DO/Intellivision use a standard DB9 port, we can build a cable with no soldering required.

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

For 3DO controllers you will need to make the following connections:

| DB9 Pin | Arduino Digital Pin |
|:-------:|:-------------------:|
|    1    |    Not Connected    |
|    2    |    Not Connected    |
|    3    |    Not Connected    |
|    4    |    Not Connected    |
|    5    |    Not Connected    |
|    6    |          2          |
|    7    |          3          |
|    8    |    Not Connected    |
|    9    |          4          |

For Intellivision controllers you will need to make the following connections:

| DB9 Pin | Arduino Digital Pin |
|:-------:|:-------------------:|
|    1    |          2          |
|    2    |          3          |
|    3    |          4          |
|    4    |          5          |
|    5    |    Not Connected    |
|    6    |          7          |
|    7    |         10          |
|    8    |         11          |
|    9    |          8          |

For CD32 controllers you will need to make the following connections:

|  DB9 Pin | Arduino Digital Pin |
|:--------:|:-------------------:|
|     1    |          8          |
|     2    |          2          |
|     3    |          3          |
|     4    |  		  4          |
|     5    |          5          |
|     6    |          6          |
|     7    |                     |
|     8    |         GND         |
|     9    |          7          |

**NOTE/WARNING**:  GND has to be connected!  I also discovered that some controller extension cables do not handle GND to the Amiga's liking.  So, I had to plug the DB9 Y cable directly into the Amiga.

## Instructions for a Neo-Geo cable

### Wiring

It is possible to solder jumper wires onto each wire of the extension cable, but since Neo-Geo uses a standarded DB15 port we can build a cable with no soldering required.

1.  Take the Neo-Geo extension cable, wires, DB15 Breakout Board, DB15 Y adapter and optional headers you have acquired and put them together in this configuration (ignoring the fact that the cable pictured is actually an Atari cable):

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

1. Plug in the USB cable to your Arduino and PC.

2. Install the [latest Arduino software](http://arduino.cc/en/Main/Software), download the Windows Installer option.

3. Once installed, open the Arduino software, you should see "Arduino Uno on COMX" at the bottom right corner if everything is working. If not, you may need to restart and/or replug the USB connector.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/emptyide.png)

4. Download and unzip the [latest release of RetroSpy](https://github.com/zoggins/RetroSpy/releases/latest) somewhere.

5. Select File->Open and open the ``firmware.ino`` file from the firmware folder of the unzipped RetroSpy release.

6. Now uncomment the option for the operation mode (which controller) you will use. Note that `MODE_SNES` is uncommented. 

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/uncomment.png)

7. Hit the upload button (right pointing arrow) located just under the 'Edit' menu, this will upload and run the software on the Arduino. It should look like the following image. Once successfully uploaded, you won't have to upload software again to the Arduino again unless you want to change controller modes. 

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/upload.png)

8. Run ``RetroSpy.exe``.

9. The selection here should be pretty straightforward, select the 'COMX' port that the Arduino is on, select the controller you are using, select a skin, and hit 'Go'. If everything is hooked up correctly you should see your controller and inputs displaying.
