# MultiPlug.Ext.RasPi.GPIO
Raspberry Pi GPIO Extension for the MultiPlug .Net Fog Computing Platform

## Getting Started

These instructions will guide you the installation of the Extension on an instance of the MultiPlug .Net Fog Computing Platform.

### Prerequisites

Both Mono and the MultiPlug .Net Fog Computing Platform must be installed on Raspberry Pi's Raspbian operating system.

Add the following line to /etc/apt/sources.list

```
deb [trusted=yes] http://apt.multiplug.uk ./
```
Run the following command:
```
sudo apt update
```
To install Mono run the following command:
```
sudo apt-get install mono-complete
```
To install MultiPlug run the following command:
```
sudo apt-get install multiplug
```

### Installing

The Extension can be installed using the in-built MultiPlug installer located at [http://multiplug.local/settings/add/](http://multiplug.local/settings/add/)
 *Replace multiplug.local for the IP Address of the MultiPlug instance*

## Runtime
### Screenshot

![Image of MultiPlug.Ext.RasPi.Config](https://raw.githubusercontent.com/Industry4/MultiPlug.Ext.RasPi.GPIO/master/media/multiplug-ext-raspi-gpio.png)

### Application

The Extension can be accessed from: [http://multiplug.local/extensions/raspi-config/](http://multiplug.local/extensions/raspi-config/)
 *Replace multiplug.local for the IP Address of the MultiPlug instance*
 
### Functionality

* Read or Write to the GPIO pins, which will trigger MultiPlug Events on Read, and write on a subscription change.
* Set Pin pull up or pull down settings.


### Known Bugs
* Due to a bug with the MultiPlug platform an system restart is needed before the Extension can be accessed. Restart the system via the option found in [http://multiplug.local/settings](http://multiplug.local/settings/)
* The use of the native GPIO libary results in a slow timed-out shutdown.

## Authors

* **David Graham** - *Initial work* - [4IR British Systems](https://4ir.uk)

## License

This project is licensed under the MIT License
## Acknowledgments
Thanks for the support from:
* Seemin Suleri
* Julian Singh
* Ian Rathbone
* Julius Angwenyi
* Brainboxes Ltd