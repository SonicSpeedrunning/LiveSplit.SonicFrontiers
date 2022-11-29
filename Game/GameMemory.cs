using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        // Pointers and offsets
        private readonly Dictionary<string, IntPtr> pointers = new Dictionary<string, IntPtr>();
        private readonly Dictionary<string, int> offsets = new Dictionary<string, int>();

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

            IGT = new FakeMemoryWatcher<TimeSpan>(() => pointers["StageTimeExtension"] == IntPtr.Zero ? TimeSpan.Zero : TimeSpan.FromSeconds(Math.Truncate(new DeepPointer(pointers["StageTimeExtension"] + 0x28).Deref<float>(GameProcess.Game) * 100) / 100)) { Current = TimeSpan.Zero };
            Status = new FakeMemoryWatcher<string>(() => pointers["HsmExtension"] == IntPtr.Zero ? default : new DeepPointer(pointers["HsmExtension"] + 0x60, 0x20, 0x0).DerefString(GameProcess.Game, 255));
            LevelID = new FakeMemoryWatcher<string>(() => new DeepPointer(pointers["APPLICATIONSEQUENCE"] + 0xA0).DerefString(GameProcess.Game, 5));
            StoryModeCyberSpaceCompletionFlag = new FakeMemoryWatcher<bool>(() => !IsInArcade && (LevelID.Current == null || !LevelID.Current.Contains("w") || !LevelID.Current.Contains("r0")) && (Status.Current == SonicFrontiers.Status.Result || StoryModeCyberSpaceCompletionFlag.Old));
            QTEStatus = new FakeMemoryWatcher<byte>(() => !IsInEndQTE ? default : new DeepPointer(pointers["baseAddress"], 0x70, 0xD0, 0x28, 0x0, 0x254).Deref<byte>(GameProcess.Game));
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
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            GetApplicationSequence();
            GetStagePointers();

            // Update the old watchers
            IGT.Update();
            Status.Update();
            LevelID.Update();
            StoryModeCyberSpaceCompletionFlag.Update();

            var tutorialstage = new DeepPointer(pointers["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], 0xF8).DerefString(GameProcess.Game, 5);
            IsInTutorial = tutorialstage != null && tutorialstage.Contains("w") && tutorialstage.Contains("t");

            QTEStatus.Update();
            IsInEndQTE = LevelID.Current == SpecialLevels.TheEndBoss && (IntPtr)new DeepPointer(pointers["baseAddress"], 0x70, 0xD0, 0x28, 0x0, 0x0).Deref<long>(GameProcess.Game) == RTTI["EventQTEInput::evt::app"];
            EndQTECount.Update();


            // If the timer is not running (eg. a run has been reset) these variables need to be reset
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                if (AccumulatedIGT != TimeSpan.Zero) AccumulatedIGT = TimeSpan.Zero;
                if (VisitedIslands.Count > 0) VisitedIslands.Clear();
                if (VisitedFishingLevels.Count > 0) VisitedFishingLevels.Clear();
                IsInArcade = Status.Current == SonicFrontiers.Status.ArcadeMode || new DeepPointer(pointers["APPLICATIONSEQUENCE"] + 0x122).Deref<byte>(GameProcess.Game).BitCheck(0);
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
            
            pointers["baseAddress"] = scanner.ScanOrThrow(new SigScanTarget(1, "E8 ???????? 4C 8B 78 70")
            {
                OnFound = (p, s, addr) =>
                {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });

            // Game patch offset
            pointers["baseFocus"] = scanner.ScanOrThrow(new SigScanTarget("?? 36 48 8B 52 28"));

            // APPLICATION
            offsets["APPLICATION"] = GameProcess.Game.ReadValue<int>(scanner.ScanOrThrow(new SigScanTarget(3, "48 8B 99 ???????? 48 8B F9 48 8B 81 ???????? 48 8D 34 C3 48 3B DE 74 21")));

            // GAMEMODE
            offsets["GAMEMODE"] = 0x78; //GameProcess.Game.ReadValue<byte>(scanner.ScanOrThrow(new SigScanTarget(1, "74 31 48 8D 55 E0") { OnFound = (p, s, addr) => addr + 0x31 + 0x4 }));

            // GAMEMODEEXTENSION
            offsets["GAMEMODEEXTENSION"] = GameProcess.Game.ReadValue<int>(scanner.ScanOrThrow(new SigScanTarget(8, "E8 ???????? 48 8B BB ???????? 48 8B 83 ???????? 4C 8D 34 C7")));

            // new stuff - work in progress
            /*
            offsets["GLOBALINSTANCES_1"] = 0x70;
            offsets["GLOBALINSTANCES_2"] = 0x658; //0x130;
            offsets["BOSS"] = 0x8;
            offsets["BOSSDATA"] = 0x130;
            //
            offsets["BOSSSTATE"] = 0xA8;

            offsets["SAVEDATA_1"] = 0x28;
            offsets["SAVEDATA_2"] = 0x110;
            offsets["SAVEDATA_3"] = 0x40;
            */

            // RTTI
            RTTI = new RTTI(scanner);
            RTTI.AddEntry(scanner, "ApplicationSequenceExtension::game::app");
            RTTI.AddEntry(scanner, "GameModeHsmExtension::game::app");
            RTTI.AddEntry(scanner, "GameModeStageTimeExtension::game::app");
            //RTTI.AddEntry(scanner, "BossRifle::app");  --> check pointer baseAddress, 0x70, 0x658, 0x8?
            RTTI.AddEntry(scanner, "EventQTEInput::evt::app");
            RTTI.ClearDict();

            WFocusChange?.Invoke(this, EventArgs.Empty);
        }

        private void GetApplicationSequence()
        {
            pointers["APPLICATIONSEQUENCE"] = IntPtr.Zero;

            byte ApplicationSequenceCount = new DeepPointer(pointers["baseAddress"], offsets["APPLICATION"] + 0x8).Deref<byte>(GameProcess.Game);
            if (ApplicationSequenceCount == 0) return;

            IntPtr ApplicationSequence = (IntPtr)new DeepPointer(pointers["baseAddress"], offsets["APPLICATION"]).Deref<long>(GameProcess.Game);
            GameProcess.Game.ReadBytes(ApplicationSequence, ApplicationSequenceCount * 8, out byte[] array);

            for (int i = 0; i < ApplicationSequenceCount; i++)
            {
                IntPtr InstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);

                if ((IntPtr)GameProcess.Game.ReadValue<long>(InstanceAddress) == RTTI["ApplicationSequenceExtension::game::app"])
                {
                    pointers["APPLICATIONSEQUENCE"] = InstanceAddress;
                    break;
                }
            }
        }

        private void GetStagePointers()
        {
            pointers["HsmExtension"] = IntPtr.Zero;
            pointers["StageTimeExtension"] = IntPtr.Zero;

            offsets["GameModeExtensionCount"] = new DeepPointer(pointers["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"] + 0x8).Deref<byte>(GameProcess.Game);

            if (offsets["GameModeExtensionCount"] == 0)
                return;
            
            IntPtr GameMode = (IntPtr)new DeepPointer(pointers["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"]).Deref<long>(GameProcess.Game);
            GameProcess.Game.ReadBytes(GameMode, offsets["GameModeExtensionCount"] * 8, out byte[] array);

            for (int i = 0; i < offsets["GameModeExtensionCount"]; i++)
            {
                IntPtr InstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);
                IntPtr temp = (IntPtr)GameProcess.Game.ReadValue<long>(InstanceAddress);

                if (temp == RTTI["GameModeHsmExtension::game::app"]) pointers["HsmExtension"] = InstanceAddress;
                if (temp == RTTI["GameModeStageTimeExtension::game::app"]) pointers["StageTimeExtension"] = InstanceAddress;

                if (pointers["HsmExtension"] != IntPtr.Zero && pointers["StageTimeExtension"] != IntPtr.Zero) return;
            }
        }

         
        /*
         * Work in progress
         * 

        private void GetIndexPointers2()
        {
            int count = new DeepPointer(pointers["baseAddress"], offsets["GLOBALINSTANCES_1"], offsets["GLOBALINSTANCES_2"] + 0x8).Deref<int>(GameProcess.Game);

            if (count < 1 || count > 2048)
                return;

            IntPtr GlobalInstances = (IntPtr)new DeepPointer(pointers["baseAddress"], offsets["GLOBALINSTANCES_1"], offsets["GLOBALINSTANCES_2"]).Deref<long>(GameProcess.Game);
            GameProcess.Game.ReadBytes(GlobalInstances, count * 8, out byte[] array);

            for (int i = 0; i < count; i++)
            {
                IntPtr InstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);
                if ((IntPtr)GameProcess.Game.ReadValue<long>(InstanceAddress) == RTTI["BossRifle::app"])
                {
                    pointers["BossRifle"] = InstanceAddress;
                    break;
                }
            }
        }
        */

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

        public event EventHandler WFocusChange;
        public void ScreenFocus(bool enabled)
        {
            if (GameProcess.InitStatus == GameInitStatus.NotStarted) return;
            if (enabled) GameProcess.Game.WriteValue<byte>(pointers["baseFocus"], 0xEB);
            if (!enabled) GameProcess.Game.WriteValue<byte>(pointers["baseFocus"], 0x74);
        }
    }
}