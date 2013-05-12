DIY Tutorial for N64 Viewer Hardware
======

### Things you'll need
* An [Arduino Uno](http://arduino.cc/en/Main/ArduinoBoardUno). (really any ATMega328-based Arduino at 16 MHz)
* An [N64 controller extension cable](http://www.amazon.com/s/?field-keywords=n64%20extension%20cable). (unless you want to cut in to your favorite controller)
* Soldering equipment including some spare wire.
* Electrical tape.

### Theory of Operation

The N64 controller cable is made up of 3 wires: power, signal, and ground.  The N64 and the controller communicate back and forth alternating their use of
the signal wire.  The N64 makes a request for the current controller state, memory card data, rumble pack state, etc, and then waits for the controller to respond
on the the same signal line.  What this Arduino sketch does is monitor the signal line without interfering with the N64-controller communication at all.  It
waits until it perceives a possible controller read from the N64, and then starts reading data off the signal line until it knows it's stored a full controller
read worth of data.  This data is then checked to make sure the signal was, in fact, a controller state read, and then it is sent through the Arduino's
USB to serial COM port interface to a PC for display.

### Splicing the Extension Cable

![alt text](https://github.com/jeremyaburns/NintendoSpy/raw/master/tutorial-images/cut.jpg "")
![alt text](https://github.com/jeremyaburns/NintendoSpy/raw/master/tutorial-images/stripped.jpg "")
![alt text](https://github.com/jeremyaburns/NintendoSpy/raw/master/tutorial-images/solder.jpg "")
![alt text](https://github.com/jeremyaburns/NintendoSpy/raw/master/tutorial-images/spliced.jpg "")
![alt text](https://github.com/jeremyaburns/NintendoSpy/raw/master/tutorial-images/tapedup.jpg "")

### Configuring the Arduino

First you'll need to load [this sketch](https://raw.github.com/jeremyaburns/NintendoSpy/master/NintendoSpy-v1.0.pde) on to your Arduino. 
If you're not familiar with how to do this then check out the [Getting Started](http://arduino.cc/en/Guide/HomePage) guide over at Arduino.
Once you've got the NintendoSpy sketch uploaded to the board, attach the controller cable to the Arduino as follows.  (If you're using a different
Arduino, just make sure that N64 GND is connected to any Arduino GND and that N64 Signal is connected to digital pin 2)

![alt text](https://github.com/jeremyaburns/NintendoSpy/raw/master/tutorial-images/wiring.jpg "")

With the program loaded and the wires connected, you can close the Arduino environment. The Arduino is now programmed to act as an NintendoSpy
until another program is loaded on to it.  You can disconnect/reconnect the device from USB at will since the program is loaded in to non-volatile
memory.  Simply run the display software linked at the top of the page while the Arduino's plugged in, and you should be good to go.