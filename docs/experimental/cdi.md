
RetroSpy Setup for CD-i Wired and Wireless Controllers
======

## Why Experimental?

CD-i controller support is experimental because it requires additional hardware and libraries to support the wireless controllers.

## Components needed for both wireless and wired controllers

1. [Arduino Uno](http://www.amazon.com/Arduino-UNO-board-DIP-ATmega328P/dp/B006H06TVG)  You might be able to find this cheaper elsewhere.  A clone such as [Funduino](https://www.foxytronics.com/products/265-funduino-uno-r3) works just as well.
2. [USB cable](http://www.amazon.com/AmazonBasics-Hi-Speed-A-Male-B-Male-Meters/dp/B001TH7GUA/) to connect the Arduino to your computer]

## Components and Equipment needed wireless controllers

1. An IR Receiver tuned to 38kHz, I used a [TSOP38238](https://www.adafruit.com/product/157)
2. Wires to connect the IR receiver to the Ardiuno

## Components and Equipment needed for wired controllers

1. Wire cutters/strippers
2. (optional) Digital multimeter or a cheap continuity tester 
3. [8-Pin Mini-DIN Serial Cable](https://www.amazon.com/C2G-02318-Mini-DIN-Serial-Meters/dp/B0002GWN84)
4. [8-Pin Mini-DIN Pass-through Breakout Board](https://elabbay.myshopify.com/collections/camera/products/mdin8-f-f-v2a-mini-din-8-female-to-female-pass-through-adapter-breakout-board-elabguy)
5. Wires to insert into the breakout board to go to the Arduino (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well. For the breakout board, you can cut off one end and expose the wire)

## Software

1. [The latest Arduino software](http://arduino.cc/en/Main/Software)
2. Firmware for the Arduino
3. cyborg5's IRLib2 Ardunio library, found [here](https://github.com/cyborg5/IRLib2)
4. PC software to connect to the Arduino and display the controller

\#2 is included in the release package of RetroSpy.  The firmware is located in the ``experimental/cdi`` folder and is called ``cdi.ino``.   Just run ``RetroSpy.exe`` to launch the display software.

### Hardware Setup

### Wireless

1. Connect the data pin of the IR Receiver to Arduino Pin 2.  The IR receiver will also likely need 5V and GND, so connect accordingly.

### Wired

1.  Connect the controller to one end of the breakout board and the 8-Pin Mini-DIN Serial cable to both the CD-i and the other end of the breakout board.
2.  Connect CD-i controller Pin 2 to Arduino Pin 9.  **NOTE**: The breakout board I used had the pins labeled wrong.  Pin 2 was actually marked as Pin 8, so you may need to verify which pin is which.

## Software and Setup

Once the wiring is done, hook everything up to your game system and computer, now for the easy part.

1. Plug in the USB cable to your Arduino and PC.

2. Install the [latest Arduino software](http://arduino.cc/en/Main/Software), download the Windows Installer option.

3. Once installed, open the Arduino software, you should see "Arduino Uno on COMX" at the bottom right corner if everything is working. If not, you may need to restart and/or replug the USB connector.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/emptyide.png)

4. Install cyborg5's IRLib2 library.  Installation directions are [here](https://github.com/cyborg5/IRLib2).

5. Download and unzip the [latest release of RetroSpy](https://github.com/zoggins/RetroSpy/releases/latest) somewhere.

6. Select File->Open and open the ``cdi.ino`` file from the ``experimental/cdi`` folder of the unzipped RetroSpy release.

7. Hit the upload button (right pointing arrow) located just under the 'Edit' menu, this will upload and run the software on the Arduino. It should look like the following image. Once successfully uploaded, you won't have to upload software again to the Arduino again unless you want to change controller modes. 

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/upload.png)

8. Run ``RetroSpy.exe``.

9. The selection here should be pretty straightforward, select the 'COMX' port that the Arduino is on, select the controller you are using, select a skin, and hit 'Go'. If everything is hooked up correctly you should see your controller and inputs displaying.
