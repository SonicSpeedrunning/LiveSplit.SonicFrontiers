using System;
using System.Collections.Generic;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;

namespace LiveSplit.SonicFrontiers
{
    class Watchers
    {
        // Game process
        private readonly ProcessHook GameProcess;
        private IntPtr baseAddress;
        private readonly LiveSplitState state;

        // State variables we're gonna use for the splitting logic 
        public FakeMemoryWatcher<TimeSpan> IGT { get; protected set; }
        public FakeMemoryWatcher<string> Status { get; protected set; }
        public FakeMemoryWatcher<string> LevelID { get; protected set; }
        public FakeMemoryWatcher<bool> IsInQTE { get; protected set; }
        public TimeSpan AccumulatedIGT { get; protected set; }
        public FakeMemoryWatcher<bool> StoryModeCyberSpaceCompletionFlag { get; protected set; }
        public List<string> VisitedIslands { get; protected set; }
        public List<string> VisitedFishingLevels { get; protected set; }
        public bool IsInTutorial { get; protected set; }
        public bool IsInArcade { get; protected set; }
        public int EndQTECount { get; protected set; }
        public bool GameModeLoad => GameModeExtensionCount == 0;


        // Internal state variables
        private int APPLICATION;
        private int APPLICATIONSEQUENCE;
        private int GAMEMODE;
        private int GameModeExtensionCount;
        private int GAMEMODEEXTENSION;

        // "Static" state variables - meaning they never change once they get set
        private long APPLICATIONSEQUENCE_addr;
        private long QTEADDRESS;
        private long StageTimeExtension;
        private long HsmExtension;

        public Watchers(LiveSplitState state)
        {
            this.state = state;
            IGT = new FakeMemoryWatcher<TimeSpan>();
            Status = new FakeMemoryWatcher<string>();
            LevelID = new FakeMemoryWatcher<string>();
            IsInQTE = new FakeMemoryWatcher<bool>();
            AccumulatedIGT = TimeSpan.Zero;
            IsInTutorial = false;
            IsInArcade = false;
            EndQTECount = 0;
            StoryModeCyberSpaceCompletionFlag = new FakeMemoryWatcher<bool>();
            VisitedIslands = new List<string>();
            VisitedFishingLevels = new List<string>();

            GameProcess = new ProcessHook("SonicFrontiers");
        }

        public bool Update()
        {
            if (!GameProcess.IsGameHooked) return false;

            if (!GameProcess.GotAddresses)
                try { GameProcess.GotAddresses = GetAddresses(); } catch { GameProcess.GotAddresses = false; }

            if (!GameProcess.GotAddresses)
                return false;

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

            return true;
        }

        // GetAddresses is essentially the equivalent of init in script-based autosplitters
        private bool GetAddresses()
        {
            var scanner = new SignatureScanner(GameProcess.Game, GameProcess.Game.MainModuleWow64Safe().BaseAddress, GameProcess.Game.MainModuleWow64Safe().ModuleMemorySize);
            
            baseAddress = scanner.Scan(new SigScanTarget(1, "E8 ???????? 4C 8B 78 70")
            {
                OnFound = (p, s, addr) =>
                {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });
            if (baseAddress == IntPtr.Zero) return false;

            IntPtr ptr;

            // APPLICATION
            ptr = scanner.Scan(new SigScanTarget(3, "48 8B 99 ???????? 48 8B F9 48 8B 81 ???????? 48 8D 34 C3 48 3B DE 74 21"));
            if (ptr == IntPtr.Zero) return false;
            APPLICATION = GameProcess.Game.ReadValue<int>(ptr);

            // APPLICATIONSEQUENCE
            // Even though it never happened in my case, this offset can theoretically change dynamically
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            ptr = scanner.Scan(new SigScanTarget(7, "48 83 EC 20 48 8D 05 ???????? 48 89 51 08 48 89 01 48 89 CF 48 8D 05") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
            if (ptr == IntPtr.Zero) return false;
            APPLICATIONSEQUENCE_addr = (long)ptr;

            // GAMEMODE
            ptr = scanner.Scan(new SigScanTarget(1, "74 31 48 8D 55 E0") { OnFound = (p, s, addr) => addr + 0x31 + 0x4 });
            if (ptr == IntPtr.Zero) return false;
            GAMEMODE = GameProcess.Game.ReadValue<byte>(ptr);

            // GAMEMODEEXTENSION
            ptr = scanner.Scan(new SigScanTarget(8, "E8 ???????? 48 8B BB ???????? 48 8B 83 ???????? 4C 8D 34 C7"));
            if (ptr == IntPtr.Zero) return false;
            GAMEMODEEXTENSION = GameProcess.Game.ReadValue<int>(ptr);

            // StageTimeExtension class
            ptr = scanner.Scan(new SigScanTarget(1, "E9 ???????? 0F 86 15 8A 59 FF")
            {
                OnFound = (p, s, addr) => {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x7;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });
            if (ptr == IntPtr.Zero) return false;
            StageTimeExtension = (long)ptr;

            // HsmExtension - Hedgehog Scene Manager, maybe? Anyway, it's used to look for the status variable
            ptr = scanner.Scan(new SigScanTarget(1, "E8 ???????? 48 8B F8 48 8D 55 C0")
            {
                OnFound = (p, s, addr) => {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x9;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });
            if (ptr == IntPtr.Zero) return false;
            HsmExtension = (long)ptr;

            // QTEADDRESS
            ptr = scanner.Scan(new SigScanTarget(13, "C7 83 ???????? ???????? 48 8D 05 ???????? 48 89 BB") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
            if (ptr == IntPtr.Zero) return false;
            QTEADDRESS = (long)ptr;

            // End
            return true;
            // GotAddresses = true;
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

        internal bool GetArcadeFlag()
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

        public bool GetStoryModeCyberSpaceCompletionFlag()
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