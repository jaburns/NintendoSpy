I wrote this guide to give step by step instructions on how to make your own controller input display (currently the same setup as siglemic). This setup currently works with NES, SNES, N64, and GCN controllers. You can check out what the final product looks like in some of [cylon13](http://www.twitch.tv/cylon13/profile)'s or [my video history](http://www.twitch.tv/evilash25/profile).

#### Components and Equipment needed

1. [Arduino Uno](http://www.amazon.com/Arduino-UNO-board-DIP-ATmega328P/dp/B006H06TVG). You might be able to find this cheaper elsewhere.  A clone such as [Funduino](https://www.foxytronics.com/products/265-funduino-uno-r3) works just as well.
2. [USB cable to connect the Arduino to your computer]((http://www.amazon.com/AmazonBasics-Hi-Speed-A-Male-B-Male-Meters/dp/B001TH7GUA/)
3. controller extension cable (NES, SNES, N64, or GCN)
4. (optional) male/female connectors with 5 pins minimum for easy controller switching
5. wires to solder into the controller extension cable to go to the Arduino (the Arduino sockets are very small, so you will need some smaller gauge wire to fit, so it might be best to pickup some wire after you see the socket size)
6. wire cutters/strippers
7. exacto knife or box cutters
8. soldering iron and solder
9. electrical tape
10. digital multimeter or a cheap continuity tester


#### Software

1. [the latest Arduino software](http://arduino.cc/en/Main/Software)
2. firmware to program into the Arduino with
3. PC software to connect to the Arduino and display the controller


\#2 and #3 above are included in the release package of NintendoSpy.  The firmware is located in the ``firmware`` folder and is called ``firmware.ino``.   Just run ``NintendoSpy.exe`` to launch the display software.


#### Instructions

##### Wiring

This is the most time consuming piece, especially if you have never done any wiring/soldering before.

1. First you will need to cut your controller extension cable so you can splice into the wires (consider which spot in the extension cable to cut, game system side, controller side, middle)

![](https://raw.githubusercontent.com/jaburns/NintendoSpy/master/docs/tutorial-images/cut.jpg)

2. Use your exacto knife or box cutters to very carefully cut away and peel back the plastic covering on both halves, about 2-3 inches should be good enough.

3. Use wire strippers to strip back about 1/2 an inch of the plastic covering on each wire. In my case I had to carefully use my exacto knife because the wires were too small for the stripper, I rotated the wire against the blade until I could pull the plastic off the end.

![](https://raw.githubusercontent.com/jaburns/NintendoSpy/master/docs/tutorial-images/stripped.jpg) 

4. Next you will need to use a digital multimeter or continuity tester to figure out which pin on your controller plugin goes to which wire in the extension cable, make sure to write down your findings. A quick google search of "<system> + controller pinout" should give you the information you need.

5. Here are the minimum pins we are interested in for each system:
 - NES - Latch, Data, and Clock
 - SNES - Latch, Data, and Clock
 - N64 - Data and Ground (there are only 3 wires, so it's pretty obvious here)
 - GCN - Data and (any non-shield)Ground

6. Figure out the length you need between your controller extension cable/Arduino and cut and strip a wire for each wire you are going to splice into.

7. Solder each wire back together with your spliced wires, here's what mine looked like when finished I soldered them this way (instead of end-to-end) because this will provide more strain relief against the small controller extension wires possibly breaking with use.

![](http://i.imgur.com/heGzrDe.jpg) 

8. After soldering everything back together, test out your extension cable with your game system to see that it still works before proceeding.

9. Use electrical tape to tape up each wire separately.

![](http://i.imgur.com/dOF7cG4.jpg)

10. Again use electrical tape to tape all the wires back together, make sure to tape all the way back up to where the extension cable covering starts

![](http://i.imgur.com/U3MjsdA.jpg).

11. (optional) Wire the spliced cables to a connector to make easy swapping between controllers, you will need the opposite gender connector, pins, and more wires to go to the Arduino.

12. Hook up the newly spliced extension cable to your Arduino according to this pinout.

![]()

##### Here is what mine looks like all hooked up

Note I am using a breadboard here to just jumper the wires over to the Arduino, it is not needed.

![](http://i.imgur.com/4Ew6CjM.jpg)
![](http://i.imgur.com/vimMORK.jpg)

/// http://i.imgur.com/KpmJnVX.jpg
/// http://i.imgur.com/RIUqaEp.jpg
/// http://i.imgur.com/54HtRdB.jpg
/// http://i.imgur.com/iYBCDOt.jpg

#### Software and Setup

Once the wiring is done, hook everything up to your game system and computer, now for the easy part.

1) Plug in the USB connector to your Arduino and PC.
2) Install the latest Arduino software (http://arduino.cc/en/Main/Software), download the Windows Installer option.
3) Once installed, open the Arduino software, you should see "Arduino Uno on COMX" at the bottom right corner if everything is working (http://imgur.com/a/oeUNp#6). If not, you may need to restart and/or replug the USB connector.
4) Select File->Open and open the firmware file provided here (https://www.dropbox.com/s/qvu87zou1iqriaa/firmware_new.ino).
5) Now uncomment the option for the operation mode (which controller) you will use (http://imgur.com/a/oeUNp#7). Note I am using a SNES controller here.
6) Hit the upload button (right pointing arrow) located just under the 'Edit' menu, this will upload and run the software on the Arduino. It should look like this (http://imgur.com/a/oeUNp#8). Once successfully uploaded, you won't have to upload software again to the Arduino again unless you want to change controller modes.
7) Open the NintendoSpy controller input display software provided here (https://github.com/jeremyaburns/NintendoSpy/blob/master/NintendoSpy.exe?raw=true).
8) The selection here should be pretty straightforward, select the 'COMX' port that the Arduino is on, select the controller you are using, and hit 'Go' (http://imgur.com/a/oeUNp#9). If everything is hooked up correctly you should see your controller and inputs displaying.
9) Celebrate with a cold drink and a playthrough of your favorite game :>


Special notes

1) I found that the N64 display will not work correctly if you have a memory pack plugged into the controller. I haven't tested, but possibly the rumble pack could do this as well.
2) I found that my NES controller didn't work with cylon13's original firmware, so I updated it with a few changes. In the future I might try to get this updated into the original project.
	- created a new function to read the Sift Register for NES/SNES controllers, this uses the added CLOCK pin
	- moved the GCN signal pin to share with the N64, there was no reason to have these separate (just a bonus)


Troubleshooting

1) The most common problem is likely to be wiring, so if the input display isn't working I would double check that
        a) The controller is working with your game system.
        b) The correct pins are wired between your extension harness to the correct pins on your Arduino.
        c) Your PC can see your Arduino on a COM port (you are able to upload the firmware.ino to your Arduino)

2) If you are having trouble getting the Arduino programming software working or getting it to see your Auduino on a COM port, try this guide (file:///C:/Program%20Files%20(x86)/Arduino/reference/Guide_Windows.html)

FAQ

Q: Will you make one of these for me? Even for money?
A: The short answer is no, I provided this guide to help others make the input display for themselves. I encourage you to try it, it was a fun little project.

Q: Will you add more controller support, add features, or change the looks of the controller display?
A: I didn't write the software myself, I used what was provided by the original author (https://github.com/jeremyaburns/NintendoSpy). That being said, I have made some of my own modifications to the software.  The look of the controllers are currently hard-coded into the display program, so a basic knowledge of Visual Studio is required.  Use Visual Studio Express 2012 to makes changes yourself (this software is free from MicroSoft)

Q: Why do you need an Arduino board for this to work? Can't you just use the split extension cable and wire the other end into a USB adapter for the PC?
A: These controllers require 2-way communication to work properly. For example, this means that a SNES sends data to the controller as well as the controller sending data back to the SNES. If you plugged in both the SNES and the USB adapter to a PC there would be (in effect) 2 SNES systems trying to talk with a single controller, and nothing would work properly. The reason that the Arduino works is that it merely 'listens' to the data going back and forth between the system and controller, it does no communication itself.

If you have any other questions about this setup, feel free to contact me on my Twitch channel (www.twitch.tv/evilash25) or on SRL IRC under the same nick. I will try to keep this guide up to date with more information and improvements.

Change Log
v1.0	- Initial document
v1.1 	- I could not get the NES controller to work with cylon13's firmware for the Arduino, so I created my own version, which adds reliability to both NES and SNES controllers by using the Clock Pin to properly time incoming controller data.
	- Added wiring for the CLOCK pins of NES and SNES controllers
	- Updated images to include the CLOCK Pins
	- Added my own firmware.ino release to use the CLOCK Pins of NES/SNES controllers
