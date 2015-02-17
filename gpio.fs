    \ (c) 2014, 2015 Oliver Holt
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

include gpio_lib.fs

: d cr .s ; \ debug word to see stack contents
: bin  2 base ! ;
: dec 10 base ! ;

: gpio 	dup 0= if 14 else \ lookup table to turn useful pin numbers to real pin numbers (see table)
	dup 1 = if 15 else
	dup 2 = if 18 else
	dup 3 = if 23 else
	dup 4 = if 24 else
	dup 5 = if 25 else
	dup 6 = if 8 else
	dup 7 = if 7 else
	dup 8 = if 2 else
	dup 9 = if 3 else
	dup 10 = if 4 else
	dup 11 = if 17 else
	dup 12 = if 27 else
	dup 13 = if 22 else
	dup 14 = if 10 else
	dup 15 = if 9 else
	dup 16 = if 11 else
	then then then then then then then then
	then then then then then then then then then
	swap drop
;

: output { pin -- }	\ sets specified pin as an output
	piosetup 0= if 
		pin pipinoutput 0= if s" pin " pin . s" set for output" type cr endif
	else s" oops GPIO did not initalize!" type endif 2drop
;

: input { pin -- }	\ sets specified pin as an input
piosetup 0= if 
	pin pipininput 0= if s" pin " pin . s" set for input" type cr endif
else s" oops GPIO did not initalize!" type endif 
;

: high { pin -- f }	\ sets specified output pin high
	pin pipinhigh drop
;

: low { pin -- f }	\ sets specified output pin low
	pin pipinlow drop
;
: cleanup ( -- )	\ cleans up GPIO
	piocleanup 0= if s" Ok GPIO is now turned off!" type cr endif
;

: read { pin -- }	\ reads from specified input pin
	pin pad pipinread 0= if else s" pin " pin . s" failed to read for some reason!" type cr endif
	pad c@ 0= if 0 else -1 endif
;

: outputr ( pinlow pinhigh -- ) 1+ swap do I gpio output loop ;	\ sets the inclusive range of the pins entered as outputs

: inputr ( pinlow pinhigh -- ) 1+ swap do I gpio input loop ;	\ sets the inclusive range of the pins entered as inputs

: highr ( pinlow pinhigh -- )  1+ swap do I gpio high loop ;	\ sets the inclusive range of the output pins entered to high

: lowr ( pinlow pinhigh -- )  2 + swap do I gpio low loop ;	\ sets the inclusive range of the output pins entered low

: shiftout ( n -- ) 8 0 do dup 2 mod 0= if I gpio low else I gpio high then 2/ loop drop ; \ shifts an 8 bit number onto pins 0 to 7 

: count  ( -- )  255 0 do I shiftout 50 ms loop ;	\ counts from 0 to 255 on pins 0 to 7 with a spacing of 50ms

: serialout  ( -- )  8 0 do					\ shift data out to 595 shift register or similar
		I 2 mod 0 xor 0= if 0 gpio low else 0 gpio high then		\ clock (sck) 	0 gpio
		dup 2 mod 0= if 1 gpio low else 1 gpio high then 	 	\ data  (ser)	1 gpio
		I 8 mod 7 = if 2 gpio high else 0 gpio low then			\ latch (rck)	2 gpio
		2/ loop 
;


: 7seg  dup 0= if 0x3f else	\ 7 segment display lookup table g-a
	dup 1 = if 0x06 else
	dup 2 = if 0x5b else
	dup 3 = if 0x4f else
	dup 4 = if 0x66 else
	dup 5 = if 0x6d else
	dup 6 = if 0x7d else
	dup 7 = if 0x07 else
	dup 8 = if 0x7f else
	dup 9 = if 0x6F else
	dup 10 = if 0x77 else \ A
	dup 11 = if 0x7C else \ B
	dup 12 = if 0x39 else \ C
	dup 13 = if 0x5E else \ D
	dup 14 = if 0x79 else \ E
	dup 15 = if 0x71 else \ F
	dup 16 = if 0x00 else \ blank space
	then then then then then
	then then then then then
	then then then then then 
	then then swap drop
