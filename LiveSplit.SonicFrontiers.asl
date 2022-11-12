// Load time remover for Sonic Frontiers
// Coding: Jujstme
// contacts: just.tribe@gmail.com
// Version: 1.0.4 (Nov 12th, 2022)

state("SonicFrontiers") {}

init
{
    // Basic stuff we're gonna use in the autosplitter
    
    // Pretty self-explanatory. It checks if a certain IntPtr is IntPtr.Zero
    // If it is, then throw an Exception.
    // This is usually used to deal with invalid sigscans.
    var checkptr = (Action<IntPtr>)((p) => {
        if (p == IntPtr.Zero)
        throw new NullReferenceException("Sigscanning failed!");
    });

    // Basic sigscan. This looks for the pointer to the main instance of app:MyApplication,
    // which is the base pointer from which we can find every other relevant memory address.
    SignatureScanner scanner = new SignatureScanner(game, modules.First().BaseAddress, modules.First().ModuleMemorySize);
    IntPtr ptr = scanner.Scan(new SigScanTarget(1, "E8 ???????? 4C 8B 78 70") { OnFound = (p, s, addr) => {
                var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3;
                tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                return tempAddr;
    }});
    checkptr(ptr);

    // Important offsets below
    IntPtr tempaddr;

    // APPLICATION
    tempaddr = scanner.Scan(new SigScanTarget(3, "48 8B 99 ???????? 48 8B F9 48 8B 81 ???????? 48 8D 34 C3 48 3B DE 74 21"));
    checkptr(tempaddr);
    int APPLICATION = game.ReadValue<int>(tempaddr);

    // APPLICATIONSEQUENCE
    // Even though it never happened in my case, this offset can theoretically change dynamically
    // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
    tempaddr = scanner.Scan(new SigScanTarget(7, "48 83 EC 20 48 8D 05 ???????? 48 89 51 08 48 89 01 48 89 CF 48 8D 05") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
    checkptr(tempaddr);
    long APPLICATIONSEQUENCE_f = (long)tempaddr;
    vars.APPLICATIONSEQUENCE = 0;
    vars.GetAPPLICATIONSEQUENCE = (Func<int>)(() => {
        int value = new DeepPointer(ptr, APPLICATION + 0x8).Deref<byte>(game);
        if (value == 0) return 0;
        for (int i = 0; i < value; i++)
        {
            var g = new DeepPointer(ptr, APPLICATION, 0x8 * i, 0x0).Deref<long>(game);
            if (g == APPLICATIONSEQUENCE_f) return 0x8 * i;
        }
        return 0;
    });

    // GAMEMODE
    tempaddr = scanner.Scan(new SigScanTarget(1, "74 31 48 8D 55 E0") { OnFound = (p, s, addr) => addr + 0x31 + 0x4 });
    checkptr(tempaddr);
    int GAMEMODE = game.ReadValue<byte>(tempaddr);

    // GAMEMODEEXTENSION
    tempaddr = scanner.Scan(new SigScanTarget(8, "E8 ???????? 48 8B BB ???????? 48 8B 83 ???????? 4C 8D 34 C7"));
    checkptr(tempaddr);
    int GAMEMODEEXTENSION = game.ReadValue<int>(tempaddr);


    // Using DeepPointer instead of a memorywatcher because we need to do quirky stuff that relies on the APPLICATIONSEQUENNCE() which is not a constant.
    // Well'use .Deref instead.
    vars.CURRENTSTAGE = (Func<string>)(() => new DeepPointer(ptr, APPLICATION, vars.APPLICATIONSEQUENCE, 0xA0).DerefString(game, 5));
    vars.TUTORIALSTAGE = (Func<string>)(() => new DeepPointer(ptr, APPLICATION, vars.APPLICATIONSEQUENCE, GAMEMODE, 0xF8).DerefString(game, 5));
    vars.ARCADEFLAG = (Func<byte>)(() => new DeepPointer(ptr, APPLICATION, vars.APPLICATIONSEQUENCE, 0x122).Deref<byte>(game));


    //
    // Recovering the addresses for some RTTI stuff
    //

    // This value tells the game (and the autosplitter) the number of subclasses loaded in the main GAMEMODE class. It's used for the loops in the following functions.
    // It's the only MemoryWatcher used in this script.
    vars.GetGameModeExtensionCount = (Func<int>)(() => new DeepPointer(ptr, APPLICATION, vars.APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION + 0x8).Deref<byte>(game));
    vars.GameModeExtensionCount = 0;
    
    // StageTimeExtension - this is used to identify the active instance of this class. Used for IGT
    tempaddr = scanner.Scan(new SigScanTarget(1, "E9 ???????? 0F 86 15 8A 59 FF") { OnFound = (p, s, addr) => {
        var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x7;
        tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
        return tempAddr;
    }});
    checkptr(tempaddr);
    var StageTimeExtension = (long)tempaddr;

    // This ensures the real IGT is always caught
    vars.GetIGT = (Func<TimeSpan>)(() => {
        if (vars.GameModeExtensionCount == 0) return TimeSpan.Zero;
        for (int i = 0; i < vars.GameModeExtensionCount; i++)
        {
            var q = new DeepPointer(ptr, APPLICATION, vars.APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x0).Deref<long>(game);
            if (q == StageTimeExtension)
                return TimeSpan.FromSeconds(Math.Truncate(new DeepPointer(ptr, APPLICATION, vars.APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x28).Deref<float>(game) * 100) / 100);
        }
        return TimeSpan.Zero;
    });


    // HsmExtension - Hedgehog Scene Manager, maybe? Anyway, it's used to look for the status variable
    tempaddr = scanner.Scan(new SigScanTarget(1, "E8 ???????? 48 8B F8 48 8D 55 C0") { OnFound = (p, s, addr) => {
        var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x9;
        tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
        return tempAddr;
    }});
    checkptr(tempaddr);
    var HsmExtension = (long)tempaddr;

    // Very important variable. Essentially tells us the current status of the game
    vars.GetStatus = (Func<string>)(() => {
        if (vars.GameModeExtensionCount == 0) return string.Empty;
        for (int i = 0; i < vars.GameModeExtensionCount; i++)
        {
            var q = new DeepPointer(ptr, APPLICATION, vars.APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x0).Deref<long>(game);
            if (q == HsmExtension)
            return new DeepPointer(ptr, APPLICATION, vars.APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x60, 0x20, 0x0).DerefString(game, 255);
        }
        return string.Empty;
    });

    // QTE management data - we need the function's address to identify whether we are in a QTE or not
    vars.QTEinput = (Func<long>)(() => new DeepPointer(ptr, 0x70, 0xD0, 0x28, 0x0, 0x0).Deref<long>(game));
    tempaddr = scanner.Scan(new SigScanTarget(13, "C7 83 ???????? ???????? 48 8D 05 ???????? 48 89 BB") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
    checkptr(tempaddr);
    vars.QTEADDRESS = (long)tempaddr;

    // Default state
    current.IGT = TimeSpan.Zero;
    current.Status = "";
    current.LevelID = "";
    current.isInTutorial = false;
    current.isInQTE = false;
}

startup
{
    settings.Add("newGame", true, "Enable autostart on New Game");
    settings.Add("arcade", true, "Enable autostart on Arcade mode");
    settings.Add("startonw6d01", true, "Start the timer only when entering Act 1-1", "arcade");
    //settings.Add("storymode", true, "Story mode autosplitting options");
    //settings.Add("kronos", true, "Split when completing Kronos Island", "storymode");
    //settings.Add("ares", true, "Split when completing Ares Island", "storymode");
    //settings.Add("chaos", true, "Split when completing Chaos Island", "storymode");
    //settings.Add("rhea", true, "Split when completing Rhea Island", "storymode");
    //settings.Add("finalQTE", true, "Split at the final boss' QTE (final split for story mode)", "storymode");
    settings.Add("finalQTE", true, "Split at the final boss' QTE (final split for story mode)");
    var CyberSpaceLevels = new Dictionary<string, string>{
        { "w6d01", "1-1" }, { "w8d01", "1-2" }, { "w9d04", "1-3" }, { "w6d02", "1-4" }, { "w7d04", "1-5" }, { "w6d06", "1-6" }, { "w9d06", "1-7" },
        { "w6d05", "2-1" }, { "w8d03", "2-2" }, { "w7d02", "2-3" }, { "w7d06", "2-4" }, { "w8d04", "2-5" }, { "w6d03", "2-6" }, { "w8d05", "2-7" },
        { "w6d04", "3-1" }, { "w6d08", "3-2" }, { "w8d02", "3-3" }, { "w6d09", "3-4" }, { "w6d07", "3-5" }, { "w8d06", "3-6" }, { "w7d03", "3-7" },
        { "w7d08", "4-1" }, { "w9d02", "4-2" }, { "w7d01", "4-3" }, { "w9d03", "4-4" }, { "w6d10", "4-5" }, { "w7d07", "4-6" }, { "w9d05", "4-7" }, { "w7d05", "4-8" }, { "w9d07", "4-9" }
    };
    settings.Add("cyberSpace", false, "Cyber Space levels autosplitting (story mode)");
    foreach (var entry in CyberSpaceLevels) settings.Add(entry.Key + "_story", true, entry.Value, "cyberSpace");
    settings.Add("cyberSpace_arcade", true, "Cyber Space levels autosplitting (arcade mode)");
    foreach (var entry in CyberSpaceLevels) settings.Add(entry.Key + "_arcade", true, entry.Value, "cyberSpace_arcade");
    settings.Add("w9d07_soon", true, "Split as soon as possible when completing this level", "w9d07_arcade");

    // Constants
    vars.STATUS_TOPMENU = "TopMenu"; // Main menu screen
    vars.STATUS_NEWGAMEMENU = "NewGameMenu"; // New game
    vars.STATUS_CONTINUEMENU = "ContinueMenu"; // Continue screen - save selection
    vars.STATUS_ARCADEMODE = "ArcadeMode"; // Arcade mode menu
    vars.STATUS_SELECTLANGUAGE = "SelectLanguage"; // Select language
    vars.STATUS_CHANGINGLANGUAGE = "ChangingLanguage";
    vars.STATUS_VIEWLICENSE = "ViewLicense"; // Copyright menu
    vars.STATUS_VIEWQRCODE = "ViewQRCode"; // User's manual
    vars.STATUS_EXTRACONTENTS = "ExtraContents"; // DLC menu
    vars.STATUS_CHECKQUIT = "CheckQuit"; // Asking for confirmation before quitting the game
    vars.STATUS_QUIT = "Quit"; // Quitting the main menu
    vars.STATUS_PLAYTOP = "PlayTop"; //Normal gameplay
    vars.STATUS_FINISH = "Finish"; // Reached the end of a level
    vars.STATUS_RESULT = "Result"; // Results screen
    vars.STATUS_BUILD = "Build"; // Used while "building" the level, regardless of the actual load. Don't use iT

    //vars.STATE2_LASTBOSSENDQTE = "LastBossEndQte"; 

    // Basic variables
    vars.AccumulatedIGT = TimeSpan.Zero;
    vars.isInArcade = false;
    vars.EndQTECount = 0;

    // Islands
    // w1r03 - Chronos Island
    // w2r01 - Ares Island
    // w3r01 - Chaos Island
    // w1r05 - Rhea Island
    // w1r04 - Ouranos Island

	if (timer.CurrentTimingMethod == TimingMethod.RealTime)
    {        
        var timingMessage = MessageBox.Show (
                "This autosplitter supports Time without Loads (Game Time).\n"+
                "LiveSplit is currently set to show Real Time (RTA).\n"+
                "Would you like to set the timing method to Game Time?",
                "LiveSplit | Sonic Frontiers",
                MessageBoxButtons.YesNo,MessageBoxIcon.Question
        );
        if (timingMessage == DialogResult.Yes)
            timer.CurrentTimingMethod = TimingMethod.GameTime;
    }
}

update
{
    // Update the main variables
    vars.GameModeExtensionCount = vars.GetGameModeExtensionCount();
    vars.APPLICATIONSEQUENCE = vars.GetAPPLICATIONSEQUENCE();
    current.Status = vars.GetStatus();
    current.IGT = vars.GetIGT();
    current.LevelID = vars.CURRENTSTAGE();
    current.isInQTE = vars.QTEinput() == vars.QTEADDRESS;

    var tutorial = vars.TUTORIALSTAGE();
    current.isInTutorial = tutorial != null && tutorial.Contains("w") && tutorial.Contains("t");
    
    // if the timer is not running (eg. a run has been reset) these variables need to be reset
    if (timer.CurrentPhase == TimerPhase.NotRunning)
    {
        vars.AccumulatedIGT = TimeSpan.Zero;
        vars.isInArcade = (vars.ARCADEFLAG() & 1) != 0 || current.Status == vars.STATUS_ARCADEMODE;
    }

    // Accumulate the time if the IGT resets
    if (old.IGT != TimeSpan.Zero && current.IGT == TimeSpan.Zero)
        vars.AccumulatedIGT += old.IGT;

    // Final split QTE stuff
    if (current.LevelID == "w5r01")
    {
        if (old.isInQTE && !current.isInQTE)
            vars.EndQTECount++;
    } else if (vars.EndQTECount > 0) {
        vars.EndQTECount = 0;
    }
}

gameTime
{
    if (vars.isInArcade)
        return current.IGT + vars.AccumulatedIGT;
}

isLoading
{
    return vars.isInArcade ? true : (current.LevelID == "w0r01" ? false : vars.GameModeExtensionCount == 0) || (current.isInTutorial && !current.LevelID.Contains("r"));
}

start
{
    if (current.Status == vars.STATUS_QUIT && old.Status == vars.STATUS_NEWGAMEMENU) // Story mode
    {
        return settings["newGame"];
    } else if (settings["arcade"] && vars.isInArcade) {  // Arcade mode
        if (settings["startonw6d01"])
        {
            return current.LevelID == "w6d01" && old.LevelID == "w0r01";
        } else {
            return current.LevelID != old.LevelID && old.LevelID == "w0r01";
        }
    }
}

reset
{
    return current.Status == vars.STATUS_TOPMENU && old.Status != current.Status;
}

split
{
    if (vars.isInArcade)
    {
        if (old.LevelID == "w9d07")
        {
            if (!settings[old.LevelID + "_arcade"])
                return false;
            if (settings["w9d07_soon"])
                return old.Status != vars.STATUS_FINISH && current.Status == vars.STATUS_FINISH;
            else
                return old.Status == vars.STATUS_RESULT && current.Status != vars.STATUS_RESULT;
        }
        else
        {
            return old.Status == vars.STATUS_RESULT && current.Status != vars.STATUS_RESULT;
        }
    } else {
        if (current.LevelID == "w5r01" && vars.EndQTECount == 3)
        {
            vars.EndQTECount = 0;
            return settings["finalQTE"];
        } else {
            return old.Status == vars.STATUS_RESULT && current.Status != vars.STATUS_RESULT && settings[old.LevelID + "_story"];
        }
    }
}
