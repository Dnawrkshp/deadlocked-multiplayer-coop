# Deadlocked PS2 Multiplayer Coop

This tool connects two players and synchronizes controller and game data between them on Ratchet: Deadlocked. It works by hooking itself into the pad read subroutine and replacing the appropriate controller inputs with a remote player's. Additional data, like position and rotation, are sent rapidly in game and interpolated on the client to prevent desynchronization.

## State

* [x] Synchronize Controller Inputs
* [x] Synchronize Player Objects
* [x] Synchronize Equipment
* [ ] Synchronize Stats
* [x] Synchronize Game Progress
* [ ] Synchronize Active Mission and Level
* [x] Synchronize Menu Selection
* [ ] Synchronize Active Menu
* [ ] Handle Disconnection

## Build

* Visual Studio 2017
* .NET Framework 4.0

## Setup and Debugging

Currently testing requires cloning and building the project from scratch. This will help set you up.

### Step 1: Setup Environment Variables for Debugging

Set the environment variable `PCSX2` to the path of your PCSX2 executable. Set the environment variable `DEADLOCKED_ISO` to the path of your Deadlocked iso. This just makes it easier to debug.

### Step 2: (Optional) Running on localhost

If you're testing over localhost then I recommend making a new copy of your PCSX2 directory to prevent any conflicts. Be sure to change the memory card config to use the new memory card copies. Then open cmd in the build directory and run either of the following commands (depending on host or client):

`DLMC.Launcher.exe connect -h localhost -p 19001 --pcsx2 PATH_TO_NEW_PCSX2_EXE --dl PATH_TO_DL_ISO`

or

`DLMC.Launcher.exe host -p 19001 --pcsx2 PATH_TO_NEW_PCSX2_EXE --dl PATH_TO_DL_ISO`

### Step 3: Setup Pad Hook in PCSX2

In order for the pad hook to work you need to enable cheats and place the following codes into the `9BFBCD42.pnach` cheats file:

```
// DLMC Hook
patch=1,EE,2013b9ac,extended,0c03c000
patch=1,EE,2013b9b0,extended,00000000
patch=1,EE,2013b9b4,extended,00000000
patch=1,EE,2013b9b8,extended,00000000

// DLMC Pad Overwrite
patch=1,EE,200f0000,extended,3c04000f
patch=1,EE,200f0004,extended,8c8400f0
patch=1,EE,200f0008,extended,1083000b
patch=1,EE,200f000C,extended,00000000
patch=1,EE,200f0010,extended,24840080
patch=1,EE,200f0014,extended,10830008
patch=1,EE,200f0018,extended,00000000
patch=1,EE,200f001C,extended,00000000
patch=1,EE,200f0020,extended,dc670010
patch=1,EE,200f0024,extended,dc680018
patch=1,EE,200f0028,extended,dc640000
patch=1,EE,200f002C,extended,dc650008
patch=1,EE,200f0030,extended,03e00008
patch=1,EE,200f0034,extended,00000000
patch=1,EE,200f0038,extended,3c07000f
patch=1,EE,200f003C,extended,34e70100
patch=1,EE,200f0040,extended,dce40000
patch=1,EE,200f0044,extended,dce50008
patch=1,EE,200f0048,extended,dc670010
patch=1,EE,200f004C,extended,dc680018
patch=1,EE,200f0050,extended,03e00008
```

### Step 4: Configure Controller in PCSX2

If you're planning to be the host, then you will be player 1. Make sure that your controller config is setup for player 1.

If you're the client, connecting to the host, then you will be player 2. Make sure that your controller/keyboard is setup to affect player 2.

### Step 5: Hosting and Connecting

If you plan on hosting, you'll need to port forward whichever port you choose to use for UDP. The default port is 19001. After port forwarding, open an instance of cmd in the build directory. Run the following command:

`DLMC.Launcher.exe host -p 19001 --pcsx2 PATH_TO_PCSX2_EXE --dl PATH_TO_DL_ISO`

**NOTE: For hosting you can also just debug the project with the `DEBUG_SERVER` configuration set.**

If you plan on connecting to the host, you'll simply need the host's public ip address and port. Then open an instance of cmd in the build directory and run the following command:

`DLMC.Launcher.exe connect -h PUBLIC_IP_ADDRESS -p PORT --pcsx2 PATH_TO_PCSX2_EXE --dl PATH_TO_DL_ISO`

### Step 6: Connecting

After connecting a new instance of PCSX2 will launch on both peers. It will automatically start the provided iso. You are welcome to have an instance already running and simply omit the paths to PCSX2 and the DL ISO if you'd like the program to hook into your running instance.

Since active menu and mission/level aren't synchronized, both peers must start in the same menu and level/mission. I recommend creating a new save with the other person to ensure identical starting points.

It is still possible for menu navigation to become desynchronized when a player goes through menus too quickly. Be sure to give time for the other client to catch up before entering or exiting a menu.
