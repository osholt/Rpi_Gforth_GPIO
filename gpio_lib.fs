\ This Gforth code is a Raspberry Pi GPIO library
\    Copyright (C) 2013  Philip K. Smith slightly modified by Oliver Holt 2014, 2015

    \ This file is part of rpi-gforth-gpio.

    \ rpi-gforth-gpio is free software: you can redistribute it and/or modify
    \ it under the terms of the GNU General Public License as published by
    \ the Free Software Foundation, either version 3 of the License, or
    \ (at your option) any later version.

    \ rpi-gforth-gpio is distributed in the hope that it will be useful,
    \ but WITHOUT ANY WARRANTY; without even the implied warranty of
    \ MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    \ GNU General Public License for more details.

    \ You should have received a copy of the GNU General Public License
    \ along with rpi-gforth-gpio.  If not, see <http://www.gnu.org/licenses/>.




clear-libs
c-library mygpio
s" rpigpio" add-lib
\ **** GPIO wrappers  ****
\c #include "rpiGpio.h"		
\c int pipinsetpullup(int pin) { return ( gpioSetPullResistor( pin, pullup ));}
\c int pipinsetpulldown(int pin) { return ( gpioSetPullResistor( pin, pulldown));}
\c int pipinsetpulldisable(int pin) { return ( gpioSetPullResistor( pin, pullDisable));} 
\c int pipininput(int pin) { return ( gpioSetFunction ( pin, input )) ; }
\c int pipinoutput(int pin) { return ( gpioSetFunction ( pin, output )) ; }
\c int pipinlow(int pin) { return ( gpioSetPin ( pin, low )) ; }
\c int pipinhigh(int pin) { return ( gpioSetPin ( pin, high )) ; }
\c int pipinread(int pin, eState *state) { return ( gpioReadPin(pin, state)); }
c-function pipinread pipinread n a -- n
c-function pipinsetpullup pipinsetpullup n -- n
c-function pipinsetpulldown pipinsetpulldown n -- n
c-function pipinsetpulldisable pipinsetpulldisable n -- n
c-function pipininput pipininput n -- n
c-function pipinoutput pipinoutput n -- n 
c-function piosetup gpioSetup -- n
c-function piocleanup gpioCleanup -- n
c-function pipinlow pipinlow n -- n
c-function pipinhigh pipinhigh n -- n
\ **** GPIO I2C wrappers ****
c-function pii2cread gpioI2cReadData a n -- n
c-function pii2cwrite gpioI2cWriteData a n -- n
c-function pii2clock gpioI2cSetClock n -- n
c-function pii2caddress gpioI2cSet7BitSlave n -- n
c-function pii2csetup gpioI2cSetup -- n
c-function pii2cleanup gpioI2cCleanup -- n
end-c-library

\ basic output on gpio pin 25 example
\ piosetup .	\ this should do basic setup of the gpio function and show errStatus value (0 for all ok)
\ 25 pipinoutput .
\ 25 pipinlow .
\ 1000 ms
\ 25 pipinhigh .
\ piocleanup .	\ this should stop the gpio function and show errStatus value ( 0 for all ok)
\ *******************************************************

\ basic input on GPIO pin 25 example
\ piosetup .
\ 25 pipininput .
\ 25 pad pipinread . pad 1 dump
\ piocleanup .
\ *******************************************************

\ basic read from a I2C device that is id # 104 decimal 
\ piosetup .
\ pii2csetup .
\ 104 pii2caddress .
\ 100000 pii2clock .
\ here dup 10 allot 10 pii2cread . \ read 10 bytes from device
\ 10 dump 	\ this should give you the 10 items that were read from the device
\ pii2cleanup .
\ piocleanup .