;

: point ( --) 0x80 + ;	\ 7 segment decimal point add on 

: display4  ( n -- ) 	0 11 outputr \ displays a 4 digit decimal number on 4 7 segment displays (common cathode and multiplexed with pins 8 to 11)
			8 11 highr 
				begin
				 	dup 10 mod 7seg 8 gpio high shiftout 11 gpio low 1 ms
					dup 10 / 10 mod 7seg 11 gpio high shiftout 10 gpio low 1 ms 
					dup 100 / 10 mod 7seg 10 gpio high shiftout 9 gpio low 1 ms
					dup 1000 / 10 mod 7seg 9 gpio high shiftout 8 gpio low 1 ms
				again
;

: display8  ( n -- ) 	0 15 outputr \ displays an 8 digit decimal number on 8 7 segment displays (common cathode and multiplexed with pins 8 to 15)
			8 15 highr 
				begin
				 	dup 10 mod 7seg 8 gpio high shiftout 15 gpio low 1 ms
					dup 10 / 10 mod 7seg 15 gpio high shiftout 14 gpio low 1 ms 
					dup 100 / 10 mod 7seg 14 gpio high shiftout 13 gpio low 1 ms
					dup 1000 / 10 mod 7seg 13 gpio high shiftout 12 gpio low 1 ms
					dup 10000 / 10 mod 7seg 12 gpio high shiftout 11 gpio low 1 ms 
					dup 100000 / 10 mod 7seg 11 gpio high shiftout 10 gpio low 1 ms
					dup 1000000 / 10 mod 7seg 10 gpio high shiftout 9 gpio low 1 ms
					dup 10000000 / 10 mod 7seg 9 gpio high shiftout 8 gpio low 1 ms
				again
;


: displayn4  ( n t -- ) 4 / 0	\ same as display4 but for a finite time in ms
				8 11 highr 
				?do
				 	dup 10 mod 7seg 8 gpio high shiftout 11 gpio low 1 ms
					dup 10 / 10 mod 7seg 11 gpio high shiftout 10 gpio low 1 ms 
					dup 100 / 10 mod 7seg 10 gpio high shiftout 9 gpio low 1 ms
					dup 1000 / 10 mod 7seg 9 gpio high shiftout 8 gpio low 1 ms
				loop drop
;


: displayn8  ( n t -- ) 4 / 0 	\ same as display8 but for a finite time in ms
				8 15 highr 
				?do
				 	dup 10 mod 7seg 8 gpio high shiftout 15 gpio low 1 ms
					dup 10 / 10 mod 7seg 15 gpio high shiftout 14 gpio low 1 ms 
					dup 100 / 10 mod 7seg 14 gpio high shiftout 13 gpio low 1 ms
					dup 1000 / 10 mod 7seg 13 gpio high shiftout 12 gpio low 1 ms
					dup 10000 / 10 mod 7seg 12 gpio high shiftout 11 gpio low 1 ms 
					dup 100000 / 10 mod 7seg 11 gpio high shiftout 10 gpio low 1 ms
					dup 1000000 / 10 mod 7seg 10 gpio high shiftout 9 gpio low 1 ms
					dup 10000000 / 10 mod 7seg 9 gpio high shiftout 8 gpio low 1 ms
				loop drop
;

: dispbyte  ( n t -- ) 4 / 0	\ same as displayn8 but display is in Hexadecimal
				8 15 highr 
				?do
				 	dup %10 mod 7seg 8 gpio high shiftout 15 gpio low 1 ms
					dup %10 / %10 mod 7seg 15 gpio high shiftout 14 gpio low 1 ms 
					dup %100 / %10 mod 7seg 14 gpio high shiftout 13 gpio low 1 ms
					dup %1000 / %10 mod 7seg 13 gpio high shiftout 12 gpio low 1 ms
					dup %10000 / %10 mod 7seg 12 gpio high shiftout 11 gpio low 1 ms 
					dup %100000 / %10 mod 7seg 11 gpio high shiftout 10 gpio low 1 ms
					dup %1000000 / %10 mod 7seg 10 gpio high shiftout 9 gpio low 1 ms
					dup %10000000 / %10 mod 7seg 9 gpio high shiftout 8 gpio low 1 ms
				loop drop
