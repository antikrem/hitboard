# hitboard
> Configurable Hitbox/Mixboard Emulator

Sets up a virtual controller and reroute keyboard input to emulate arcade controllers. Fully customisable though the GUI. Custom profiles can be saved for later use.

## Install
1. Download and install the latest release of [ViGEm drivers](https://github.com/ViGEm/ViGEmBus/releases)
2. Download the latest release of hitboard and run the setup

## Usage
![frontend](https://i.ibb.co/9gs0xDx/Untitled.png)

1. Pick a configuration from the drop down.
2. Optionally, change parameters to your specific needs.
3. Press `Start` (This begins the keyboard hook, and selected inputs will be redirected to the virtual controller).
4. Hit `Stop` to pause the controller (or press ESC).

## Features
* Simultaneous Opposing Cardinal Directions (SOCD) - Choose how you wish to resolve oposing inputs. More information about SOCD can be found [here](https://www.hitboxarcade.com/blogs/faq/what-is-an-socd)
* Profiles - In addition to the default profiles, create and customise profiles then share through `json` files.
* Performance - OS level hooks and event based design minimises input delay. There is no polling lag or dropped input, even under load.

## Build
0. Compilation can be done with Visual Studio 2017 or later.
1. Clone the repo
2. You will need to first compile the ViGEm client. This will have to be specific to your machine. You should get a (Alternativly, you could maybe find the dll online)
3. Compile ViGEmWrapper.
4. Compile hitboard.
5. Run create_releast.bat to build a release export.
