
RetroSpy Setup for Atari Paddles
======

# Why Experimental?

Atari paddle support is experimental because the current output sucks.  The technical details of how Atari handles the paddles is beyond the scope of this document, but implementation causes the output to jitter pretty badly. I have tried to do some amount of smoothing, but the jitter still exists.  Additionally, the parameters for the smoothing code are very likely specific paddle dependent.

# What You Need
1. [Arduino Uno](http://www.amazon.com/Arduino-UNO-board-DIP-ATmega328P/dp/B006H06TVG)
2. [USB cable](http://www.amazon.com/AmazonBasics-Hi-Speed-A-Male-B-Male-Meters/dp/B001TH7GUA/) to connect the Arduino to your computer
3. [Atari/Genesis Controller extension cable](https://www.amazon.com/Retro-Bit-Sega-Genesis-Controller-Extension-Cable/dp/B0089ZLXHW)
4. [DB9 Male Breakout Board to Screw Terminals](https://www.amazon.com/gp/product/B00CLTP2O2/ref=oh_aui_detailpage_o00_s00?ie=UTF8&psc=1)
5. [DB9 Male to 2 Female Splitter](https://www.amazon.com/gp/product/B007F2E188/ref=oh_aui_detailpage_o08_s00?ie=UTF8&psc=1)
6. Wires connect to the breakout board, like [these](https://www.amazon.com/Elegoo-EL-CP-004-Multicolored-Breadboard-arduino/dp/B01EV70C78) 

## Software

1. [The latest Arduino software](http://arduino.cc/en/Main/Software)
2. Firmware for the Arduino
3. PC software to connect to the Arduino and display the controller

#2 and #3 above are included in the release package of RetroSpy. The firmware is located in the ``experimental/Paddles`` folder and is called ``Paddles.ino``.   Just run ``RetroSpy.exe`` to launch the display software.

# Instructions

## Wiring

It is possible to solder jumper wires onto each wire of the extension cable like some other platforms, but since Atari joysticks use a standard DB9 port, we can build a cable with no soldering required.

## Hooking it up

1.  Take the Atari extension cable, wires, DB9 Breakout Board, DB9 Y cable and optional headers you have acquired and put them together in this configuration:

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/ataricable.jpg)

2. Hook up the breakout board to to your Arduino according to this pinout.

| DB9 Pin | Arduino Pin |
|:-------:|:-------------------:|
|    1    |    Not Connected    |
|    2    |    Not Connected    |
|    3    |          Digital 4          |
|    4    |          Digital 5          |
|    5    |       Analog 0      |
|    6    |    Not Connected    |
|    7    |    Not Connected    |
|    8    |         GND         |
|    9    |       Analog 1      |

3. Plug the USB cable into the Arduino.

4. Plug the other end of the USB cable into the display PC.

5. Power on your console.

## Software and Setup

Once the wiring is done and everything is hooked up to your game system and computer, now for the easy part.

1. Plug in the USB cable to your Arduino and PC.

2. Install the [latest Arduino software](http://arduino.cc/en/Main/Software), download the Windows Installer option.

3. Once installed, open the Arduino software, you should see "Arduino Uno on COMX" at the bottom right corner if everything is working. If not, you may need to restart and/or replug the USB connector.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/emptyide.png)

4. Download and unzip the [latest release of RetroSpy](https://github.com/zoggins/RetroSpy/releases/latest) somewhere.

5. Select File->Open and open the ``Paddles.ino`` file from the ``experimental/Paddles folder`` of the unzipped RetroSpy release.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/uncomment.png)

6. Hit the upload button (right pointing arrow) located just under the 'Edit' menu, this will upload and run the software on the Arduino. It should look like the following image. Once successfully uploaded, you won't have to upload software again to the Arduino again unless you want to change controller modes. 

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/upload.png)

7. Run ``RetroSpy.exe``.

8. The selection here should be pretty straightforward, select the 'COMX' port that the Arduino is on, select the controller you are using, select a skin, and hit 'Go'. If everything is hooked up correctly you should see your controller and inputs displaying.
