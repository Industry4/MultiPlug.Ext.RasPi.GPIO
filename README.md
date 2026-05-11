# MultiPlug.Ext.RasPi.GPIO
Raspberry Pi GPIO Extension for the [MultiPlug](https://www.multiplug.app/) .Net Edge Computing Platform

## Getting Started

These instructions will guide you the installation of the Extension on an instance of the MultiPlug .Net Edge Computing Platform.

### Prerequisites

Install the MultiPlug Edge Computing Platform on Raspberry Pi OS (Debian Linux). [MultiPlug Installation Instructions](https://apt.multiplug.app/)

### Linux Support

* Bullseye (Debian 11)
* Bookworm (Debian 12)
* Trixie (Debian 13)

### Installing

The Extension can be sideloaded by [downloading](https://github.com/Industry4/MultiPlug.Ext.RasPi.GPIO/releases/) the .nupkg file and navigating to **/settings/add/** within MultiPlug.

The .nupkg file is also available to download from [Nuget](https://www.nuget.org/packages/MultiPlug.Ext.RasPi.GPIO).

The Extension may have to be enabled and started from **/settings/extensions/** within MultiPlug.

## Runtime
### Screenshots
#### Setup
![Image of MultiPlug.Ext.RasPi.Config Setup](https://raw.githubusercontent.com/Industry4/MultiPlug.Ext.RasPi.GPIO/master/media/multiplug-ext-raspi-gpio.png)
#### Monitoring
![Image of MultiPlug.Ext.RasPi.Config Monitoring](https://raw.githubusercontent.com/Industry4/MultiPlug.Ext.RasPi.GPIO/master/media/multiplug-ext-raspi-gpio2.png)

### Application

The Extension can be accessed from **extensions/multiplug.ext.raspi.gpio/** within MultiPlug.
 
### Functionality

* Read or Write to the GPIO pins, which will trigger MultiPlug Events on Read, and write on a subscription change.
* Set the Pin Pull Up or Pull Down when a Pin is acting as a Input.
* Set a Debounce time when a Pin is acting as a Input.
* Set a Pin's Initialisation and Shutdown State when a Pin is acting as a Output.

## Authors

* **David Graham** - *Initial work* - [4IR.UK British Systems](https://www.4ir.uk)

## License

This project is licensed under the MIT License. Uses [Wiring Pi 3.18](https://github.com/wiringpi/wiringpi)
