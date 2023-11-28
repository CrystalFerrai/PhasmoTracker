# PhasmoTracker

A Windows app that allows you to track the progress of a contract in the game Phasmophobia. It also includes an overlay that can be used to present the current status while streaming or recording the game.

## Features

* Track evidence status.
* Track possible ghost types.
* Simulate passive sanity drain.
* Start/stop a general purpose timer.
* Enable keyboard shortcuts for all features which work while in game.
* Show a separate stream/recording friendly window for capturing with a program such as OBS.

## Releases

Releases can be found here: [Releases](https://github.com/CrystalFerrai/PhasmoTracker/releases)

## Installation

You will need to install the latest .NET 7 Desktop Runtime (x64) if you do not already have it. You can find it at [https://dotnet.microsoft.com/en-us/download/dotnet/7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0). Look for the section titled ".NET Desktop Runtime" and click on the installer link that reads "x64".

The app currently does not include an installer. Simply unzip the contents of the release into a folder and run PhasmoTracker.exe.

## Security Concerns

Antivirus or other security software programs may flag this app as a potential risk for two reasons.

1. It is not digitally signed. I do not have nor do I wish to pay for an app signing certificate.
2. It monitors keyboard input on your system in order to detect shortcut keys being pressed. This may look to an antivirus like a potential key logger. However, nothing is actually being recorded by the app.

Wether you want to trust the app is entirely up to you. If you do, then you may need to add an exception to your security software. Alternatively, you can clone the repo and build from source. Most security software will not flag programs that are built locally on your machine.

## How to Use

### Configuration Buttons

Once the app is runing, the first thing to draw your attention to are the buttons in the lower left of the main window. Here you will find the following buttons.

**Game Settings**

This button will open a separate window where you can configure the app based on your current game settings. This includes things such as the number of evidence available and various settings that affect sanity drain speed.

**Keyboard Shortcuts**

This button allows you to enable or disable the keyboard shortcuts for the app. Note that while enabled, the app will respond to keyboard input whether it is in focus or not. This allows you to press shortcut keys while you are in the game, but may not be disirable while doing other things on your PC.

**Overlay Window**
This button shows or hides the stream overlay window. The window needs to be shown in order to be captured by a program such as OBS. See "Capturing from OBS" below for more details.

**Show Sanity in Overlay**
This button allows you to show or hide current sanity in the stream overlay window. You may want to hide it if you are not actively using the sanity simulation feature. Sanity is always visible in the main window.

### Evidence Buttons
At the top of the window are evidence buttons. You can left-click on one of these to confirm an evidence or right-click to cross it out. This will grey out unavailable ghosts.

This feature is more comprehensive than the in-game journal, accounting for available evidence count as well as forced and fake evidences. Be sure you have configured the available evidence count in the game options in order for this to function properly.

### Ghost Buttons
Below the evidence list is a list of all possible ghosts. You can left-click one of these to mark at as the suspected ghost or right-click to cross it out.

### Normal Speed Button
Left-clicking this button will grey out ghosts that cannot hunt at normal ghost speed. Note that "The Twins" will not be greyed out by this action since it may not always be easy to distinguish normal speed from twin speed.

Right-clicking this button will cross out ghosts that can only hunt at normal speed, leaving only those that can possibly hunt at abnormal speeds.

### General Purpose Timer
Left-clicking on the "0:00" text will activate a timer that starts counting minutes and seconds. Left-clicking again will stop the timer and reset it back to 0. This may be useful for timing smudges, hunts, or whatever else you are interested in timing.

### Sanity Simulation
If you want to try to track your current sanity while playing, you can use the sanity simulation feature. Left-clicking on the displayed percentage will start simulating passive sanity drain (the drain that occurs when you are in a dark room). Left-clicking again will pause the drain, which you might do if you turn on a room light or leave the house. Right-clicking will reset back to starting sanity.

_Be sure to configure game settings before using this feature. A number of things affect the passive sanity drain rate._

Some sanity drain situations are not accounted for, including:
* Being in a large room on a large map with lights on.
* Being within 2 meters of an active firelight.
* Being in the presence of a manifested Phantom.
* Being hit with sanity draining abilities such as those used by Poltergeist, Yurei and Jinn.

There is also a button you can click to instantly lose 5% sanity. If you are a hit by a ghost event, press this twice to account for the lost 10% sanity. Or you can press it 3 or 4 times for special cases like Banshee and Oni.

There is an additional button that restores sanity equal to the currently configured sanity medication amount.

## Keyboard Shortcuts

Currently, the available key bindings are hard-coded and not displayed anywhere in the app. This will change later on, but for now, here is a list of available keys.

If the option "Keyboard shortcuts" option is disabled, the only availble shortcut is "R" to reset state for a new contract, and it only works when the app has focus.

If the option "Keyboard shortcuts" is enabled, the following shortcuts will be available, regardless whether the application has focus or not.

* `Alt+R` - Reset state for a new contract.
* `1` - Start or restart the general purpose timer.
* `Shift+1` - Stop the general purpose timer.
* `2` - Start or pause passive sanity drain.
* `Shift+2` - Reset sanity.
* `3` - Instantly remove 5 sanity (useful for various ghost events).
* `Shift+3` - Add sanity equal to the currently configured sanity pill amount.
* `4` through `0` - Select evidences
* `Shift+4` through `Shift+0` - Cross out evidences
* `Numpad 0`, `Numpad .`, `Numpad Enter` - Toggle bottom row of ghosts
* `Numpad 1`, `Numpad 2`, `Numpad 3` - Toggle second row of ghosts
* `Numpad 4`, `Numpad 5`, `Numpad 6` - Toggle third row of ghosts
* `Numpad 7`, `Numpad 8`, `Numpad 9` - Toggle fourth row of ghosts
* `Shift+Numpad` - Same as above, but for ghost rows 5, 6, 7 and 8
* `Backspace` - Toggle "normal ghost hunting speed"
* `Shift+Backspace` - Toggle "abnormal ghost hunting speed"

## Capturing from OBS

You can capture either the main window or stream overlay Window, depending on your preference, in OBS or other similar video capturing software. To setup capturing the stream overlay window, you need to first ensure it is open by pressing the button in the main window.

In OBS, add a new "Window Capture". Call it soemthing like PhasmoTracker, or whatever you want.
* Select the appropriate window to capture. The stream overlay window will be named "PhasmoTracker Overlay".
* Set the capture method to "Windows 10". The other capture methods will not work.
* Set the match priority to "Window title must match". This ensures it will find the window automatically in the future.

## Future Features

This app is in a prerelease state. There are some features that have not yet been developed but are planned for the eventual 1.0 release.

* Save settings between sessions.
* Add a screen to display avaialble keyboard shortcuts and allow rebinding them.
* Only process keyboard shortcuts while either the app or the game have focus, instead of globally.

## Reporting Issues

If you run into problems, you can file issue tickets on the Github repo. I make no promise that tickets will be addressed in a timely manner. This is just a little side project for me. However, I will look at each ticket and address it eventually one way or another.

Also, please do not file tickets for things that are already listed above in the "Future Features" section.

## Building from Source

Clone the repository, including submodules.
```
git clone --recursive https://github.com/CrystalFerrai/PhasmoTracker.git
```

You can then open and build PhasmoTracker.sln.