// Load time remover for Sonic Frontiers
// Coding: Jujstme
// contacts: just.tribe@gmail.com
// Version: 1.0.0

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

    // Default state
    current.IGT = TimeSpan.Zero;
}

startup
{
    settings.Add("useIGT", false, "Use IGT instead of Loadless timing");
    vars.AccumulatedIGT = TimeSpan.Zero;
    vars.SanitizeString = (Func<string, string>)(i => vars.watchers[i + "_b"].Current == 0 ? "" : vars.watchers[i].Current );
}

update
{
    vars.watchers.UpdateAll(game);
    current.LevelID = vars.SanitizeString("LevelID");

    current.IGT = vars.watchers["IGT_flag"].Current > 2 ? TimeSpan.Zero : TimeSpan.FromSeconds(Math.Truncate(vars.watchers["IGT"].Current * 100) / 100);

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