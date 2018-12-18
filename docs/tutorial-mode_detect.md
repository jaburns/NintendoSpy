MODE_DETECT tutorial
======
This is a quick tutorial on using MODE_DETECT with the Arduino firmware for RetroSpy. This allows you to define analog pins on the Arduino as modes instead of having to uncomment modes and reupload the firmware every time you want to switch consoles.

## Required equipment

In addition to the normal set of equipment needed for RetroSpy, all you need is a single wire. I prefer to use Male to Male Dupont wires, as they fit perfectly into the Arduino's headers. 

## Setting up the firmware

Open up RetroSpy's firmware in the Arduino IDE, and scroll down just past the list of modes to uncomment, and you will see a few defined variables called ``MODEPIN``. 

![](https://raw.githubusercontent.com/sk84uhlivin/RetroSpy/readme/docs/tutorial-images/mode_detect-tutorial/MODEPIN.png)

This will be our list of defined analog pins. In the example above, analog pin 0 has been defined as ``MODEPIN_SNES``. This means that our intention is, if analog pin 0 is grounded, the firmware will initialize the SNES mode. The name of the variable doesn't matter, but to avoid confusion, you should name it something related to the mode you want to activate. The Arduino UNO has 6 analog pins, so you may define pins 0-5.

Now that we have defined our analog pins, now we just need to setup our ``MODE_DETECT`` loop. Scroll down to the very bottom of the firmware until you see the ``MODE_DETECT`` loop. It should be the very last loop of the firmware.

![](https://github.com/sk84uhlivin/RetroSpy/blob/readme/docs/tutorial-images/mode_detect-tutorial/MODE_DETECT.png?raw=true)

In the ``MODE_DETECT`` loop, let's look at the syntax of the ``if`` and ``else if`` statements:
```
if( !PINC_READ( MODEPIN_SNES ) ) {
        loop_SNES();
  }		
```	
This means that if ``MODEPIN_SNES`` is grounded, which we defined as A0, then the ``loop_SNES`` loop initializes. The following ``else if`` statements will be the rest of the analog pins you defined earlier. Then finally, the ``else`` statement is the mode that is initialized if no analog pins are grounded, allowing a total of 7 possible modes to be used in the manner.

Now that our ``MODE_DETECT`` loop is setup, uncomment ``MODE_DETECT`` at the top, and upload your firmware. The firmware will now run the mode based on which, if any analog pins are grounded. Just connect one end of your wire in a GND pin on the POWER section of the board, and the other end to your desired analog pin. Note that upon switching analog pins, you will need to reset the board.

![](https://raw.githubusercontent.com/sk84uhlivin/RetroSpy/readme/docs/tutorial-images/mode_detect-tutorial/UNCOMMENT.png)
![](https://raw.githubusercontent.com/sk84uhlivin/RetroSpy/readme/docs/tutorial-images/mode_detect-tutorial/WIRE.jpg)
