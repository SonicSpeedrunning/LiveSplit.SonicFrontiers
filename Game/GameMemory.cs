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

        // IGT
        public FakeMemoryWatcher<TimeSpan> IGT { get; }
        public TimeSpan AccumulatedIGT { get; protected set; } = default;

        // Level ID and status
        public FakeMemoryWatcher<Status> Status { get; }
        public FakeMemoryWatcher<LevelID> LevelID { get; }

        // QTE stuff for the final boss
        public FakeMemoryWatcher<byte> EndQTECount { get; }
        private readonly FakeMemoryWatcher<QTEResolveStatus> QTEStatus;
        private bool IsInEndQTE = false;

        // Stage completion flags
        public FakeMemoryWatcher<bool> StoryModeCyberSpaceCompletionFlag { get; }
        public bool IsInTutorial { get; protected set; } = default;
        public bool IsInArcade { get; protected set; } = default;
        public bool GameModeLoad => offsets["GameModeExtensionCount"] == 0;

        // Story flags
        private StoryFlags Flags;
        public Dictionary<string, FakeMemoryWatcher<bool>> SplitBools { get; }
        public List<string> AlreadyTriggeredBools { get; } = new List<string>();

        public Watchers(LiveSplitState LSstate)
        {
            // Define LiveSplit's current state. We need this because we want to reset AccumulatedIGT and AlreadyTriggeredBools when a run is reset
            state = LSstate;

            // Define our FakeMemoryWatchers. I prefer using these instead of the standard MemoryWatchers because I can define a custom Update function
            IGT = new FakeMemoryWatcher<TimeSpan>(() => addresses["StageTimeExtension"].IsZero() ? default : TimeSpan.FromSeconds(Math.Truncate(game.ReadValue<float>(addresses["StageTimeExtension"] + 0x28) * 100) / 100));
            Status = new FakeMemoryWatcher<Status>(() => addresses["HsmExtension"].IsZero() ? SonicFrontiers.Status.Default : Enums.Status.TryGetValue(new DeepPointer(addresses["HsmExtension"] + 0x60, 0x20, 0x0).DerefString(game, 250, " "), out var status) ? status : SonicFrontiers.Status.Default);
            LevelID = new FakeMemoryWatcher<LevelID>(() => addresses["APPLICATIONSEQUENCE"].IsZero() ? SonicFrontiers.LevelID.None : Enums.LevelID.TryGetValue(game.ReadString(addresses["APPLICATIONSEQUENCE"] + 0xA0, 5) ?? " ", out var value) ? value : LevelID.Current);
            StoryModeCyberSpaceCompletionFlag = new FakeMemoryWatcher<bool>(() => !IsInArcade && LevelID.Current <= SonicFrontiers.LevelID.w4_9 && (Status.Current == SonicFrontiers.Status.Result || StoryModeCyberSpaceCompletionFlag.Old));
            QTEStatus = new FakeMemoryWatcher<QTEResolveStatus>(() => !IsInEndQTE ? QTEResolveStatus.NotCompleted : (QTEResolveStatus) new DeepPointer(addresses["baseAddress"], 0x70, 0xD0, 0x28, 0x0, 0x254).Deref<byte>(game));
            EndQTECount = new FakeMemoryWatcher<byte>(() => LevelID.Current != SonicFrontiers.LevelID.Boss_TheEnd || (QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Failed) || (EndQTECount.Current == 3 && !IsInEndQTE) || EndQTECount.Current > 3 ? default : IsInEndQTE && QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Completed ? (byte)(EndQTECount.Current + 1) : EndQTECount.Current);

            // Split bools
            SplitBools = new Dictionary<string, FakeMemoryWatcher<bool>>
            {
                //Skills (Manual Unlocks)
                {"Skill_Cyloop", new FakeMemoryWatcher<bool>(()=>Flags.Skill_Cyloop)},
                {"Skill_PhantomRush", new FakeMemoryWatcher<bool>(() => Flags.Skill_PhantomRush)},
                {"Skill_AirTrick", new FakeMemoryWatcher<bool>(() => Flags.Skill_AirTrick)},
                {"Skill_StompAttack", new FakeMemoryWatcher<bool>(() => Flags.Skill_StompAttack)},
                {"Skill_QuickCyloop", new FakeMemoryWatcher<bool>(() => Flags.Skill_QuickCyloop)},
                {"Skill_SonicBoom", new FakeMemoryWatcher<bool>(() => Flags.Skill_SonicBoom)},
                {"Skill_WildRush", new FakeMemoryWatcher<bool>(() => Flags.Skill_WildRush)},
                {"Skill_LoopKick", new FakeMemoryWatcher<bool>(()=>Flags.Skill_LoopKick)},
                {"Skill_HomingShot", new FakeMemoryWatcher<bool>(() => Flags.Skill_HomingShot)},
                {"Skill_AutoCombo", new FakeMemoryWatcher<bool>(() => Flags.Skill_AutoCombo)},
                {"Skill_SpinSlash", new FakeMemoryWatcher<bool>(() => Flags.Skill_SpinSlash)},
                {"Skill_RecoverySmash", new FakeMemoryWatcher<bool>(() => Flags.Skill_RecoverySmash)},
                // Kronos
                { "Kronos_Ninja",           new FakeMemoryWatcher<bool>(() => Flags.Kronos_Ninja) },
                { "Kronos_Door",            new FakeMemoryWatcher<bool>(() => Flags.Kronos_Door) },
                { "Kronos_Amy",             new FakeMemoryWatcher<bool>(() => Flags.Kronos_Amy) },
                { "Kronos_GigantosFirst",   new FakeMemoryWatcher<bool>(() => Flags.Kronos_GigantosFirst) },
                { "Kronos_GreenCE",         new FakeMemoryWatcher<bool>(() => Flags.Kronos_GreenCE) },
                { "Kronos_CyanCE",          new FakeMemoryWatcher<bool>(() => Flags.Kronos_CyanCE) },
                { "Kronos_Tombstones",      new FakeMemoryWatcher<bool>(() => Flags.Kronos_Tombstones) },
                { "Kronos_BlueCE",          new FakeMemoryWatcher<bool>(() => Flags.Kronos_BlueCE) },
                { "Kronos_RedCE",           new FakeMemoryWatcher<bool>(() => Flags.Kronos_RedCE) },
                { "Kronos_YellowCE",        new FakeMemoryWatcher<bool>(() => Flags.Kronos_YellowCE) },
                { "Kronos_WhiteCE",         new FakeMemoryWatcher<bool>(() => Flags.Kronos_WhiteCE) },
                { "Kronos_GigantosStart",   new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Kronos && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x0, 0x20, 0x0).DerefString(game, 255, " ") == "WalkingBase") },
                { "Kronos_SuperSonic",      new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Kronos && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x0, 0x20, 0x0).DerefString(game, 255, " ") == "BattlePhaseParent") },
                { "Island_Kronos_story",    new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Island_Kronos && LevelID.Current == SonicFrontiers.LevelID.Island_Ares) },
                { "Island_Kronos_fishing",  new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Kronos) },

                // Ares
                { "Ares_Knuckles",          new FakeMemoryWatcher<bool>(() => Flags.Ares_Knuckles) },
                { "Ares_WyvernFirst",       new FakeMemoryWatcher<bool>(() => Flags.Ares_WyvernFirst) },
                { "Ares_Water",             new FakeMemoryWatcher<bool>(() => Flags.Ares_Water) },
                // { "Ares_KocoRoundup",    new FakeMemoryWatcher<bool>(() => Flags.Ares_KocoRoundup },
                { "Ares_Crane",             new FakeMemoryWatcher<bool>(() => Flags.Ares_Crane) },
                { "Ares_GreenCE",           new FakeMemoryWatcher<bool>(() => Flags.Ares_GreenCE) },
                { "Ares_CyanCE",            new FakeMemoryWatcher<bool>(() => Flags.Ares_CyanCE) },
                { "Ares_BlueCE",            new FakeMemoryWatcher<bool>(() => Flags.Ares_BlueCE) },
                { "Ares_RedCE",             new FakeMemoryWatcher<bool>(() => Flags.Ares_RedCE) },
                { "Ares_YellowCE",          new FakeMemoryWatcher<bool>(() => Flags.Ares_YellowCE) },
                { "Ares_WhiteCE",           new FakeMemoryWatcher<bool>(() => Flags.Ares_WhiteCE) },
                { "Ares_WyvernStart",       new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x8, 0x20, 0x0).DerefString(game, 255, " ") == "PatrolTop") },
                { "Ares_WyvernRun",         new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x8, 0x20, 0x0).DerefString(game, 255, " ") == "EventRise") },
                { "Ares_SuperSonic",        new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x8, 0x20, 0x0).DerefString(game, 255, " ") == "BattleTop") },
                { "Island_Ares_story",      new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Island_Ares && LevelID.Current == SonicFrontiers.LevelID.Island_Chaos) },
                { "Island_Ares_fishing",    new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Chaos) },

                // Chaos
                { "Chaos_Tails",            new FakeMemoryWatcher<bool>(() => Flags.Chaos_Tails) },
                { "Chaos_KnightFirst",      new FakeMemoryWatcher<bool>(() => Flags.Chaos_KnightFirst) },
                { "Chaos_Hacking",          new FakeMemoryWatcher<bool>(() => Flags.Chaos_Hacking) },
                { "Chaos_GreenCE",          new FakeMemoryWatcher<bool>(() => Flags.Chaos_GreenCE) },
                { "Chaos_CyanCE",           new FakeMemoryWatcher<bool>(() => Flags.Chaos_CyanCE) },
                { "Chaos_PinballStart",     new FakeMemoryWatcher<bool>(() => Flags.Chaos_PinballStart) },
                { "Chaos_PinballEnd",       new FakeMemoryWatcher<bool>(() => Flags.Chaos_PinballEnd)},
                { "Chaos_BlueCE",           new FakeMemoryWatcher<bool>(() => Flags.Chaos_BlueCE) },
                { "Chaos_RedCE",            new FakeMemoryWatcher<bool>(() => Flags.Chaos_RedCE) },
                { "Chaos_YellowCE",         new FakeMemoryWatcher<bool>(() => Flags.Chaos_YellowCE) },
                { "Chaos_WhiteCE",          new FakeMemoryWatcher<bool>(() => Flags.Chaos_WhiteCE) },
                { "Chaos_KnightStart",      new FakeMemoryWatcher<bool>(() => Flags.Chaos_KnightStart) },
                { "Chaos_SuperSonic",       new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Chaos && !addresses["CURRENTBOSS"].IsZero() && new DeepPointer(addresses["CURRENTBOSS"] + 0xA8, 0x0, 0x20, 0x0).DerefString(game, 255, " ") == "Battle1Top") },
                { "Island_Chaos_story",     new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Island_Chaos && LevelID.Current == SonicFrontiers.LevelID.Island_Rhea) },
                { "Island_Chaos_fishing",   new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Chaos) },

                // Rhea
                { "Rhea_Tower1",            new FakeMemoryWatcher<bool>(() => Flags.Rhea_Tower1) },
                { "Rhea_Tower2",            new FakeMemoryWatcher<bool>(() => Flags.Rhea_Tower2) },
                { "Rhea_Tower3",            new FakeMemoryWatcher<bool>(() => Flags.Rhea_Tower3) },
                { "Rhea_Tower4",            new FakeMemoryWatcher<bool>(() => Flags.Rhea_Tower4) },
                { "Rhea_Tower5",            new FakeMemoryWatcher<bool>(() => Flags.Rhea_Tower5) },
                { "Rhea_Tower6",            new FakeMemoryWatcher<bool>(() => Flags.Rhea_Tower6) },
                { "Island_Rhea_story",      new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Island_Rhea && LevelID.Current == SonicFrontiers.LevelID.Island_Ouranos) },

                // Ouranos
                { "Ouranos_Bridge",         new FakeMemoryWatcher<bool>(() => Flags.Ouranos_Bridge) },
                { "Ouranos_SupremeDefeated",new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Island_Ouranos && LevelID.Current == SonicFrontiers.LevelID.Boss_TheEnd) },
                { "Ouranos_BlueCE",         new FakeMemoryWatcher<bool>(() => Flags.Ouranos_BlueCE) },
                { "Ouranos_RedCE",          new FakeMemoryWatcher<bool>(() => Flags.Ouranos_RedCE) },
                { "Ouranos_GreenCE",        new FakeMemoryWatcher<bool>(() => Flags.Ouranos_GreenCE) },
                { "Ouranos_YellowCE",       new FakeMemoryWatcher<bool>(() => Flags.Ouranos_YellowCE) },
                { "Ouranos_CyanCE",         new FakeMemoryWatcher<bool>(() => Flags.Ouranos_CyanCE) },
                { "Ouranos_WhiteCE",        new FakeMemoryWatcher<bool>(() => Flags.Ouranos_WhiteCE) },
                { "Island_Ouranos_fishing", new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Ouranos) }
            };

            GameProcess = new ProcessHook("SonicFrontiers");
        }

        public void Update()
        {
            // In the update sequence, first of all we need to retrieve the new dynamic offsets
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            GetPointerAddresses();
            GetBossInstance();

            // Update the Fake Watchers
            // The order matters because some watchers depend on others
            IGT.Update();
            Status.Update();
            LevelID.Update();

            // Get the game flags for story mode
            //Flags = new DeepPointer(addresses["baseAddress"], 0x28, 0x110, 0x40, 0x50).Deref<StoryFlags>(game);
            Flags = new DeepPointer(addresses["baseAddress"], 0x28, 0x110, 0x40, 0x0).Deref<StoryFlags>(game);
            Flags.ValidateRTTI(RTTI["UserElement::save::app"]);
            foreach (var entry in SplitBools)
                entry.Value.Update();

            if (state.CurrentPhase == TimerPhase.NotRunning)
                IsInArcade = Status.Current == SonicFrontiers.Status.ArcadeMode || game.ReadValue<byte>(addresses["APPLICATIONSEQUENCE"] + 0x122).BitCheck(0);

            StoryModeCyberSpaceCompletionFlag.Update();
            IsInTutorial = Enums.LevelID.TryGetValue(new DeepPointer(addresses["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"], 0xF8).DerefString(game, 5, " "), out var value) && value == SonicFrontiers.LevelID.Tutorial;
            
            // I'm not happy I use 3 different variables to define the behaviour in the final QTE, but heh, it works
            IsInEndQTE = LevelID.Current == SonicFrontiers.LevelID.Boss_TheEnd && (IntPtr)new DeepPointer(addresses["baseAddress"], 0x70, 0xD0, 0x28, 0x0, 0x0).Deref<long>(game) == RTTI["EventQTEInput::evt::app"];
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
                "UserElement::save::app",
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
    }
}