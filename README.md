![icon](resources/a_icon.png)


# A-Helper: Automated Driver Helper and Optimization Utility for ROG Ally

Welcome to A-Helper, a specialized utility designed to enhance your experience on ROG Ally devices by providing driver installation support and system optimizations.

## Features

- **Automated Radeon 780m Driver Installation**: Provides experimental support for automatic driver installation for 780m driver on Z1x SoC.
- **Armoury Crate Button Redirection**: Mimics the Xbox guide button functionality, enhancing the gaming interface.
- **1-Click Optimization**: Simplifies the optimization process for WiFi/Bluetooth controls, stops unnecessary services, clears temporary files, and applies GPU cache and system-level tweaks. This feature is designed to automatically select and manage certain services that are commonly found on ROG Ally devices, ensuring optimal system performance with minimal user input.

Recently Asus finally released AFMF enabled drivers - which makes the case for 780M drivers perhaps a bit unnecessary. But I would think that AMD will still be releasing updates to 780M drivers much quicker than Asus will to their Z1x Radeon Driver, so the use case for installing 780M driver IMHO is still valid

## Prerequisites

- **.NET 6**: Necessary to run the application. If not installed, the application will guide you to the official Microsoft website for download.

## Installation

1. Download the test build from the [release page](https://github.com/alixzibit/a-helper/releases/download/testbuild_release/ahelper_test_build.zip).
2. Extract the files to your preferred location.

## Important Usage Information

- **System Restore**: Strongly recommended to create a system restore point before using this utility on your device.
- **UI Navigation**: Utilizes gamepad inputs for navigation – the navigation works but its still under development – You will need to use Left button to cycle through UI elements if you are not able to focus using left stick or d-pad.
   - **Movement**: Use the left stick or d-pad.
   - **Select**: Press the 'A' button.
   - **Cancel**: Press the 'B' button to exit automated interactions or screens.

## Automated Driver Installation

The application supports an automated UI interaction method for driver installation. Place your driver setup file in the application's driver_setup folder. The application will handle the extraction and launch of the device properties automatically, guiding you through the installation process.

**Note**: During the automated UI interaction, avoid any screen taps or button presses that might interrupt the process. If necessary, press the 'B' button multiple times to cancel.

## Contributions

Contributions are welcomed! Fork the repository, make your improvements, and submit a pull request.
