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
        private readonly Dictionary<string, IntPtr> addresses = new Dictionary<string, IntPtr>();
        private readonly Dictionary<string, int> offsets = new Dictionary<string, int>();

        // RTTI vtables
        private RTTI RTTI;

        // State variables we're gonna use for the splitting logic 

        // IGT
        public FakeMemoryWatcher<TimeSpan> IGT { get; protected set; }
        public TimeSpan AccumulatedIGT { get; protected set; } = TimeSpan.Zero;

        // Level ID and status
        public FakeMemoryWatcher<string> Status { get; protected set; }
        public FakeMemoryWatcher<byte> LevelID { get; protected set; }

        // QTE stuff for the final boss
        private FakeMemoryWatcher<byte> QTEStatus { get; set; } // Internal state of the QTE. 0 = not resolved; 1 = successful; 2 = failed
        public FakeMemoryWatcher<byte> EndQTECount { get; protected set; }
        private bool IsInEndQTE { get; set; } = default;


        // Stage completion flags
        public FakeMemoryWatcher<bool> StoryModeCyberSpaceCompletionFlag { get; protected set; }
        public bool IsInTutorial { get; protected set; } = default;
        public bool IsInArcade { get; protected set; } = default;
        public bool GameModeLoad => offsets["GameModeExtensionCount"] == 0;


        // Story flags
        private byte[] Flags { get; set; }
        public Dictionary<string, FakeMemoryWatcher<bool>> SplitBools { get; protected set; }
        public List<string> AlreadyTriggeredBools { get; set; } = new List<string>();


        public Watchers(LiveSplitState LSstate, params string[] gameNames)
        {
            state = LSstate;

            IGT = new FakeMemoryWatcher<TimeSpan>(() => addresses["StageTimeExtension"] == IntPtr.Zero ? default : TimeSpan.FromSeconds(Math.Truncate(game.ReadValue<float>(addresses["StageTimeExtension"] + 0x28) * 100) / 100));
            Status = new FakeMemoryWatcher<string>(() => addresses["HsmExtension"] == IntPtr.Zero ? default : new DeepPointer(addresses["HsmExtension"] + 0x60, 0x20, 0x0).DerefString(game, 250));
            LevelID = new FakeMemoryWatcher<byte>(() => FakeEnums.LevelID.TryGetValue(game.ReadString(addresses["APPLICATIONSEQUENCE"] + 0xA0, 5) ?? " ", out var value) ? value : LevelID.Current);
            StoryModeCyberSpaceCompletionFlag = new FakeMemoryWatcher<bool>(() => !IsInArcade && LevelID.Current <= 29 && (Status.Current == FakeEnums.Status.Result || StoryModeCyberSpaceCompletionFlag.Old));

            QTEStatus = new FakeMemoryWatcher<byte>(() => !IsInEndQTE ? default : new DeepPointer(addresses["baseAddress"], 0x70, 0xD0, 0x28, 0x0, 0x254).Deref<byte>(game));
            EndQTECount = new FakeMemoryWatcher<byte>(GetQTECount);


            // Split bools
            SplitBools = new Dictionary<string, FakeMemoryWatcher<bool>>
            {
                // Kronos
                { "Kronos_Ninja",           GetFlag(0x1201, 2) },
                { "Kronos_Door",            GetFlag(0x1201, 0) },
                { "Kronos_Amy",             GetFlag(0x1784, 2) },
                { "Kronos_GigantosFirst",   GetFlag(0x1789, 7) },
                { "Kronos_GreenCE",         GetFlag(0x1783, 3) },
                { "Kronos_CyanCE",          GetFlag(0x1783, 4) },
                { "Kronos_Tombstones",      GetFlag(0xA48 , 3) },
                { "Kronos_BlueCE",          GetFlag(0x10C1, 0) },
                { "Kronos_RedCE",           GetFlag(0x10C1, 2) },
                { "Kronos_YellowCE",        GetFlag(0x10C1, 1) },
                { "Kronos_WhiteCE",         GetFlag(0x10C1, 3) },
                { "Kronos_GigantosStart",   new FakeMemoryWatcher<bool>( () => LevelID.Current == 50 && new DeepPointer(addresses["baseAddress"], 0x70, 0x658, 0x8, 0x130, 0x8, 0xA8, 0x0, 0x20, 0x0).DerefString(game, 255, " ") == "WalkingBase") },
                { "c50_story",              new FakeMemoryWatcher<bool>(() => LevelID.Old == 50 && LevelID.Current == 51) },
                { "c50_fishing",            new FakeMemoryWatcher<bool>(() => LevelID.Old == 70 && LevelID.Current == 50) },

                // Rhea
                { "Rhea_Tower1",            GetFlag(0xA5B, 5) },
                { "Rhea_Tower2",            GetFlag(0xA5B, 4) },
                { "Rhea_Tower3",            GetFlag(0xA5B, 3) },
                { "Rhea_Tower4",            GetFlag(0xA5B, 2) },
                { "Rhea_Tower5",            GetFlag(0xA5B, 1) },
                { "Rhea_Tower6",            GetFlag(0xA5B, 0) },
                { "c53_story",              new FakeMemoryWatcher<bool>(() => LevelID.Old == 53 && LevelID.Current == 54) },

                // Ouranos
                { "Ouranos_Bridge",         GetFlag(0xA48, 6) },
                { "Ouranos_SupremeDefeated", new FakeMemoryWatcher<bool>(() => LevelID.Old == 54 && LevelID.Current == 72) },
                { "Ouranos_BlueCE",         GetFlag(0x80C0, 7) },
                { "Ouranos_RedCE",          GetFlag(0x80C1, 0) },
                { "Ouranos_GreenCE",        GetFlag(0x80C1, 1) },
                { "Ouranos_YellowCE",       GetFlag(0x80C1, 2) },
                { "Ouranos_CyanCE",         GetFlag(0x80C1, 3) },
                { "Ouranos_WhiteCE",        GetFlag(0x80C1, 4) },
                { "c54_fishing",            new FakeMemoryWatcher<bool>(() => LevelID.Old == 70 && LevelID.Current == 54) }
            };


            GameProcess = new ProcessHook(gameNames);
        }

        public void Update()
        {
#if DEBUG
            var timer = new Stopwatch();
            timer.Start();
#endif
            // In the update sequence, first of all we need to retrieve the new dynamic offsets
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            GetPointerAddresses();

            Flags = new DeepPointer(addresses["baseAddress"], 0x28, 0x110, 0x40, 0x50).DerefBytes(game, 0x8100);
            foreach (var entry in SplitBools) entry.Value.Update();

            // Update the old watchers
            IGT.Update();
            Status.Update();
            LevelID.Update();
            StoryModeCyberSpaceCompletionFlag.Update();

            var tutorialstage = new DeepPointer(addresses["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], 0xF8).DerefString(game, 5);
            IsInTutorial = tutorialstage != null && tutorialstage.Contains("w") && tutorialstage.Contains("t");

            QTEStatus.Update();
            IsInEndQTE = LevelID.Current == 72 && (IntPtr)new DeepPointer(addresses["baseAddress"], 0x70, 0xD0, 0x28, 0x0, 0x0).Deref<long>(game) == RTTI["EventQTEInput::evt::app"];
            EndQTECount.Update();


            // If the timer is not running (eg. a run has been reset) these variables need to be reset
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                if (AccumulatedIGT != TimeSpan.Zero) AccumulatedIGT = TimeSpan.Zero;
                if (AlreadyTriggeredBools.Count > 0) AlreadyTriggeredBools.Clear();
                IsInArcade = Status.Current == FakeEnums.Status.ArcadeMode || new DeepPointer(addresses["APPLICATIONSEQUENCE"] + 0x122).Deref<byte>(game).BitCheck(0);
            }

            if (IGT.Current == TimeSpan.Zero && IGT.Old != TimeSpan.Zero)
                AccumulatedIGT += IGT.Old;

            // Seta the game time for arcade mode
            //if (state.CurrentPhase == TimerPhase.Running && IsInArcade)
            //    state.SetGameTime(IGT.Current + AccumulatedIGT);
#if DEBUG
            timer.Stop();
            Debug.WriteLine(timer.Elapsed.ToString());
#endif
        }

        // GetAddresses is essentially the equivalent of init in script-based autosplitters
        private void GetAddresses()
        {
            SignatureScanner scanner = new SignatureScanner(game, game.MainModuleWow64Safe().BaseAddress, game.MainModuleWow64Safe().ModuleMemorySize);
            
            addresses["baseAddress"] = scanner.ScanOrThrow(new SigScanTarget(1, "E8 ???????? 4C 8B 78 70")
            {
                OnFound = (p, s, addr) =>
                {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });

            // Game patch offset - This is used for the WFocus patch
            addresses["baseFocus"] = scanner.ScanOrThrow(new SigScanTarget("?? 36 48 8B 52 28"));

            offsets["APPLICATION"]       = 0x80; // GameProcess.Game.ReadValue<int>(scanner.ScanOrThrow(new SigScanTarget(3, "48 8B 99 ???????? 48 8B F9 48 8B 81 ???????? 48 8D 34 C3 48 3B DE 74 21")));
            offsets["GAMEMODE"]          = 0x78; //GameProcess.Game.ReadValue<byte>(scanner.ScanOrThrow(new SigScanTarget(1, "74 31 48 8D 55 E0") { OnFound = (p, s, addr) => addr + 0x31 + 0x4 }));
            offsets["GAMEMODEEXTENSION"] = 0xB0; // GameProcess.Game.ReadValue<int>(scanner.ScanOrThrow(new SigScanTarget(8, "E8 ???????? 48 8B BB ???????? 48 8B 83 ???????? 4C 8D 34 C7")));

            // RTTI
            RTTI = new RTTI(scanner,
                "ApplicationSequenceExtension::game::app",
                "GameModeHsmExtension::game::app",
                "GameModeStageTimeExtension::game::app",
                "EventQTEInput::evt::app"
                );

            // Perform game fixes, if needed
            WFocusChange?.Invoke(this, EventArgs.Empty);
        }

        private void GetPointerAddresses()
        {
            addresses["APPLICATIONSEQUENCE"] = IntPtr.Zero;
            addresses["HsmExtension"] = IntPtr.Zero;
            addresses["StageTimeExtension"] = IntPtr.Zero;
            offsets["GameModeExtensionCount"] = 0;


            byte ApplicationSequenceCount = new DeepPointer(addresses["baseAddress"], offsets["APPLICATION"] + 0x8).Deref<byte>(game);
            if (ApplicationSequenceCount != 0)
            {
                IntPtr ApplicationSequence = (IntPtr)new DeepPointer(addresses["baseAddress"], offsets["APPLICATION"]).Deref<long>(game);
                byte[] array = game.ReadBytes(ApplicationSequence, ApplicationSequenceCount * 8);

                for (int i = 0; i < ApplicationSequenceCount; i++)
                {
                    IntPtr InstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);
                    if ((IntPtr)GameProcess.Game.ReadValue<long>(InstanceAddress) == RTTI["ApplicationSequenceExtension::game::app"])
                    {
                        addresses["APPLICATIONSEQUENCE"] = InstanceAddress;
                        break;
                    }
                }

                offsets["GameModeExtensionCount"] = new DeepPointer(addresses["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"] + 0x8).Deref<byte>(game);
                if (offsets["GameModeExtensionCount"] != 0)
                {
                    IntPtr GameMode = (IntPtr)new DeepPointer(addresses["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"]).Deref<long>(game);
                    array = game.ReadBytes(GameMode, offsets["GameModeExtensionCount"] * 8);

                    for (int i = 0; i < offsets["GameModeExtensionCount"]; i++)
                    {
                        IntPtr InstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);
                        IntPtr temp = (IntPtr)GameProcess.Game.ReadValue<long>(InstanceAddress);

                        if (temp == RTTI["GameModeHsmExtension::game::app"]) addresses["HsmExtension"] = InstanceAddress;
                        if (temp == RTTI["GameModeStageTimeExtension::game::app"]) addresses["StageTimeExtension"] = InstanceAddress;

                        if (addresses["HsmExtension"] != IntPtr.Zero && addresses["StageTimeExtension"] != IntPtr.Zero)
                            break;
                    }
                }
            }
        }

        private byte GetQTECount()
        {
            // Reset QTEcount if it reached 3 in the previous update cycle
            if (LevelID.Current != 72 || (QTEStatus.Changed && QTEStatus.Current == 2) || (EndQTECount.Current == 3 && !IsInEndQTE) || EndQTECount.Current > 3)
                return default;

            if (IsInEndQTE && QTEStatus.Changed && QTEStatus.Current == 1)
                return (byte)(EndQTECount.Current + 1);

            return EndQTECount.Current;
        }

        public event EventHandler WFocusChange;
        public void ScreenFocus(bool enabled)
        {
            if (GameProcess.InitStatus == GameInitStatus.NotStarted) return;
            if (enabled) GameProcess.Game.WriteValue<byte>(addresses["baseFocus"], 0xEB);
            if (!enabled) GameProcess.Game.WriteValue<byte>(addresses["baseFocus"], 0x74);
        }

        private FakeMemoryWatcher<bool> GetFlag(int offset, byte bitCheck)
        {
            return new FakeMemoryWatcher<bool>(() => Flags[offset].BitCheck(bitCheck));
        }
    }
}