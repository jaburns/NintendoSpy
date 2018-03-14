RetroSpy
======

#### [Download the latest RetroSpy release here.](https://github.com/zoggins/RetroSpy/releases/latest) (x64 and experimental x86 Windows binary)

This is a fork of [NintendoSpy](https://github.com/jaburns/NintendoSpy).  NintendoSpy provides a general solution for live-streaming your controller inputs while speedrunning, or recording inputs for tutorials on how to perform tricks.  It supports tying in to NES, SNES, Nintendo 64, and GameCube controller signals to get a live view of them, as well as any gamepad connected to your PC for use with emulators.  XBox 360 controllers are supported with a skin out of the box, but other gamepads will require creating a skin.  My fork allows for the support of Atari/Commodore joysticks, Sega Genesis controllers, SMS controllers and the Atari 2600 Omega Race Booster Grip.

The following documentation is in addition to the original NintendoSpy documentation found [here](https://github.com/jaburns/NintendoSpy/blob/master/README-ORIG.md).

## Documentation

### Wiring and hardware

The general design of NintendoSpy involves splicing the controller wire, and attaching the appropriate signal wires to an Arduino.  Then you just need to install the Arduino firmware packaged in the NintendoSpy release, and run the viewer software.  For more in-depth tutorials on how to do this, check out some of the links below.

![](https://github.com/jeremyaburns/NintendoSpy/raw/master/docs/tutorial-images/wiring-all.png)

[EvilAsh25's SNES hardware building guide](https://github.com/jaburns/NintendoSpy/blob/master/docs/guide-evilash25.md)

[Gamecube hardware tutorial](https://github.com/jaburns/NintendoSpy/blob/master/docs/tutorial-gamecube.md)

[N64 hardware tutorial](https://github.com/jaburns/NintendoSpy/blob/master/docs/tutorial-n64.md)

