<h1> <img src="https://raw.githubusercontent.com/SonicSpeedrunning/LiveSplit.SonicFrontiers/main/sonic-frontiers-logo.png" alt="SonicFrontiers" height="75" align="middle" /> Sonic Frontiers - Autosplitter</h1>

Fully-featured autosplitter for the PC version of Sonic Frontiers with customizable options

<img src="https://raw.githubusercontent.com/SonicSpeedrunning/LiveSplit.SonicFrontiers/main/settings.png">

# General behaviour

The autosplitter supports in-game timing.
- When loading the autosplitter, if your timing method is set to Real Time (RTA), LiveSplit will pop up a message prompt asking you if you want to switch over to Game Time
- Game time is full IGT in arcade mode. In normal story mode, Game Time will be Time without Loads. In order to make the autosplitter correctly manage the timing method, make sure to let it manage the start of your run

Automatic splitting can be set in the options.

# Contributing
## Setting Up Your Build Environment
There are a few things you will need to build this solution.
- This solution is configured to work on a Windows x64 computer. The build process will not work for other devices.
- You will want Visual Studio 2022 Community Edition (or better) from [Microsoft](https://visualstudio.microsoft.com/downloads/).
- Download .NET 4.8.1 from [Microsoft](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks?cid=getdotnetsdk).
- Move your "regular" copy of LiveSplit to a directory other than "C:\ProgramFiles (x86)\LiveSplit\" for safety.
- Download/make a copy of [LiveSplit](http://livesplit.org/downloads/) and put it in "C:\Program Files (x86)\LiveSplit\" The path to LiveSplit.exe needs to be "C:\Program Files (x86)\LiveSplit\LiveSplit.exe."
- Clone a copy of this respository