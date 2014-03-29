GameCube Tutorial
======

### Things You’ll Need
* An [Arduino Uno](http://arduino.cc/en/Main/ArduinoBoardUno) or any ATMega328-based Arduino at 16 MHz.
  * With USB-A to USB-B cable (Printer cable)
* Preferably a [GameCube controller extension cable](http://www.amazon.com/s/ref=nb_sb_ss_i_0_24?url=search-alias%3Daps&field-keywords=gamecube+extension+cable&sprefix=gamecube+extension+cable%2Caps%2C217&rh=i%3Aaps%2Ck%3Agamecube+extension+cable). If not, you will need to splice the wire from your GameCube Controller.
* Soldering equipment, i.e.
  * Soldering Iron
  * Solder (60/40, 63/37; very thin gauge)
  * Thin gauge wire, bought or salvaged.
  * Optional: Pin Headers
  * Electrical tape (!) Don’t skimp on this! Regular tape will burn if a circuit shorts out on you. Use electrical tape only!

### Controller Basics
A Gamecube controller cable is made up of 6 wires:
* 5v Power for Rumble
* 3.43v Logic Supply
* Bi-Directional Data Line
* 3 separate Grounds, one of which is the shielding.

We only need the Bi-Directional Data Line to read the GameCube inputs and a ground for safety. You may use any number of grounds if you wish.

(!) Note: Not all extension cables use the same colour schematic. This may lead to improperly spliced lines. You may refer to this [Google Doc](https://docs.google.com/spreadsheet/ccc?key=0AiANfINZ0c74dEZpc2RETjVoUG5iVEVQRHFWYkx6c2c&usp=drive_web#gid=0) for more info on known colour schemes. If you happen to find a new scheme, please email the details to thestarknebula@gmail.com. We will cover how to find the pinouts for any unknown extensions.
Modding the Extension

### Modding the Extension

![alt text]( "")
We are going to splice (join at the ends) the wires near the female end of cable. This will allow for the most flexibility when using NintendoSpy. If you don’t understand why, it’s okay. Just follow along. For a GameCube controller, we’d advise cutting near the Male end, with a 6-12 feet of wire available.

### Cut the Extension

![alt text]( "")
It should look something like this. You may cut the extension closer or farther to the end if you wish. Again, for splicing GameCube Controllers, cut near the Male end.

### Unsheathe the Cables

![alt text]( "")
Carefully cut open the black sheathing to reveal the inner wires. Un-sheath the ends of the wires as well. The colour scheme you see is that of a knock-off brand from Hong Kong. In this case, white is our Data and black is the shielding Ground. 
Refer to this Google Doc to identify your colour scheme.
Report new/unknown schemes to thestarknebula@gmail.com.

### Identifying the Pinouts

![alt text]( "")
If you open up the cover over the female end, you can inspect the pinouts from behind. The casing can be taped or glued back afterwards. The pinout diagram is available in the [Google Doc](https://docs.google.com/spreadsheet/ccc?key=0AiANfINZ0c74dEZpc2RETjVoUG5iVEVQRHFWYkx6c2c&usp=drive_web#gid=0) or at [int03](http://www.int03.co.uk/crema/hardware/gamecube/gc-control.html).

### Reconnect the Unused Lines

![alt text]( "")
Reconnect the unused lines back together. You won’t need to connect any other wire to these.

### Solder the Data and Ground to the Wires

![alt text]( "")
Connect your Data and Ground to a wire. A good length for it is 6 to 12 feet (2 to 4 meters.) Tip: Solder both original ends back together, then twist the new wire around the tined ending. This will make it easy to solder on the wire.

### Optional: Tin or Connect a Pin Header

![alt text]( "")
If you bought a good gauge wire, you may not need to do either. However, if you salvaged wire from something else, you may need to. This end must make proper contact with the Arduino if it is to work. Two options you may do are either tin the ends (left) so that it is sturdy enough to be pushed into the socket, or attach a pin header to it (right.)

### Tape the Solder Joints

![alt text]( "")
![alt text]( "")
Tape the joint with electrical tape to prevent any damage, shock, interference, etc. Use only electrical tape; anything else may not withstand the electricity and burn up.

### Connect Your Setup

![alt text]( "")
Your setup should then look something like this. The male end goes to the GameCube, the Female end attaches to your GameCube controller, and the spliced wire connects to the Arduino.
(!) Make sure that your Arduino’s firmware is loaded and the GameCube line uncommented!

![alt text]( "")
Happy Streaming! :)

