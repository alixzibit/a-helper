# a-helper
A-Helper: A driver helper/optimization utility designed for ROG Ally

Features:
> Automated Driver installation support for Radeon 780m driver (Experimental)
> Armoury Crate button redirection to mimic Xbox guide button
> 1-click optimization for WiFi/Bluetooth control, stopping services, clearing temporary system files, GPU cache and system level tweaks


REQUIRES .NET 6 - If you do not have this installed, the application will prompt for this and will take you to the official Microsoft website for download
Test Build is available for [Download ](https://github.com/alixzibit/a-helper/releases/download/testbuild_release/ahelper_test_build.zip)

USAGE INFO

AS THIS IS A TEST BUILD - I STRONGLY RECOMMEND CREATING A SYSTEM RESTORE POINT ON YOUR DEVICE 


REGARDING RADEON 780M DRIVER INSTALLATION

*Recently Asus finally released AFMF enabled drivers - which makes the case for 780M drivers perhaps a bit unecessary. But I would think that AMD will still be releasing updates to 780M drivers much quicker than Asus will to their Z1x Radeon Driver, so the use case for installing 780M driver IMHO is still valid*

Installating 780M drivers

I had a few ideas regarding a code managed automated driver installation but none of those ideas truly worked out in the case of manual driver installation for a device like ROG ALLY. There is a tried and tested method involving INF manipulation but that requires driver signature enforcement to be turned off which is not ideal for many reasons- the only method which can be achievable is through the use of SetupAPI programming which requires a solid understanding of low level system APIs and INF parsing. Anyways long story short - I decided to opt for a "fun" or some could say a radical way to approach this problem.

INSTRUCTIONS
Automated UI interaction based driver installation - This method is experimental, but as far as my testing on my particular Ally unit and my development PC goes - it seems to work just fine. You will need to put your setup file in the applicaitons driver_setup folder and the applicaiton will take care of extracting the setup and then it will launch device properties of the AMD GPU and automatically browse through the UI, select the INF file, search and select from the devices list "AMD Radeon 780m" and prompt to install. You will need to manually select Yes or No for the final install prompt - this is left intentionally so that the user can review in case the automation didnt work as expected for some reason. 

PLEASE NOTE - the moment automated UI interaction status appears in the application please do not tap on the screen or press any buttons which might interrupt the process - if you want to cancel you can tap B button 2-3 times to close the device properties and hence cancelling driver installation


UI Navigation

One of the primary goals of this utility is to utilize the gamepad in handheld device like in the ROG Ally to make UI interaction easier. A robust UI navigation and control is under development and testing. But for the time being the current implementation works but it is not seamless yet as its still work in progress.
Before running the application make sure your device is set to auto controller mode
- left stick or d-pad for movement,
-  A button for enter/clicks
-  B button will cancel or exit automated UI interaction or some screens
-  When you are unable to focus or jump to an expected button or control you can tap Left button
which can cycle through all available UI elements on screen.
- Only when you are interacting with stop services screen you will need to manually tap or use right stick to click out of focus from the services table list when you want to navigate away from there

Armoury Crate to Xbox Key redirection

This feature was developed based on the presence of Armoury Crate and related services which trigger the Armoury Crate application upon pressing the button - so if you stop Armoury Crate service or Asus monitor service this feature will not work. Also by default Game bar is opened when you press the button, but if you want Steam overlay - you will need to set that toggle which will disable Game bar completely.

Optimize Features

(more detailed readme will be added soon)
