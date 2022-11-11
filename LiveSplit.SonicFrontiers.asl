// Load time remover for Sonic Frontiers
// Coding: Jujstme
// contacts: just.tribe@gmail.com
// Version: 1.0.3 (Nov 10th, 2022)

state("SonicFrontiers") {}

init
{
    // Constant offsets
    const int APPLICATION = 0x80,
        APPLICATIONSEQUENCE = 0x8,
        GAMEMODE = 0x78,
        GAMEMODEEXTENSION = 0xB0;

    // Sigscanning for the base address
    var checkptr = (Action<IntPtr>)((p) => { if (p == IntPtr.Zero) throw new NullReferenceException("Sigscanning failed!"); });
    var scanner = new SignatureScanner(game, modules.First().BaseAddress, modules.First().ModuleMemorySize);
    var ptr = scanner.Scan(new SigScanTarget(1, "E8 ???????? 4C 8B 40 70") { OnFound = (p, s, addr) => {
                var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3;
                tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                return tempAddr;
    }});
    checkptr(ptr);

    // Using DeepPointer instead of a memorywatcher because we need to do quirky stuff during the update action.
    // Well'use .Deref instead.
    // Luckily we won't need to do all the weird stuff you'll see below
    vars.CURRENTSTAGE = new DeepPointer(ptr, APPLICATION, APPLICATIONSEQUENCE, 0xA0);
    vars.TUTORIALSTAGE = new DeepPointer(ptr, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, 0xF8);
    vars.ARCADEFLAG = new DeepPointer(ptr, APPLICATION, APPLICATIONSEQUENCE, 0x122);

    // This value tells the game (and the autosplitter) the number of subclasses loaded in the main GAMEMODE class. It's used for the loops in the following functions.
    // It's the only MemoryWatcher used in this script.
    vars.GameModeExtensionCount = new MemoryWatcher<int>(new DeepPointer(ptr, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION + 0x8)) { FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull };

    //
    // Recovering the addresses for some RTTI stuff
    //

    // StageTimeExtension - this is used to identify the active instance of this class. Used for IGT
    var tempaddr = scanner.Scan(new SigScanTarget(1, "E9 ???????? 0F 86 15 8A 59 FF") { OnFound = (p, s, addr) => {
        var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x7;
        tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
        return tempAddr;
    }});
    checkptr(tempaddr);
    var StageTimeExtension = (long)tempaddr;

    // HsmExtension - Hedgehog Scene Manager, maybe? Anyway, it's used to look for the status variable
    tempaddr = scanner.Scan(new SigScanTarget(1, "E8 ???????? 48 8B F8 48 8D 55 C0") { OnFound = (p, s, addr) => {
        var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x9;
        tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
        return tempAddr;
    }});
    checkptr(tempaddr);
    var HsmExtension = (long)tempaddr;

    // QTE management data - we need the function's address to identify whether we are in a QTE or not
    vars.QTEinput = new DeepPointer(ptr, 0x70, 0xD0, 0x28, 0x0, 0x0);
    tempaddr = scanner.Scan(new SigScanTarget(13, "C7 83 ???????? ???????? 48 8D 05 ???????? 48 89 BB") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
    checkptr(tempaddr);
    vars.QTEADDRESS = (long)tempaddr;

    // This ensures the real IGT is always caught
    vars.GetIGT = (Func<Process, int, float>)((p, ii) => {
        if (ii == 0)
            return 0f;
        float value = 0f;
        for (int i = 0; i < ii; i++)
        {
            var q = new DeepPointer(ptr, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x0).Deref<long>(p);
            if (q == StageTimeExtension)
                return new DeepPointer(ptr, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x28).Deref<float>(p);
        }
        return value;
    });

    // Very important variable. Essentially tells us the current status of the game
    vars.GetStatus = (Func<Process, int, string>)((p, ii) => {
        if (ii == 0)
            return string.Empty;
        string value = string.Empty;
        for (int i = 0; i < ii; i++)
        {
            var q = new DeepPointer(ptr, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x0).Deref<long>(p);
            if (q == HsmExtension)
            return new DeepPointer(ptr, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x60, 0x20, 0x0).DerefString(p, 255);
        }
        return value;
    });

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
    vars.GameModeExtensionCount.Update(game);
    current.Status = vars.GetStatus(game, vars.GameModeExtensionCount.Current);
    current.IGT = TimeSpan.FromSeconds(Math.Truncate(vars.GetIGT(game, vars.GameModeExtensionCount.Current) * 100) / 100);
    current.LevelID = vars.CURRENTSTAGE.DerefString(game, 5);
    current.isInQTE = vars.QTEinput.Deref<long>(game) == vars.QTEADDRESS;

    var tutorial = vars.TUTORIALSTAGE.DerefString(game, 5);
    current.isInTutorial = tutorial != null && tutorial.Contains("w") && tutorial.Contains("t");
    
    // if the timer is not running (eg. a run has been reset) these variables need to be reset
    if (timer.CurrentPhase == TimerPhase.NotRunning)
    {
        vars.AccumulatedIGT = TimeSpan.Zero;
        vars.isInArcade = (vars.ARCADEFLAG.Deref<byte>(game) & 1) != 0 || current.Status == vars.STATUS_ARCADEMODE;
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
    return vars.isInArcade ? true : (current.LevelID == "w0r01" ? false : vars.GameModeExtensionCount.Current == 0) || (current.isInTutorial && !current.LevelID.Contains("r"));
    // return vars.isInArcade ? true : vars.GameModeExtensionCount.Current == 0 || (current.isInTutorial && !current.LevelID.Contains("r"));
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
    return current.Status == vars.STATUS_TOPMENU && old.Status == current.Status;
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
