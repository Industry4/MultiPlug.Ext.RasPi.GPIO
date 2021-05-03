# MultiPlug.Ext.RasPi.GPIO
Raspberry Pi GPIO Extension for the MultiPlug .Net Edge Computing Platform

## Getting Started

These instructions will guide you the installation of the Extension on an instance of the MultiPlug .Net Edge Computing Platform.

### Prerequisites

Install the MultiPlug Edge Computing Platform. Instructions: [apt.multiplug.app](https://apt.multiplug.app/)

### Installing

The Extension can be installed using the in-built MultiPlug installer located at [http://multiplug.local/settings/add/](http://multiplug.local/settings/add/)
 *Replace multiplug.local for the IP Address of the MultiPlug instance*
 
Or sideloaded by downloading [multiplug.ext.raspi.gpio.nupkg](https://www.nuget.org/api/v2/package/MultiPlug.Ext.RasPi.GPIO/)

## Runtime
### Screenshot

![Image of MultiPlug.Ext.RasPi.Config](https://raw.githubusercontent.com/Industry4/MultiPlug.Ext.RasPi.GPIO/master/media/multiplug-ext-raspi-gpio.png)

### Application

The Extension can be accessed from: [http://multiplug.local/extensions/multiplug.ext.raspi.gpio/](http://multiplug.local/extensions/multiplug.ext.raspi.gpio/)
 *Replace multiplug.local for the IP Address of the MultiPlug instance*
 
### Functionality

* Read or Write to the GPIO pins, which will trigger MultiPlug Events on Read, and write on a subscription change.
* Set Pin pull up or pull down settings.


### Known Bugs
* The use of the native GPIO libary results in a slow timed-out shutdown.

## Authors

* **David Graham** - *Initial work* - [4IR British Systems](https://www.4ir.uk)

## License

This project is licensed under the MIT License
## Acknowledgments
Thanks for the support from:
* Julian Singh
* Ian Rathbone
* Julius Angwenyi
* Brainboxes Ltd

## Also see
MultiPlug Discovery Apps:
* [Windows](https://windows.multiplug.app/bin/DesktopSetup.exe)
* [Android](https://play.google.com/store/apps/details?id=uk.britishsystems.multiplug)
