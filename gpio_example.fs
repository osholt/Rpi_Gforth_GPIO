
    \ Copyright (C) 2013  Philip K. Smith , heavily modified by Oliver Holt 2014, 2015.
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



require gpio_lib.fs

: simpleoutput { pin -- }
piosetup 0= if 
	pin pipinoutput 0= if s" pin " pin . s" set for output" type cr endif
	pin pipinlow 0= if s" pin " pin . s" is low now" type cr endif
	1000 ms
	pin pipinhigh 0= if s" pin " pin . s" is high now" type cr endif
	piocleanup 0= if s" Ok GPIO is now turned off!" type cr endif
else s" oops GPIO did not initalize!" type endif 
;

: simpleinput { pin -- }
piosetup 0= if
	pin pipininput 0= if s" pin " pin . s" now set for input" type cr endif
	pin pad pipinread 0= if s" pin " pin . s" was read ok" type cr else s" pin " pin . s" failed to read for some reason!" type cr endif
	pad c@ 0= if s" pin " pin . s" current input is low" type cr else s" pin " pin . s" current input is high" type cr endif
	piocleanup 0= if s" GPIO is now turned off!" type cr endif
else s" oops GPIO did not initalize!" type endif
;

: output_timed { pin N_noop -- }
piosetup 0= if
	pin pipinoutput 0= if s" pin " pin . s" set for output" type cr endif
	BEGIN
		pin pipinlow drop
		N_noop 0 ?DO noop LOOP
		pin pipinhigh drop
	key? UNTIL	
	piocleanup 0= if s" GPIO is now turned off!" type cr endif
else s" oops GPIO did not initalize!" type cr endif
;

 