;

: 7count4 ( -- ) 10000 0 do I 12 displayn4 loop ; \ counts from 0 to 9999 on 4 7 segment displays

: 7count8 ( -- ) 100000000 0 do I 12 displayn8 loop ; \ counts from 0 to 9999999 on 8 7 segment displays

: 7time4 ( -- ) 0 11 outputr begin time&date drop 2drop rot drop 100 * + 4 displayn4 again ; \ displays the time hh:mm on 4 7 segment displays

: 7time8 ( -- ) 0 15 outputr 13 gpio input 10 gpio input begin time&date drop 2drop  1000000 * -rot 1000 * + + 4 displayn8 again ; \ displays the time hh:mm on 8 7 segment displays

: loading  ( t -- ) 4 / 0 	0 15 outputr \ sends decimal point l->r across 8 7 segment displays for time t in ms
				8 15 highr 
				?do
				 	16 7seg point 8 gpio high shiftout 15 gpio low 100 ms
					16 7seg point 15 gpio high shiftout 14 gpio low 100 ms 
					16 7seg point 14 gpio high shiftout 13 gpio low 100 ms
					16 7seg point 13 gpio high shiftout 12 gpio low 100 ms
					16 7seg point 12 gpio high shiftout 11 gpio low 100 ms 
					16 7seg point 11 gpio high shiftout 10 gpio low 100 ms
					16 7seg point 10 gpio high shiftout 9 gpio low 100 ms
					16 7seg point 9 gpio high shiftout 8 gpio low 100 ms
				loop
;

: bounce  ( t -- ) 4 / 0 	0 15 outputr \ bounces decimal pint across 8 7 segment displays for time t in ms
				8 15 highr 
				?do
					16 7seg point 15 gpio high shiftout 14 gpio low 100 ms 
					16 7seg point 14 gpio high shiftout 13 gpio low 100 ms
					16 7seg point 13 gpio high shiftout 12 gpio low 100 ms

					16 7seg point 12 gpio high shiftout 11 gpio low 100 ms 

					16 7seg point 11 gpio high shiftout 10 gpio low 100 ms
					16 7seg point 10 gpio high shiftout 9 gpio low 100 ms
					16 7seg point 9 gpio high shiftout 8 gpio low 100 ms

					16 7seg point 8 gpio high shiftout 9 gpio low 100 ms
					16 7seg point 9 gpio high shiftout 10 gpio low 100 ms
					16 7seg point 10 gpio high shiftout 11 gpio low 100 ms

					16 7seg point 11 gpio high shiftout 12 gpio low 100 ms

					16 7seg point 12 gpio high shiftout 13 gpio low 100 ms
					16 7seg point 13 gpio high shiftout 14 gpio low 100 ms 
					16 7seg point 14 gpio high shiftout 15 gpio low 100 ms
				loop
;

: square-wave ( t -- ) 0 7 outputr begin 255 shiftout dup ms 0 shiftout dup ms again drop ; \ outputs a square wave when pins 0 to 7 are connected to a DAC

: sawtooth-wave ( t -- ) 0 7 outputr begin 255 0 do I shiftout loop again drop ;	\ outputs a sawtooth wave when pins 0 to 7 are connected to a DAC

: sine-wave ( t -- ) 0 7 outputr begin 127 -128 do I I 255 */ shiftout loop 127 -128 do I I -255 */ shiftout loop again drop ; \ outputs a sinusoidal wave when pins 0 to 7 are connected to a DAC




