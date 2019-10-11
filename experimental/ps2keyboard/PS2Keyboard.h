/*
  PS2Keyboard.h - PS2Keyboard library
  Copyright (c) 2007 Free Software Foundation.  All right reserved.
  Written by Christian Weichel <info@32leaves.net>

  ** Mostly rewritten Paul Stoffregen <paul@pjrc.com>, June 2010
  ** Modified for use with Arduino 13 by L. Abraham Smith, <n3bah@microcompdesign.com> * 
  ** Modified for easy interrup pin assignement on method begin(datapin,irq_pin). Cuningan <cuninganreset@gmail.com> **

  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.

  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/


#ifndef PS2Keyboard_h
#define PS2Keyboard_h

#include <avr/io.h>
#include <avr/interrupt.h>
#include <avr/pgmspace.h>
#if defined(ARDUINO) && ARDUINO >= 100
#include "Arduino.h" // for attachInterrupt, FALLING
#else
#include "WProgram.h"
#endif



/**
 * Purpose: Provides an easy access to PS2 keyboards
 * Author:  Christian Weichel
 */
class PS2Keyboard {
  public:
  	/**
  	 * This constructor does basically nothing. Please call the begin(int,int)
  	 * method before using any other method of this class.
  	 */
    PS2Keyboard();
    
    /**
     * Starts the keyboard "service" by registering the external interrupt.
     * setting the pin modes correctly and driving those needed to high.
     * The propably best place to call this method is in the setup routine.
     */
    static void begin(uint8_t dataPin, uint8_t irq_pin);
    
    /**
     * Returns true if there is a char to be read, false if not.
     */
    static bool available();
    
    /**
     * Returns the char last read from the keyboard.
     * If there is no char availble, -1 is returned.
     */
    static int read();
};

// interrupt pins for known boards
#if !defined(CORE_INT0_PIN)
#if defined(__AVR_ATmega1280__) || defined(__AVR_ATmega2560__) // Arduino Mega
#define CORE_INT0_PIN  2
#define CORE_INT1_PIN  3
#define CORE_INT2_PIN  21
#define CORE_INT3_PIN  20
#define CORE_INT4_PIN  19
#define CORE_INT5_PIN  18
#elif defined(__AVR_ATmega644P__) || defined(__AVR_ATmega644__) // Sanguino
#define CORE_INT0_PIN  10
#define CORE_INT1_PIN  11
#define CORE_INT2_PIN  2
#else  // Arduino Duemilanove, Diecimila, LilyPad, Mini, Fio, etc...
#define CORE_INT0_PIN  2
#define CORE_INT1_PIN  3
#endif
#endif

#endif
