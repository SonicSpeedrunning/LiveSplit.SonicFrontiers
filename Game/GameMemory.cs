using System;
using System.Collections.Generic;
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

        // IGT
        public FakeMemoryWatcher<TimeSpan> IGT { get; }
        public TimeSpan AccumulatedIGT { get; protected set; } = default;

        // Level ID and status
        public FakeMemoryWatcher<string> Status { get; }
        public FakeMemoryWatcher<byte> LevelID { get; }

        // QTE stuff for the final boss
        public FakeMemoryWatcher<byte> EndQTECount { get; }
        private readonly FakeMemoryWatcher<QTEResolveStatus> QTEStatus;
        private bool IsInEndQTE;

        // Stage completion flags
        public FakeMemoryWatcher<bool> StoryModeCyberSpaceCompletionFlag { get; }
        public bool IsInTutorial { get; protected set; } = default;
        public bool IsInArcade { get; protected set; } = default;
        public bool GameModeLoad => offsets["GameModeExtensionCount"] == 0;

        // Story flags
        private byte[] Flags;
        public Dictionary<string, FakeMemoryWatcher<bool>> SplitBools { get; }
        public List<string> AlreadyTriggeredBools { get; } = new List<string>();

        public Watchers(LiveSplitState LSstate, params string[] gameNames)
        {
            // Define LiveSplit's current state. We need this because we want to reset AccumulatedIGT and AlreadyTriggeredBools when a run is reset
            state = LSstate;

            // Define our FakeMemoryWatchers. I prefer using these instead of the standard MemoryWatchers because I can define a custom Update function
            IGT = new FakeMemoryWatcher<TimeSpan>(() => addresses["StageTimeExtension"].IsZero() ? default : TimeSpan.FromSeconds(Math.Truncate(game.ReadValue<float>(addresses["StageTimeExtension"] + 0x28) * 100) / 100));
            Status = new FakeMemoryWatcher<string>(() => addresses["HsmExtension"].IsZero() ? " " : new DeepPointer(addresses["HsmExtension"] + 0x60, 0x20, 0x0).DerefString(game, 250, " "));
            LevelID = new FakeMemoryWatcher<byte>(() => FakeEnums.LevelID.TryGetValue(game.ReadString(addresses["APPLICATIONSEQUENCE"] + 0xA0, 5) ?? " ", out var value) ? value : LevelID.Current);
            StoryModeCyberSpaceCompletionFlag = new FakeMemoryWatcher<bool>(() => !IsInArcade && LevelID.Current <= 29 && (Status.Current == FakeEnums.Status.Result || StoryModeCyberSpaceCompletionFlag.Old));
            QTEStatus = new FakeMemoryWatcher<QTEResolveStatus>(() => !IsInEndQTE ? QTEResolveStatus.NotCompleted : (QTEResolveStatus) new DeepPointer(addresses["baseAddress"], 0x70, 0xD0, 0x28, 0x0, 0x254).Deref<byte>(game));
            EndQTECount = new FakeMemoryWatcher<byte>(() => LevelID.Current != 72 || (QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Failed) || (EndQTECount.Current == 3 && !IsInEndQTE) || EndQTECount.Current > 3 ? default : IsInEndQTE && QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Completed ? (byte)(EndQTECount.Current + 1) : EndQTECount.Current);

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
                { "Kronos_GigantosStart",   new FakeMemoryWatcher<bool>(() => LevelID.Current == 50 && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x0, 0x20, 0x0).DerefString(game, 255, " ") == "WalkingBase") },
                { "Kronos_SuperSonic",      new FakeMemoryWatcher<bool>(() => LevelID.Current == 50 && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x0, 0x20, 0x0).DerefString(game, 255, " ") == "BattlePhaseParent") },
                { "c50_story",              new FakeMemoryWatcher<bool>(() => LevelID.Old == 50 && LevelID.Current == 51) },
                { "c50_fishing",            new FakeMemoryWatcher<bool>(() => LevelID.Old == 70 && LevelID.Current == 50) },

                // Ares
                { "Ares_Knuckles",          GetFlag(0x339D, 2) },
                { "Ares_WyvernFirst",       GetFlag(0x33A3, 3) },
                { "Ares_Water",             GetFlag(0xA4A,  3) },
                // { "Ares_KocoRoundup",       GetFlag(0x339B, 4) },
                { "Ares_Crane",             GetFlag(0xA4A,  6) },
                { "Ares_GreenCE",           GetFlag(0x339C, 0) },
                { "Ares_CyanCE",            GetFlag(0x339C, 1) },
                { "Ares_BlueCE",            GetFlag(0x2CC0, 7) },
                { "Ares_RedCE",             GetFlag(0x2CC1, 0) },
                { "Ares_YellowCE",          GetFlag(0x2CC1, 1) },
                { "Ares_WhiteCE",           GetFlag(0x2CC1, 2) },
                { "Ares_WyvernStart",       new FakeMemoryWatcher<bool>(() => LevelID.Current == 51 && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x8, 0x20, 0x0).DerefString(game, 255, " ") == "PatrolTop") },
                { "Ares_WyvernRun",         new FakeMemoryWatcher<bool>(() => LevelID.Current == 51 && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x8, 0x20, 0x0).DerefString(game, 255, " ") == "EventRise") },
                { "Ares_SuperSonic",        new FakeMemoryWatcher<bool>(() => LevelID.Current == 51 && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x8, 0x20, 0x0).DerefString(game, 255, " ") == "BattleTop") },
                { "c51_story",              new FakeMemoryWatcher<bool>(() => LevelID.Old == 51 && LevelID.Current == 52) },
                { "c51_fishing",            new FakeMemoryWatcher<bool>(() => LevelID.Old == 70 && LevelID.Current == 51) },

                // Chaos
                { "Chaos_Tails",            GetFlag(0x4FB1, 7) },
                { "Chaos_KnightFirst",      GetFlag(0x4FBA, 0) },
                { "Chaos_Hacking",          GetFlag(0xA49,  3) },
                { "Chaos_GreenCE",          GetFlag(0x4FB3, 6) },
                { "Chaos_CyanCE",           GetFlag(0x4FB3, 7) },
                { "Chaos_PinballStart",     GetFlag(0x4FB3, 3) },
                { "Chaos_PinballEnd",       GetFlag(0xA45,  2) },
                { "Chaos_BlueCE",           GetFlag(0x48C0, 7) },
                { "Chaos_RedCE",            GetFlag(0x48C1, 0) },
                { "Chaos_YellowCE",         GetFlag(0x48C1, 1) },
                { "Chaos_WhiteCE",          GetFlag(0x48C1, 2) },
                { "Chaos_KnightStart",      GetFlag(0xA85,  0) },
                { "Chaos_SuperSonic",       new FakeMemoryWatcher<bool>(() => LevelID.Current == 52 && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x0, 0x20, 0x0).DerefString(game, 255, " ") == "Battle1Top") },
                { "c52_story",              new FakeMemoryWatcher<bool>(() => LevelID.Old == 52 && LevelID.Current == 53) },
                { "c52_fishing",            new FakeMemoryWatcher<bool>(() => LevelID.Old == 70 && LevelID.Current == 52) },

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
            // In the update sequence, first of all we need to retrieve the new dynamic offsets
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            GetPointerAddresses();
            GetBossInstance();

            // Get the game flags for story mode
            Flags = new DeepPointer(addresses["baseAddress"], 0x28, 0x110, 0x40, 0x50).DerefBytes(game, 0x8100);
            foreach (var entry in SplitBools) entry.Value.Update();

            // Update the Fake Watchers
            // The order matters because some watchers depend on others
            IGT.Update();
            Status.Update();
            LevelID.Update();
            if (state.CurrentPhase == TimerPhase.NotRunning) IsInArcade = Status.Current == FakeEnums.Status.ArcadeMode || game.ReadValue<byte>(addresses["APPLICATIONSEQUENCE"] + 0x122).BitCheck(0);
            StoryModeCyberSpaceCompletionFlag.Update();
            IsInTutorial = FakeEnums.LevelID.TryGetValue(new DeepPointer(addresses["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], 0xF8).DerefString(game, 5, " "), out byte value) && value == 73;
            
            // I'm not happy I use 3 different variables to define the behaviour in the final QTE, but heh, it works
            IsInEndQTE = LevelID.Current == 72 && (IntPtr)new DeepPointer(addresses["baseAddress"], 0x70, 0xD0, 0x28, 0x0, 0x0).Deref<long>(game) == RTTI["EventQTEInput::evt::app"];
            QTEStatus.Update();
            EndQTECount.Update();

            // If the timer is not running (eg. a run has been reset) these variables need to be reset
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                if (AccumulatedIGT != TimeSpan.Zero) AccumulatedIGT = TimeSpan.Zero;
                if (AlreadyTriggeredBools.Count > 0) AlreadyTriggeredBools.Clear();
            }

            // When exiting a stage, or whenever the IGT resets, this will keep track of the time you accumulated so far
            if (IGT.Current == TimeSpan.Zero && IGT.Old != TimeSpan.Zero)
                AccumulatedIGT += IGT.Old;
        }

        /// <summary>
        /// This function is essentially equivalent of the init descriptor in script-based autosplitters.
        /// Everything you want to be executed when the game gets hooked needs to be put here.
        /// The main purpose of this function is to perform sigscanning and get memory addresses and offsets
        /// needed by the autosplitter.
        /// </summary>
        private void GetAddresses()
        {
            SignatureScanner scanner = new SignatureScanner(game, game.MainModuleWow64Safe().BaseAddress, game.MainModuleWow64Safe().ModuleMemorySize);

            // Base Address - For the Hedgehog Engine 2, it can be considered essentially the same as GWorld for Unreal Engine games.
            // Every important value needed by an autosplitter can essentially be grabbed from this address.
            addresses["baseAddress"] = scanner.ScanOrThrow(new SigScanTarget(1, "E8 ???????? 4C 8B 78 70") { OnFound = (p, s, addr) => { IntPtr tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3; return tempAddr + p.ReadValue<int>(tempAddr) + 0x4; } });

            // Game patch offset - This points to an instruction responsible for pausing the game if the user switches to another window.
            // This is used for the WFocus patch, which is essentially patching out a conditional jmp instruction.
            addresses["baseFocus"] = scanner.ScanOrThrow(new SigScanTarget("?? 36 48 8B 52 28"));

            // Offsets - I prefer defining them here because it makes it easier to change them, if the need arises.
            // So far they never changed so it should be fine to leave them as constant values.
            offsets["APPLICATION"]       = 0x80;
            offsets["GAMEMODE"]          = 0x78;
            offsets["GAMEMODEEXTENSION"] = 0xB0;

            // Defining a new instance of the RTTI class in order to get the vTable addresses of a couple of classes.
            // This makes it incredibly easy to calculate some dynamic offsets later,
            // mainly for GameModeHsmExtension (which contains the current status) and GameModeStageTimeExtension (which contains the IGT for Cyber Space levels)
            RTTI = new RTTI(scanner,
                "ApplicationSequenceExtension::game::app",
                "GameModeHsmExtension::game::app",
                "GameModeStageTimeExtension::game::app",
                "EventQTEInput::evt::app",
                "BossGiant::app",
                "BossDragon::app",
                "BossKnight::app"
                );
            RTTI.Add("BossExtension::userdefined", scanner.ScanOrThrow(new SigScanTarget(8, "E8 ???????? 48 8D 05 ???????? 31 FF 48 89 03 48 8D 8B ???????? 48 89 BB ???????? 48 8B 53 08") { OnFound = (p, s, addr) => addr + 0x4 + p.ReadValue<int>(addr) }));

            // Invoke whichever game fixes we want to apply
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
                    if ((IntPtr)game.ReadValue<long>(InstanceAddress) == RTTI["ApplicationSequenceExtension::game::app"])
                    {
                        addresses["APPLICATIONSEQUENCE"] = InstanceAddress;
                        break;
                    }
                }

                offsets["GameModeExtensionCount"] = new DeepPointer(addresses["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"] + 0x8).Deref<byte>(game);
                if (!GameModeLoad)
                {
                    IntPtr GameMode = (IntPtr)new DeepPointer(addresses["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], offsets["GAMEMODEEXTENSION"]).Deref<long>(game);
                    array = game.ReadBytes(GameMode, offsets["GameModeExtensionCount"] * 8);

                    for (int i = 0; i < offsets["GameModeExtensionCount"]; i++)
                    {
                        IntPtr InstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);
                        IntPtr temp = (IntPtr)game.ReadValue<long>(InstanceAddress);

                        if (temp == RTTI["GameModeHsmExtension::game::app"]) addresses["HsmExtension"] = InstanceAddress;
                        if (temp == RTTI["GameModeStageTimeExtension::game::app"]) addresses["StageTimeExtension"] = InstanceAddress;

                        if (!addresses["HsmExtension"].IsZero() && !addresses["StageTimeExtension"].IsZero())
                            break;
                    }
                }
            }
        }

        private void GetBossInstance()
        {
            addresses["CURRENTBOSS"] = IntPtr.Zero;

            byte count = new DeepPointer(addresses["baseAddress"], 0x70, 0x658 + 0x8).Deref<byte>(game);
            if (count != 0)
            {
                IntPtr BossApp = (IntPtr)new DeepPointer(addresses["baseAddress"], 0x70, 0x658).Deref<long>(game);
                byte[] array = game.ReadBytes(BossApp, count * 8);

                for (int i = 0; i < count; i++)
                {
                    IntPtr BossInstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);
                    IntPtr temp = (IntPtr)game.ReadValue<long>(BossInstanceAddress);

                    if (temp == RTTI["BossGiant::app"] || temp == RTTI["BossDragon::app"] || temp == RTTI["BossKnight::app"])
                    {
                        count = game.ReadValue<byte>(BossInstanceAddress + 0x130 + 0x8);
                        if (count != 0)
                        {
                            BossApp = (IntPtr)game.ReadValue<long>(BossInstanceAddress + 0x130);
                            array = game.ReadBytes(BossApp, count * 8);

                            for (int j = 0; j < count; j++)
                            {
                                BossInstanceAddress = (IntPtr)BitConverter.ToInt64(array, j * 8);
                                temp = (IntPtr)game.ReadValue<long>(BossInstanceAddress);
                                if (temp == RTTI["BossExtension::userdefined"])
                                {
                                    addresses["CURRENTBOSS"] = BossInstanceAddress;
                                    break;
                                }
                            }

                        }
                        break;
                    }
                }
            }
        }

        private FakeMemoryWatcher<bool> GetFlag(int offset, byte bitCheck)
        {
            return new FakeMemoryWatcher<bool>(() => Flags != null && Flags[offset].BitCheck(bitCheck));
        }

        enum QTEResolveStatus : byte
        {
            NotCompleted = 0,
            Completed = 1,
            Failed = 2
        }
    }
}