using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;

namespace LiveSplit.SonicFrontiers
{
    class Watchers
    {
        // Game process
        private readonly ProcessHook GameProcess;
        private readonly LiveSplitState state;
        private IntPtr baseAddress;

        // State variables we're gonna use for the splitting logic 
        public FakeMemoryWatcher<TimeSpan> IGT { get; protected set; } = new FakeMemoryWatcher<TimeSpan>();
        public FakeMemoryWatcher<string> Status { get; protected set; } = new FakeMemoryWatcher<string>();
        public FakeMemoryWatcher<string> LevelID { get; protected set; } = new FakeMemoryWatcher<string>();
        public FakeMemoryWatcher<bool> IsInQTE { get; protected set; } = new FakeMemoryWatcher<bool>();
        public TimeSpan AccumulatedIGT { get; protected set; } = TimeSpan.Zero;
        public FakeMemoryWatcher<bool> StoryModeCyberSpaceCompletionFlag { get; protected set; } = new FakeMemoryWatcher<bool>();
        public List<string> VisitedIslands { get; protected set; } = new List<string>();
        public List<string> VisitedFishingLevels { get; protected set; } = new List<string>();
        public bool IsInTutorial { get; protected set; } = false;
        public bool IsInArcade { get; protected set; } = false;
        public int EndQTECount { get; protected set; } = 0;
        public bool GameModeLoad => GameModeExtensionCount == 0;


        // Internal state variables
        private int APPLICATION;
        private int APPLICATIONSEQUENCE;
        private int GAMEMODE;
        private int GameModeExtensionCount;
        private int GAMEMODEEXTENSION;

        // "Static" state variables - meaning they never change (or at least they shouldn't) once they get set
        private long APPLICATIONSEQUENCE_addr;
        private long QTEADDRESS;
        private long StageTimeExtension;
        private long HsmExtension;

        public Watchers(LiveSplitState state, params string[] gameNames)
        {
            this.state = state;
            GameProcess = new ProcessHook(gameNames);
        }

        public bool Init()
        {
            // This "init" function checks if the autosplitter has connected to the game
            // (if it has not, there's no point in going further) and starts a Task to
            // get the needed memory addresses for the other methods.
            if (!GameProcess.IsGameHooked)
                return false;

            // The purpose of this task is to limit the update cycle to 1 every 1.5 seconds
            // (instead of the usual 1 every 16 msec) in order to avoid wasting resources
            if (GameProcess.InitStatus == GameInitStatus.NotStarted)
                Task.Run(() =>
                {
                    GameProcess.InitStatus = GameInitStatus.InProgress;
                    try
                    {
                        GetAddresses();
                        GameProcess.InitStatus = GameInitStatus.Completed;
                    }
                    catch
                    {
                        Task.Delay(1500).Wait();
                        GameProcess.InitStatus = GameInitStatus.NotStarted;
                    }
                    // I'm running this manually because the signature scanner, especially
                    // if it runs several times, can take A LOT of memory, to the point of
                    // filling your RAM with several GB of useless data that doesn't get
                    // collected for some reason.
                    GC.Collect();
                });

            // At this point, if init has not been completed yet, return
            // false to avoid running the rest of the splitting logic.
            if (GameProcess.InitStatus != GameInitStatus.Completed)
                return false;

            return true;
        }

        public void Update()
        {
            // In the update sequence, first of all we need to retrieve the new dynamic offsets

            // Even though it never happened in my case, this offset can theoretically change dynamically
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            APPLICATIONSEQUENCE = GetApplicationSequence();
            GameModeExtensionCount = GetGameModeExtensionCount();

            // Update the old watchers
            IGT.Update();
            Status.Update();
            LevelID.Update();
            IsInQTE.Update();
            StoryModeCyberSpaceCompletionFlag.Update();

            // Find and define the new values
            IGT.Current = GetIGT();
            Status.Current = GetStatus();
            LevelID.Current = GetCurrentStage();
            IsInQTE.Current = GetQTEInput() == QTEADDRESS;
            StoryModeCyberSpaceCompletionFlag.Current = GetStoryModeCyberSpaceCompletionFlag();

            var tutorialstage = GetTutorialStage();
            IsInTutorial = tutorialstage != null && tutorialstage.Contains("w") && tutorialstage.Contains("t");


            // Reset QTEcount if it reached 3 in the previous update cycle
            if (EndQTECount >= 3)
                EndQTECount = 0;

            // Final split QTE stuff
            if (LevelID.Current == SpecialLevels.TheEndBoss)
            {
                if (IsInQTE.Old && !IsInQTE.Current)
                    EndQTECount++;
            }
            else if (EndQTECount > 0)
            {
                EndQTECount = 0;
            }

            // If the timer is not running (eg. a run has been reset) these variables need to be reset
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                if (AccumulatedIGT != TimeSpan.Zero) AccumulatedIGT = TimeSpan.Zero;
                if (VisitedIslands.Count > 0) VisitedIslands.Clear();
                if (VisitedFishingLevels.Count > 0) VisitedFishingLevels.Clear();
                IsInArcade = GetArcadeFlag() || Status.Current == SonicFrontiers.Status.ArcadeMode;
            }

            if (IGT.Current == TimeSpan.Zero && IGT.Old != TimeSpan.Zero)
                AccumulatedIGT += IGT.Old;

            // Seta the game time for arcade mode
            if (state.CurrentPhase == TimerPhase.Running && IsInArcade)
                state.SetGameTime(IGT.Current + AccumulatedIGT);
        }

