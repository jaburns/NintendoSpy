
RetroSpy Setup for Dreamcast
======

## Why Experimental?

Dreamcast controller support is experimental because it requires a newer faster microcontroller board and its just not as well tested as the existing Arduino hardware/software.

## Components and Equipment needed

1. [Teensy 3.6 with Pins](https://www.amazon.com/PJRC-Teensy-3-6-with-pins/dp/B01MG2PYYP/ref=sr_1_1?s=electronics&ie=UTF8&qid=1548799893&sr=1-1&keywords=teensy+3.6) (The code has been confirmed to also work with a Teensy 3.5 @ 168 Mhz)
3. [Breadboard](https://www.amazon.com/EL-CP-003-Breadboard-Solderless-Distribution-Connecting/dp/B01EV6LJ7G/ref=pd_lpo_vtph_21_bs_img_1?_encoding=UTF8&psc=1&refRID=NW19FZVRQYZDFQC900X00)
3. [USB cable](https://www.amazon.com/Staging-Product-Not-Retail-Sale/dp/B0741WGQ36/ref=sr_1_1_sspa?s=electronics&ie=UTF8&qid=1548799929&sr=1-1-spons&keywords=micro+USB+cable&psc=1) to connect the Arduino to your computer]
4. Wire cutters/strippers
5. (optional) Digital multimeter or a cheap continuity tester 
6. Controller extension cable
7. Wires to solder into the controller extension cable to go to the Teensy/Breadboard (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well)
8. Soldering iron and solder
9. Electrical tape and/or heat shrink tubing

## Software

1. [The latest Arduino software](http://arduino.cc/en/Main/Software)
2. [The latest Teensyduino software](https://www.pjrc.com/teensy/td_download.html)
3. Firmware for the Teensy
4. PC software to connect to the Teensy and display the controller

\#3 is included in the release package of RetroSpy.  The firmware is located in the ``experimenta\dreamcast`` folder and is called ``dreamcast.ino``.   Just run ``RetroSpy.exe`` to launch the display software.

## Instructions for making a Dreamcast RetroSpy cable

You will need to make a splice cable, and connect the controller lines to the listed digital pin on the Teensy. evilash25 made a very good [guide](https://github.com/zoggins/RetroSpy/blob/master/docs/guide-evilash25.md#wiring) for building splice cables.
The correct connections for Dreamcast are:

|   Dreamcast   | Teensy Digital Pin |
|:-------------:|:-------------------:|
|   Data pin 1  |          2          |
|      5V       |    Not Connected    |
|      GND      |    Not Connected    |
|     Sense     |    Not Connected    |
|   Data pin 5  |          14         |

## Software and Setup

Once the wiring is done, hook everything up to your game system and computer, now for the easy part.

1. Plug in the USB cable to your Teensy and PC.

2. Install the [latest Arduino software](http://arduino.cc/en/Main/Software), download the Windows Installer option.

3. Install the [latest Teensyduino software](https://www.pjrc.com/teensy/td_download.html), download the Windows Installer option.

4. Once installed, open the Arduino software, go to the Tools menu and set:

|   Option      |       Value         |
|:-------------:|:-------------------:|
|   Board       |   Teensy 3.6        |
|  USB Type     |    Serial           |
| CPU Speed     |    240 MHz (overclock)   |
|  Optimize     |    Fastest + pure-code with LTO    |

5. You should now see "Teensy 3.6, Serial, 240 MHz (overclock), Fastest + pure-code with LTO, US English on COMX" at the bottom right corner if everything is working. If not, you may need to restart and/or replug the USB connector.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/TeensyOnCom7.png)

6. Download and unzip the [latest release of RetroSpy](https://github.com/zoggins/RetroSpy/releases/latest) somewhere.

7. Select File->Open and open the ``dreamcast.ino`` file from the ``experimental/dreamcast`` folder of the unzipped RetroSpy release. 

8. Hit the upload button (right pointing arrow) located just under the 'Edit' menu, this will upload and run the software on the Teensy. If its your first time programming the Teensy it may ask you to push the button on the board in order to program it.  It should look like the following image. Once successfully uploaded, you won't have to upload software again to the Teensy again unless you want to change controller modes. 

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/upload.png)

9. Run ``RetroSpy.exe``.

10. The selection here should be pretty straightforward, select the 'COMX' port that the Teensy is on, select the controller you are using, select a skin, and hit 'Go'. If everything is hooked up correctly you should see your controller and inputs displaying.
