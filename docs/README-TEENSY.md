
RetroSpy for Teensy 3.5
======

#### [Download the latest RetroSpy release here.](https://github.com/zoggins/RetroSpy/releases/latest)

A fork of [NintendoSpy](https://github.com/jaburns/NintendoSpy), RetroSpy is designed to present controller inputs from a console or computer in a display window.  This allows you to show your controller inputs for things like speedrunning, game tutorials, and more.  You can also convert controller presses into keystrokes to control programs on your computer such as LiveSplit and OBS.  RetroSpy supports the following systems and their regional equivalents:

 - NES
 - SNES
 - Nintendo 64
 - GameCube 
 - Sega Dreamcast
 
## Documentation

The rest of the README will explain how to get RetroSpy up and running. For more specific tutorials, check out the [docs](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs) folder in the repository.

The general design of RetroSpy involves splicing a controller extension cable, and attaching the appropriate signal wires to a Teensy.  Then you just need to install the Teensy firmware packaged in the RetroSpy release, and run the display software.

## Components and Equipment needed for all types of cables 

1. [Teensy 3.5 with Pins](https://www.amazon.com/PJRC-Teensy-3-5-with-pins/dp/B072MZW2KZ/ref=sr_1_cc_1?s=aps&ie=UTF8&qid=1550771504&sr=1-1-catcorr&keywords=teensy+3.5+pins)
3. [Breadboard](https://www.amazon.com/EL-CP-003-Breadboard-Solderless-Distribution-Connecting/dp/B01EV6LJ7G/ref=pd_lpo_vtph_21_bs_img_1?_encoding=UTF8&psc=1&refRID=NW19FZVRQYZDFQC900X00)
3. [USB cable](https://www.amazon.com/Staging-Product-Not-Retail-Sale/dp/B0741WGQ36/ref=sr_1_1_sspa?s=electronics&ie=UTF8&qid=1548799929&sr=1-1-spons&keywords=micro+USB+cable&psc=1) to connect the Arduino to your computer]
4. Wire cutters/strippers
5. (optional) Digital multimeter or a cheap continuity tester 

## Components and Equipment needed for a Nintendo/Dreamcast cable

1. Controller extension cable
2. Wires to solder into the controller extension cable to go to the Arduino (Male to Male Dupont wires, such as [these](https://www.newegg.com/Product/Product.aspx?Item=9SIABKS5R54282&ignorebbr=1&nm_mc=KNC-GoogleMKP-PC&cm_mmc=KNC-GoogleMKP-PC-_-pla-New+Ocean+Tech-_-Gadgets-_-9SIABKS5R54282&gclid=Cj0KCQiAi57gBRDqARIsABhDSMpuM-JL8VWplLwJAD_A3pZrJ0GYVSMUcdcLZrZELpDAdR4VpBIDVyYaApR_EALw_wcB&gclsrc=aw.ds) work very well)
3. Soldering iron and solder
4. Electrical tape and/or heat shrink tubing

## Software

1. [The latest Arduino software](http://arduino.cc/en/Main/Software)
2. [The latest Teensyduino software](https://www.pjrc.com/teensy/td_download.html)
3. Firmware for the Teensy
4. PC software to connect to the Teensy and display the controller

\#3 is included in the release package of RetroSpy.  The firmware is located in the ``firmware-teensy`` folder and is called ``firmware-teensy.ino``.   Just run ``RetroSpy.exe`` to launch the display software.

## Instructions for Nintendo/Dreamcast cables

You will need to make a splice cable, and connect the controller lines to the listed digital pin in the Arduino. evilash25 made a very good [guide](https://github.com/zoggins/RetroSpy/blob/master/docs/guide-evilash25.md#wiring) for building splice cables, and can be applied to any of the following systems:

###	NES

|   NES   | Teensy Digital Pin |
|:-------:|:-------------------:|
|   GND   |    Not Connected    |
|  Clock  |          21          |
|  Latch  |          8          |
|   Data  |          6          |
|   5V    |    Not Connected    |
|  Zp.LS  |    Not Connected    |
|  Zp.TP  |    Not Connected    |

### SNES

|  SNES   | Teensy Digital Pin |
|:-------:|:-------------------:|
|    5V   |    Not Connected    |
|  Clock  |          21          |
|  Latch  |          8          |
|   Data  |          6          |
|   GND   |    Not Connected    |

### Nintendo 64

|   N64   | Teensy Digital Pin |
|:-------:|:-------------------:|
|  3.3V   |    Not Connected    |
|  Data   |          7          |
|   GND   |         GND         |

### GameCube

|   GCN   | Teensy Digital Pin |
|:-------:|:-------------------:|
|   5V    |    Not Connected    |
|   Data  |          20          |
|   GND   |         GND         |
|   3.3V  |    Not Connected    |

Only 1 GND from the GCN controller is required.

### Dreamcast

|   Dreamcast   | Teensy Digital Pin |
|:-------------:|:-------------------:|
|   Data pin 1  |          2          |
|      5V       |    Not Connected    |
|      GND      |    Not Connected    |
|     Sense     |    Not Connected    |
|   Data pin 5  |          14         |


## Software and Setup

Once the wiring is done, hook everything up to your game system and computer, now for the easy part.

Once the wiring is done, hook everything up to your game system and computer, now for the easy part.

1. Plug in the USB cable to your Teensy and PC.

2. Install the [latest Arduino software](http://arduino.cc/en/Main/Software), download the Windows Installer option.

3. Install the [latest Teensyduino software](https://www.pjrc.com/teensy/td_download.html), download the Windows Installer option.

4. Once installed, open the Arduino software, go to the Tools menu and set:

|   Option      |       Value         |
|:-------------:|:-------------------:|
|   Board       |   Teensy 3.5        |
|  USB Type     |    Serial           |
| CPU Speed     |    168 MHz (overclock)   |
|  Optimize     |    Fastest + pure-code with LTO    |

5. You should now see "Teensy 3.5, Serial, 168 MHz (overclock), Fastest + pure-code with LTO, US English on COMX" at the bottom right corner if everything is working. If not, you may need to restart and/or replug the USB connector.

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/TeensyOnCom7.png)

6. Download and unzip the [latest release of RetroSpy](https://github.com/zoggins/RetroSpy/releases/latest) somewhere.

7. Select File->Open and open the ``firmware-teensy.ino`` file from the firmware-teensy folder of the unzipped RetroSpy release.

8. Now uncomment the option for the operation mode (which controller) you will use. Note that `MODE_SNES` is uncommented. 

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/uncomment.png)

9. Hit the upload button (right pointing arrow) located just under the 'Edit' menu, this will upload and run the software on the Teensy. You will very likely need to push the button on the board in order to program it.  It should look like the following image. Once successfully uploaded, you won't have to upload software again to the Teensy again unless you want to change controller modes. 

![](https://raw.githubusercontent.com/zoggins/RetroSpy/master/docs/tutorial-images/readme_images/upload.png)

10. Run ``RetroSpy.exe``.

11. The selection here should be pretty straightforward, select the 'COMX' port that the Teensy is on, select the controller you are using, select a skin, and hit 'Go'. If everything is hooked up correctly you should see your controller and inputs displaying.