        // GetAddresses is essentially the equivalent of init in script-based autosplitters
        private void GetAddresses()
        {
            var scanner = new SignatureScanner(GameProcess.Game, GameProcess.Game.MainModuleWow64Safe().BaseAddress, GameProcess.Game.MainModuleWow64Safe().ModuleMemorySize);
            
            baseAddress = scanner.ScanOrThrow(new SigScanTarget(1, "E8 ???????? 4C 8B 78 70")
            {
                OnFound = (p, s, addr) =>
                {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });

            IntPtr ptr;

            // APPLICATION
            ptr = scanner.ScanOrThrow(new SigScanTarget(3, "48 8B 99 ???????? 48 8B F9 48 8B 81 ???????? 48 8D 34 C3 48 3B DE 74 21"));
            APPLICATION = GameProcess.Game.ReadValue<int>(ptr);

            // APPLICATIONSEQUENCE
            // Even though it never happened in my case, this offset can theoretically change dynamically
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            ptr = scanner.ScanOrThrow(new SigScanTarget(7, "48 83 EC 20 48 8D 05 ???????? 48 89 51 08 48 89 01 48 89 CF 48 8D 05") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
            APPLICATIONSEQUENCE_addr = (long)ptr;

            // GAMEMODE
            ptr = scanner.ScanOrThrow(new SigScanTarget(1, "74 31 48 8D 55 E0") { OnFound = (p, s, addr) => addr + 0x31 + 0x4 });
            GAMEMODE = GameProcess.Game.ReadValue<byte>(ptr);

            // GAMEMODEEXTENSION
            ptr = scanner.ScanOrThrow(new SigScanTarget(8, "E8 ???????? 48 8B BB ???????? 48 8B 83 ???????? 4C 8D 34 C7"));
            GAMEMODEEXTENSION = GameProcess.Game.ReadValue<int>(ptr);

            // StageTimeExtension class
            ptr = scanner.ScanOrThrow(new SigScanTarget(1, "E9 ???????? 0F 86 15 8A 59 FF")
            {
                OnFound = (p, s, addr) => {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x7;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });
            StageTimeExtension = (long)ptr;

            // HsmExtension - Hedgehog Scene Manager, maybe? Anyway, it's used to look for the status variable
            ptr = scanner.ScanOrThrow(new SigScanTarget(1, "E8 ???????? 48 8B F8 48 8D 55 C0")
            {
                OnFound = (p, s, addr) => {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x9;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });
            HsmExtension = (long)ptr;

            // QTEADDRESS
            ptr = scanner.ScanOrThrow(new SigScanTarget(13, "C7 83 ???????? ???????? 48 8D 05 ???????? 48 89 BB") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
            QTEADDRESS = (long)ptr;
        }

        private int GetApplicationSequence()
        {
            int value = new DeepPointer(baseAddress, APPLICATION + 0x8).Deref<byte>(GameProcess.Game);
            
            if (value == 0)
                return 0;
            
            for (int i = 0; i < value; i++)
            {
                var g = new DeepPointer(baseAddress, APPLICATION, 0x8 * i, 0x0).Deref<long>(GameProcess.Game);
                if (g == APPLICATIONSEQUENCE_addr)
                    return 0x8 * i;
            }
            
            return 0;
        }

        private int GetGameModeExtensionCount()
        {
            // Yes, I know we're returning as a byte instead of an int.
            // This is intended so if the value is too high LiveSplit won't be stuck in an (almost) endless loop.
            return new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION + 0x8).Deref<byte>(GameProcess.Game);
        }

        private string GetStatus()
        {
            if (GameModeExtensionCount == 0)
                return string.Empty;

            for (int i = 0; i < GameModeExtensionCount; i++)
            {
                var q = new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x0).Deref<long>(GameProcess.Game);

                if (q == HsmExtension)
                    return new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x60, 0x20, 0x0).DerefString(GameProcess.Game, 255);
            }

            return string.Empty;
        }

        private TimeSpan GetIGT()
        {
            if (GameModeExtensionCount == 0)
                return TimeSpan.Zero;
            
            for (int i = 0; i < GameModeExtensionCount; i++)
            {
                var q = new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x0).Deref<long>(GameProcess.Game);
                if (q == StageTimeExtension)
                    return TimeSpan.FromSeconds(Math.Truncate(new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x28).Deref<float>(GameProcess.Game) * 100) / 100);
            }

            return TimeSpan.Zero;
        }

        private string GetCurrentStage()
        {
            return new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, 0xA0).DerefString(GameProcess.Game, 5);
        }

        private string GetTutorialStage()
        {
            return new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, 0xF8).DerefString(GameProcess.Game, 5);
        }

        private bool GetArcadeFlag()
        {
            byte i = new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, 0x122).Deref<byte>(GameProcess.Game);
            return (i & 1) != 0;
        }

        private long GetQTEInput()
        {
            return new DeepPointer(baseAddress, 0x70, 0xD0, 0x28, 0x0, 0x0).Deref<long>(GameProcess.Game);
        }

        public bool SwitchedIsland()
        {
            if (LevelID.Old == null || LevelID.Current == null || LevelID.Old == LevelID.Current || LevelID.Old == SpecialLevels.MainMenu || LevelID.Current == SpecialLevels.MainMenu)
                return false;

            return LevelID.Old.Contains("w") && LevelID.Old.Contains("r0") && LevelID.Current.Contains("w") && LevelID.Current.Contains("r0");
        }

        private bool GetStoryModeCyberSpaceCompletionFlag()
        {
            // Workaround for story mode stage end stuff
            // Avoid splitting when restarting a stage in story mode
            if (IsInArcade || (LevelID.Current.Contains("w") && LevelID.Current.Contains("r0")))
                return false;

            return Status.Current == SonicFrontiers.Status.Result || StoryModeCyberSpaceCompletionFlag.Old;
        }

        public void Dispose()
        {
            GameProcess.Dispose();
        }
    }
}