; PC-Server-Demo
#SingleInstance Force
SoundPlay, Sounds\open-heineken.wav 
gui, show, , Control Interface


Return
;===================
F1::
Joy1::

IfWinExist,Stepper
{
	WinActivate
	ControlFocus, 1
	send , {enter}
	}
return
;=================
F2::
Joy2::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus, 2
	send , {enter}
	}
return
;=================
F3::
Joy3::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus, 3
	send , {enter}
	}
return
;=================
F4::
Joy4::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,4
	send , {enter}
	}
return
;=================
F5::
Joy5::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,5
	send , {enter}
	}
return
;=================
F6::
Joy6::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,6
	send , {enter}
	}
return
;=================
F7::
Joy7::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,7
	send , {enter}
	}
return
;=================
F8::
Joy8::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,8
	send , {enter}
	}
return
;=================
F9::
Joy9::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,9
	send , {enter}
	}
return
;=================
F10::
Joy10::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,A
	send , {enter}
	}
return
;=================
F11::
Joy11::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,B
	send , {enter}
	}
return
;=================
F12::
Joy12::
IfWinExist,Stepper
{
	WinActivate
	ControlFocus,C
	send , {enter}
	}
return
;=================
closeApps:
IfWinExist, PowerPoint Slide Show
	WinClose 
IfWinExist, CyberLink
	WinClose
return
;=================
guiclose:
exitapp 
return
