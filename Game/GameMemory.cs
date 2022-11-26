using System;
using System.Collections.Generic;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        private IntPtr baseAddress;

        // RTTI vtables
        private RTTI RTTI;

        // State variables we're gonna use for the splitting logic 
        public FakeMemoryWatcher<TimeSpan> IGT { get; protected set; }
        public FakeMemoryWatcher<string> Status { get; protected set; }
        public FakeMemoryWatcher<string> LevelID { get; protected set; }
        private FakeMemoryWatcher<byte> QTEStatus { get; set; }
        public FakeMemoryWatcher<byte> EndQTECount { get; protected set; }
        public FakeMemoryWatcher<bool> StoryModeCyberSpaceCompletionFlag { get; protected set; }
        public TimeSpan AccumulatedIGT { get; protected set; }
        public List<string> VisitedIslands { get; protected set; }
        public List<string> VisitedFishingLevels { get; protected set; }
        public bool IsInTutorial { get; protected set; }
        public bool IsInArcade { get; protected set; }
        private bool IsInEndQTE { get; set; }
        public bool GameModeLoad => offsets["GameModeExtensionCount"] == 0;


        public Watchers(LiveSplitState state, params string[] gameNames)
        {
            this.state = state;

            offsets = new Dictionary<string, int>();

            IGT = new FakeMemoryWatcher<TimeSpan>(GetIGT) { Current = TimeSpan.Zero };
            Status = new FakeMemoryWatcher<string>(GetStatus);
            LevelID = new FakeMemoryWatcher<string>(() => new DeepPointer(baseAddress, offsets["APPLICATION"], offsets["APPLICATIONSEQUENCE"], 0xA0).DerefString(GameProcess.Game, 5));
            StoryModeCyberSpaceCompletionFlag = new FakeMemoryWatcher<bool>(() => !IsInArcade && (LevelID.Current == null || !LevelID.Current.Contains("w") || !LevelID.Current.Contains("r0")) && (Status.Current == SonicFrontiers.Status.Result || StoryModeCyberSpaceCompletionFlag.Old));
            QTEStatus = new FakeMemoryWatcher<byte>(() => !IsInEndQTE ? default : new DeepPointer(baseAddress, 0x70, 0xD0, 0x28, 0x0, 0x254).Deref<byte>(GameProcess.Game));
            EndQTECount = new FakeMemoryWatcher<byte>(GetQTECount);

            // Default state
            AccumulatedIGT = TimeSpan.Zero;
            VisitedIslands = new List<string>();
            VisitedFishingLevels = new List<string>();
            IsInTutorial = false;
            IsInArcade = false;
            IsInEndQTE = false;

            GameProcess = new ProcessHook(gameNames);
        }

        public void Update()
        {
            // In the update sequence, first of all we need to retrieve the new dynamic offsets

            // Even though it never happened in my case, this offset can theoretically change dynamically
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            offsets["APPLICATIONSEQUENCE"] = GetApplicationSequence();
            offsets["GameModeExtensionCount"] = new DeepPointer(baseAddress, offsets["APPLICATION"], offsets["APPLICATIONSEQUENCE"], offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"] + 0x8).Deref<byte>(GameProcess.Game);

            // Update the old watchers
            IGT.Update();
            Status.Update();
            LevelID.Update();
            StoryModeCyberSpaceCompletionFlag.Update();

            var tutorialstage = new DeepPointer(baseAddress, offsets["APPLICATION"], offsets["APPLICATIONSEQUENCE"], offsets["GAMEMODE"], 0xF8).DerefString(GameProcess.Game, 5);
            IsInTutorial = tutorialstage != null && tutorialstage.Contains("w") && tutorialstage.Contains("t");

            QTEStatus.Update();
            IsInEndQTE = LevelID.Current == SpecialLevels.TheEndBoss && new DeepPointer(baseAddress, 0x70, 0xD0, 0x28, 0x0, 0x0).Deref<long>(GameProcess.Game) == RTTI["EventQTEInput::evt::app"];
            EndQTECount.Update();


            // If the timer is not running (eg. a run has been reset) these variables need to be reset
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                if (AccumulatedIGT != TimeSpan.Zero) AccumulatedIGT = TimeSpan.Zero;
                if (VisitedIslands.Count > 0) VisitedIslands.Clear();
                if (VisitedFishingLevels.Count > 0) VisitedFishingLevels.Clear();
                IsInArcade = Status.Current == SonicFrontiers.Status.ArcadeMode || new DeepPointer(baseAddress, offsets["APPLICATION"], offsets["APPLICATIONSEQUENCE"], 0x122).Deref<byte>(GameProcess.Game).BitCheck(0);
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

            // APPLICATION
            offsets["APPLICATION"] = GameProcess.Game.ReadValue<int>(scanner.ScanOrThrow(new SigScanTarget(3, "48 8B 99 ???????? 48 8B F9 48 8B 81 ???????? 48 8D 34 C3 48 3B DE 74 21")));

            // GAMEMODE
            offsets["GAMEMODE"] = GameProcess.Game.ReadValue<byte>(scanner.ScanOrThrow(new SigScanTarget(1, "74 31 48 8D 55 E0") { OnFound = (p, s, addr) => addr + 0x31 + 0x4 }));

            // GAMEMODEEXTENSION
            offsets["GAMEMODEEXTENSION"] = GameProcess.Game.ReadValue<int>(scanner.ScanOrThrow(new SigScanTarget(8, "E8 ???????? 48 8B BB ???????? 48 8B 83 ???????? 4C 8D 34 C7")));

            RTTI = new RTTI(scanner, GameProcess.Game,
                "ApplicationSequenceExtension::game::app",
                "GameModeHsmExtension::game::app",
                "GameModeStageTimeExtension::game::app",
                "EventQTEInput::evt::app"
            );
        }

        private int GetApplicationSequence()
        {
            int value = new DeepPointer(baseAddress, offsets["APPLICATION"] + 0x8).Deref<byte>(GameProcess.Game);
            if (value == 0) return default;
            
            for (int i = 0; i < value; i++)
            {
                var g = new DeepPointer(baseAddress, offsets["APPLICATION"], 0x8 * i, 0x0).Deref<long>(GameProcess.Game);
                if (g == RTTI["ApplicationSequenceExtension::game::app"]) return 0x8 * i;
            }
            
            return default;
        }

        private string GetStatus()
        {
            if (offsets["GameModeExtensionCount"] == 0)
                return string.Empty;

            for (int i = 0; i < offsets["GameModeExtensionCount"]; i++)
            {
                var q = new DeepPointer(baseAddress, offsets["APPLICATION"], offsets["APPLICATIONSEQUENCE"], offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"], 0x8 * i, 0x0).Deref<long>(GameProcess.Game);

                if (q == RTTI["GameModeHsmExtension::game::app"])
                    return new DeepPointer(baseAddress, offsets["APPLICATION"], offsets["APPLICATIONSEQUENCE"], offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"], 0x8 * i, 0x60, 0x20, 0x0).DerefString(GameProcess.Game, 255);
            }

            return string.Empty;
        }

        private TimeSpan GetIGT()
        {
            if (offsets["GameModeExtensionCount"] == 0)
                return TimeSpan.Zero;
            
            for (int i = 0; i < offsets["GameModeExtensionCount"]; i++)
            {
                var q = new DeepPointer(baseAddress, offsets["APPLICATION"], offsets["APPLICATIONSEQUENCE"], offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"], 0x8 * i, 0x0).Deref<long>(GameProcess.Game);
                if (q == RTTI["GameModeStageTimeExtension::game::app"])
                    return TimeSpan.FromSeconds(Math.Truncate(new DeepPointer(baseAddress, offsets["APPLICATION"], offsets["APPLICATIONSEQUENCE"], offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"], 0x8 * i, 0x28).Deref<float>(GameProcess.Game) * 100) / 100);
            }

            return TimeSpan.Zero;
        }

        private byte GetQTECount()
        {
            // Reset QTEcount if it reached 3 in the previous update cycle
            if ((EndQTECount.Current == 3 && !IsInEndQTE) || EndQTECount.Current > 3)
                return default;

            // Final split QTE stuff
            if (LevelID.Current != SpecialLevels.TheEndBoss)
                return default;

            if (IsInEndQTE && QTEStatus.Changed && QTEStatus.Current == 1)
                return (byte)(EndQTECount.Current + 1);

            if (QTEStatus.Changed && QTEStatus.Current == 2)
                return default;

            return EndQTECount.Current;
        }

        public bool SwitchedIsland()
        {
            if (LevelID.Old == null || LevelID.Current == null || LevelID.Old == LevelID.Current || LevelID.Old == SpecialLevels.MainMenu || LevelID.Current == SpecialLevels.MainMenu)
                return false;

            return LevelID.Old.Contains("w") && LevelID.Old.Contains("r0") && LevelID.Current.Contains("w") && LevelID.Current.Contains("r0");
        }
    }
}