// Load time remover for Sonic Frontiers
// Coding: Jujstme
// contacts: just.tribe@gmail.com
// Version: 1.0.1 (Nov 11th, 2022)

state("SonicFrontiers") {}

init
{
    var scanner = new SignatureScanner(game, modules.First().BaseAddress, modules.First().ModuleMemorySize);
    var ptr = new SignatureScanner(game, modules.First().BaseAddress, modules.First().ModuleMemorySize).
        Scan(new SigScanTarget(1, "E8 ???????? 4C 8B 40") { OnFound = (p, s, addr) => {
            var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3;
            tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
            return tempAddr;
        }});
    if (ptr == IntPtr.Zero) throw new NullReferenceException();

    vars.watchers = new MemoryWatcherList();
    vars.watchers.Add(new MemoryWatcher<byte>(new DeepPointer(ptr, 0x80, 0x8, 0x78, 0xB0, 0x20, 0x30)) { Name = "IGT_flag", FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull });
    vars.watchers.Add(new MemoryWatcher<float>(new DeepPointer(ptr, 0x80, 0x8, 0x78, 0xB0, 0x20, 0x28)) { Name = "IGT", FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull });
    vars.watchers.Add(new MemoryWatcher<byte>(new DeepPointer(ptr, 0x80, 0x8, 0x78, 0xB8)) { Name = "statusflag", FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull });

    vars.watchers.Add(new StringWatcher(new DeepPointer(ptr, 0x80, 0x8, 0xA0), 5) { Name = "LevelID" });
    vars.watchers.Add(new MemoryWatcher<int>(new DeepPointer(ptr, 0x80, 0x8, 0xA0)) { Name = "LevelID_b", FailAction = MemoryWatcher.ReadFailAction.SetZeroOrNull });


    // Get the game status. Used for autostart and autosplitting purposes
    var StatusWatcher = new List<DeepPointer>();
    var StatusWatcher2 = new List<DeepPointer>();
    var offsets = new int[] { 1, 4, 5, 6, 7};
    foreach (var i in offsets)
    {
        StatusWatcher.Add(new DeepPointer(ptr, 0x80, 0x8, 0x78, 0xB0, 0x8 * i, 0x60, 0x20, 0x0));
        StatusWatcher2.Add(new DeepPointer(ptr, 0x80, 0x8, 0x78, 0xB0, 0x8 * i, 0x68, 0x20, 0x0));
    }
    vars.GetStatus = (Func<string>)(() => {
        string statuses = "";
        foreach (var entry in StatusWatcher) statuses += entry.DerefString(game, 255);
        return statuses;
    });
    vars.GetStatus2 = (Func<string>)(() => {
        string statuses = "";
        foreach (var entry in StatusWatcher2) statuses += entry.DerefString(game, 255);
        return statuses;
    });

    // Default state
    current.IGT = TimeSpan.Zero;
}

startup
{
    settings.Add("newGame", true, "Enable autostart on New Game");
    settings.Add("arcade", true, "Enable autostart on Arcade mode");
    settings.Add("useIGT", false, "Use IGT instead of Loadless timing");
    var CyberSpaceLevels = new Dictionary<string, string>{
        { "w6d01", "1-1" },
        { "w8d01", "1-2" },
        { "w9d04", "1-3" },
        { "w6d02", "1-4" },
        { "w7d04", "1-5" },
        { "w6d06", "1-6" },
        { "w9d06", "1-7" },
        { "w6d05", "2-1" },
        { "w8d03", "2-2" },
        { "w7d02", "2-3" },
        { "w7d06", "2-4" },
        { "w8d04", "2-5" },
        { "w6d03", "2-6" },
        { "w8d05", "2-7" },
        { "w6d04", "3-1" },
        { "w6d08", "3-2" },
        { "w8d02", "3-3" },
        { "w6d09", "3-4" },
        { "w6d07", "3-5" },
        { "w8d06", "3-6" },
        { "w7d03", "3-7" },
        { "w7d08", "4-1" },
        { "w9d02", "4-2" },
        { "w7d01", "4-3" },
        { "w9d03", "4-4" },
        { "w6d10", "4-5" },
        { "w7d07", "4-6" },
        { "w9d05", "4-7" },
        { "w7d05", "4-8" },
        { "w9d07", "4-9" },
    };
    settings.Add("cyberSpace", true, "Cyber Space levels autosplitting");
    foreach (var entry in CyberSpaceLevels) settings.Add(entry.Key, true, entry.Value, "cyberSpace");

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

    vars.STATE2_LASTBOSSENDQTE = "LastBossEndQte"; 

    vars.AccumulatedIGT = TimeSpan.Zero;
    vars.SanitizeString = (Func<string, string>)(i => vars.watchers[i + "_b"].Current == 0 ? "" : vars.watchers[i].Current );

    // Islands
    // w1r03 - Chronos Island
    // w2r01 - Ares Island
    // w3r01 - Chaos Island
    // w1r05 - Rhea Island
    // w1r04 - Ouranos Island
}

update
{
    vars.watchers.UpdateAll(game);

    current.Status = vars.GetStatus();

    current.LevelID = vars.SanitizeString("LevelID");

    current.IGT = vars.watchers["IGT_flag"].Current > 2 || current.LevelID.Contains("r") ? TimeSpan.Zero : TimeSpan.FromSeconds(Math.Truncate(vars.watchers["IGT"].Current * 100) / 100);

    // if the timer is not running (eg. a run has been reset) these variables need to be reset
    if (timer.CurrentPhase == TimerPhase.NotRunning)
        vars.AccumulatedIGT = TimeSpan.Zero;

    // Accumulate the time if the IGT resets
    if (old.IGT != TimeSpan.Zero && current.IGT == TimeSpan.Zero)
        vars.AccumulatedIGT += old.IGT;   
}

gameTime
{
    if (settings["useIGT"])
        return current.IGT + vars.AccumulatedIGT;
}

isLoading
{
    return settings["useIGT"] ? true : vars.watchers["statusflag"].Current == 0 || (vars.watchers["statusflag"].Current == 6 && !current.LevelID.Contains("r"));
}

start
{
    return
        (current.Status.Contains(vars.STATUS_QUIT) && old.Status.Contains(vars.STATUS_NEWGAMEMENU))
        || (current.LevelID == "w6d01" && old.LevelID == "w0r01");
}

reset
{
    return current.Status.Contains(vars.STATUS_TOPMENU) && old.Status == current.Status;
}

split
{
    return old.Status.Contains(vars.STATUS_RESULT) && !current.Status.Contains(vars.STATUS_RESULT) && settings[old.LevelID];
        
    // Final split
    // current.LevelID == "w5r01" && vars.watchers["FinalBoss_Health"].Current == 780 && vars.watchers["FinalBoss_Health"].Old == vars.watchers["FinalBoss_Health"].Current
}
