RetroSpy
======

#### [Download the latest RetroSpy release here.](https://github.com/zoggins/RetroSpy/releases/latest)

This is a fork of [NintendoSpy](https://github.com/jaburns/NintendoSpy).  NintendoSpy provides a general solution for live-streaming your controller inputs while speedrunning, or recording inputs for tutorials on how to perform tricks.  It supports tying in to NES, SNES, Nintendo 64, and GameCube controller signals to get a live view of them, as well as any gamepad connected to your PC for use with emulators.  XBox 360 controllers are supported with a skin out of the box, but other gamepads will require creating a skin.  My fork allows for the support of Atari/Commodore joysticks, Sega Genesis controllers, SMS controllers, the Atari 2600 Omega Race Booster Grip, Turbographx-16/PC Engine controllers and PSX controllers.  Additionally, it adds full support for the Super Gameboy SNES cartridge, which wasn't correctly supported in the original NintendoSpy.

The following documentation is in addition to the original NintendoSpy documentation found [here](https://github.com/zoggins/RetroSpy/blob/master/README-ORIG.md).  These instructions borrow heavily from evilash25's awesome NintendoSpy build guide, which can be found [here](https://github.com/zoggins/RetroSpy/blob/master/docs/guide-evilash25.md#wiring).

## Documentation

The general design of RetroSpy involves splicing the controller wire, and attaching the appropriate signal wires to an Arduino.  Then you just need to install the Arduino firmware packaged in the RetroSpy release, and run the viewer software.

## Components and Equipment needed for all types of cables 

1. [Arduino Uno](http://www.amazon.com/Arduino-UNO-board-DIP-ATmega328P/dp/B006H06TVG). You might be able to find this cheaper elsewhere.  A clone such as [Funduino](https://www.foxytronics.com/products/265-funduino-uno-r3) works just as well.
2. [USB cable to connect the Arduino to your computer](http://www.amazon.com/AmazonBasics-Hi-Speed-A-Male-B-Male-Meters/dp/B001TH7GUA/)

## Specific Components and Equipment needed for a Nintendo cable

1. Controller extension cable (NES, SNES, N64, or GCN)
2. (optional) male/female connectors with 5 pins minimum for easy controller switching
3. Wires to solder into the controller extension cable to go to the Arduino (the Arduino sockets are very small, so you will need some smaller gauge wire to fit, so it might be best to pickup some wire after you see the socket size)
4. Wire cutters/strippers
5. Exacto knife or box cutters
6. Soldering iron and solder
7. Electrical tape
8. Digital multimeter or a cheap continuity tester

## Specific Components and Equipment needed for a Genesis/SMS/Atari cable

1. Atari Controller extension cable
2. [Wires](https://www.amazon.com/gp/product/B06XRV92ZB/ref=oh_aui_detailpage_o07_s00?ie=UTF8&psc=1)
3. [DB9 Male to 2 Female Splitter Cable](https://www.amazon.com/gp/product/B007F2E188/ref=oh_aui_detailpage_o08_s00?ie=UTF8&psc=1)
4. [DB9 Male Breakout Board to Screw Terminals](https://www.amazon.com/gp/product/B00CLTP2O2/ref=oh_aui_detailpage_o00_s00?ie=UTF8&psc=1)
5. (optional) [Shield Stacking Header Set for Arduino UNO R3](https://www.amazon.com/gp/product/B0756KRCFX/ref=oh_aui_detailpage_o06_s00?ie=UTF8&psc=1)
5. Wire cutters/strippers
## Specific Components and Equipment needed for a PSX, Turbographx-16/PC Engine or Sega Saturn cable

1. Controller extension cable (PSX, TG16/PC Engine or Sega Saturn)
2. (optional) male/female connectors with 5 pins minimum for easy controller switching
3. Wires to solder into the controller extension cable to go to the Arduino (the Arduino sockets are very small, so you will need some smaller gauge wire to fit, so it might be best to pickup some wire after you see the socket size)
4. Wire cutters/strippers
5. Exacto knife or box cutters
6. Soldering iron and solder
7. Electrical tape
8. Digital multimeter or a cheap continuity tester


## Software

1. [the latest Arduino software](http://arduino.cc/en/Main/Software)
2. [ClassicController Arduino Library](https://github.com/zoggins/ClassicControllerSpy/releases/latest)
2. Firmware for the Arduino
3. PC software to connect to the Arduino and display the controller

\#3 and #4 above are included in the release package of RetroSpy.  The firmware is located in the ``firmware`` folder and is called ``firmware.ino``.   Just run ``RetroSpy.exe`` to launch the display software.

## Instructions for NES, SNES, N64 or GameCube

For building a NES, SNES, N64 or GameCube cable follow the steps found [here](https://github.com/zoggins/RetroSpy/blob/master/docs/guide-evilash25.md#wiring).

## Instructions for PSX, Turbographx-16/PC Engine or Sega Saturn

The building of PSX, Turbographx-16 or Sega Saturn cable is very similar to building a Nintendo cable, so for the most part follow the steps found [here](https://github.com/zoggins/RetroSpy/blob/master/docs/guide-evilash25.md#wiring).  The pinout of the cables is as follows.

### PSX

**NOTE/WARNING**:  PIN 4 usually does not need to be connected, but I have encountered controllers that will behave oddly without it connect to the Arduino's GND.

|   PSX   | Arduino Digital Pin |
|:-------:|:-------------------:|
|    1    |          6          |
|    2    |          5          |
|    3    |    Not Connected    |
|    4    |    Not Connected or GND   |
|    5    |    Not Connected    |
|    6    |          2          |
|    7    |          3          |
|    8    |    Not Connected    |
|    9    |          4          |

### Turbografx-16/PC Engine

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
|    2      |          2          |
|    3      |          3          |
|    4      |          6          |
|    5      |          7          |
|    6      |          8          |
|    7      |          4          |
|    8      |          5          |
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
