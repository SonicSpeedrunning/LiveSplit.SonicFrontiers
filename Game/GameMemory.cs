using System;
using System.Collections.Generic;
using System.Diagnostics;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using LiveSplit.Options;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        // Game stuff
        private GameVersion GameVersion { get; set; } = GameVersion.Unknown;

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
        public GameMode CurrentGameMode { get; protected set; } = GameMode.Story;
        public bool GameModeLoad => offsets["GameModeExtensionCount"] == 0;
        public bool IsInTutorial { get; protected set; } = default;

        // Story flags
        private StoryFlags Flags;
        public Dictionary<string, FakeMemoryWatcher<bool>> SplitBools { get; }
        public List<string> AlreadyTriggeredBools { get; } = new List<string>();

        // Boss Rush
        public FakeMemoryWatcher<BossRushAct> BossRushAct { get; }
        
        // Music Notes
        public FakeMemoryWatcher<byte[]> MusicNotes;
        public Watchers(LiveSplitState LSstate)
        {
            // Define LiveSplit's current state. We need this because we want to reset AccumulatedIGT and AlreadyTriggeredBools when a run is reset
            state = LSstate;

            // Define our FakeMemoryWatchers. I prefer using these instead of the standard MemoryWatchers because I can define a custom Update function
            IGT = new FakeMemoryWatcher<TimeSpan>(() =>
            {
                if (!addresses["StageTimeExtension"].IsZero())
                {
                    double igt = game.ReadValue<float>(addresses["StageTimeExtension"] + offsets["cyberstage_igt"]);
                    if (GameVersion != GameVersion.v1_01 && GameVersion != GameVersion.v1_10)
                    {
                        const double coef = .05 + (1 / 60d);
                        if (igt <= coef)
                            igt = 0d;
                        else
                            igt -= coef;
                    }
                    return TimeSpan.FromSeconds(Math.Truncate(igt * 100) / 100);
                }
                else if (!addresses["BattleRushExtension"].IsZero())
                {
                    double igt = game.ReadValue<float>(addresses["BattleRushExtension"] + 0x38);
                    return TimeSpan.FromSeconds(Math.Truncate(igt * 100) / 100);
                }
                else
                {
                    return TimeSpan.Zero;
                }
            });

            Status = new FakeMemoryWatcher<Status>(() =>
            {
                if (addresses["HsmExtension"].IsZero())
                    return SonicFrontiers.Status.Default;

                var _addr = (IntPtr)game.ReadValue<long>(addresses["HsmExtension"] + 0x60);
                
                if (_addr == IntPtr.Zero)
                    return SonicFrontiers.Status.Default;
                
                _addr = (IntPtr)game.ReadValue<long>(_addr + 0x20);

                if (_addr == IntPtr.Zero)
                    return SonicFrontiers.Status.Default;

                if (game.ReadString(_addr, 250, out var status))
                {
                    if (Enums.Status.TryGetValue(status, out var sstatus))
                        return sstatus;
                }

                return SonicFrontiers.Status.Default;
            });

            LevelID = new FakeMemoryWatcher<LevelID>(() =>
            {
                if (addresses["APPLICATIONSEQUENCE"].IsZero())
                    return SonicFrontiers.LevelID.None;

                if (game.ReadString(addresses["APPLICATIONSEQUENCE"] + 0xA0, 5, out var _string) && Enums.LevelID.TryGetValue(_string, out var levelid))
                {
                    if (levelid != SonicFrontiers.LevelID.MainMenu)
                        return levelid;
                    else
                    {
                        if (game.ReadString(addresses["GAMEMODE"] + 0x100, 5, out _string) && Enums.LevelID.TryGetValue(_string, out var levelid2))
                            return levelid2;
                        else
                            return levelid;
                    }
                }

                return LevelID.Current;
            });
            MusicNotes = new FakeMemoryWatcher<byte[]>(() =>
                {
                    if (Flags.NoteFlags == null)
                    {
                        Log.Warning("DEFAULT RETURNED");
                        return new byte[] { 0, 0, 0, 0, 0 };
                    }
                    return Flags.NoteFlags;
                }
            );
            StoryModeCyberSpaceCompletionFlag = new FakeMemoryWatcher<bool>(() => CurrentGameMode == GameMode.Story && LevelID.Current <= SonicFrontiers.LevelID.w4_9 && (Status.Current == SonicFrontiers.Status.Result || StoryModeCyberSpaceCompletionFlag.Old));

            QTEStatus = new FakeMemoryWatcher<QTEResolveStatus>(() =>
            {
                if (!IsInEndQTE || !game.ReadValue<QTEResolveStatus>(addresses["QTE"] + 0x254, out var status))
                    return QTEResolveStatus.NotCompleted;
                else
                    return status;
            });

            EndQTECount = new FakeMemoryWatcher<byte>(() =>
            {
                if (LevelID.Current != SonicFrontiers.LevelID.Boss_TheEnd)
                    return 0;
                if (QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Failed)
                    return 0;
                if (EndQTECount.Current == 3 && !IsInEndQTE)
                    return 0;
                if (EndQTECount.Current > 3)
                    return 0;

                if (IsInEndQTE && QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Completed)
                    return (byte)(EndQTECount.Current + 1);
                else
                    return EndQTECount.Current;
            });

            // Split bools
            SplitBools = new Dictionary<string, FakeMemoryWatcher<bool>>
            {
                //Skills (Manual Unlocks)
                {"Skill_Cyloop",            new FakeMemoryWatcher<bool>(() => Flags.Skill_Cyloop) },
                {"Skill_PhantomRush",       new FakeMemoryWatcher<bool>(() => Flags.Skill_PhantomRush) },
                {"Skill_AirTrick",          new FakeMemoryWatcher<bool>(() => Flags.Skill_AirTrick) },
                {"Skill_StompAttack",       new FakeMemoryWatcher<bool>(() => Flags.Skill_StompAttack) },
                {"Skill_QuickCyloop",       new FakeMemoryWatcher<bool>(() => Flags.Skill_QuickCyloop) },
                {"Skill_SonicBoom",         new FakeMemoryWatcher<bool>(() => Flags.Skill_SonicBoom) },
                {"Skill_WildRush",          new FakeMemoryWatcher<bool>(() => Flags.Skill_WildRush) },
                {"Skill_LoopKick",          new FakeMemoryWatcher<bool>(() => Flags.Skill_LoopKick) },
                {"Skill_HomingShot",        new FakeMemoryWatcher<bool>(() => Flags.Skill_HomingShot) },
                {"Skill_AutoCombo",         new FakeMemoryWatcher<bool>(() => Flags.Skill_AutoCombo) },
                {"Skill_SpinSlash",         new FakeMemoryWatcher<bool>(() => Flags.Skill_SpinSlash) },
                {"Skill_RecoverySmash",     new FakeMemoryWatcher<bool>(() => Flags.Skill_RecoverySmash) },

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
                { "Kronos_GigantosStart",   new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Kronos && !addresses["CURRENTBOSSSTATUS"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS"], 255, out var value) && value == "WalkingBase") },
                { "Kronos_SuperSonic",      new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Kronos && !addresses["CURRENTBOSSSTATUS"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS"], 255, out var value) && value == "BattlePhaseParent") },
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
                { "Ares_WyvernStart",       new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSSSTATUS_WYVERN"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS_WYVERN"], 255, out var value) && value == "PatrolTop") },
                { "Ares_WyvernRun",         new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSSSTATUS_WYVERN"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS_WYVERN"], 255, out var value) && value == "EventRise") },
                { "Ares_SuperSonic",        new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSSSTATUS_WYVERN"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS_WYVERN"], 255, out var value) && value == "BattleTop") },
                { "Island_Ares_story",      new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Island_Ares && LevelID.Current == SonicFrontiers.LevelID.Island_Chaos) },
                { "Island_Ares_fishing",    new FakeMemoryWatcher<bool>(() => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Ares) },

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
                { "Chaos_SuperSonic",       new FakeMemoryWatcher<bool>(() => LevelID.Current == SonicFrontiers.LevelID.Island_Chaos && !addresses["CURRENTBOSSSTATUS"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS"], 255, out var value) && value == "Battle1Top") },
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
            
            BossRushAct = new FakeMemoryWatcher<BossRushAct>(() =>
            {
                var levelid = LevelID.Current;
                if (levelid != SonicFrontiers.LevelID.Island_Kronos_BossRush && levelid != SonicFrontiers.LevelID.Island_Ares_BossRush && levelid != SonicFrontiers.LevelID.Island_Chaos_BossRush && levelid != SonicFrontiers.LevelID.Island_Ouranos_BossRush)
                    return SonicFrontiers.BossRushAct.None;

                if (addresses["BattleRushExtension"].IsZero() || !game.ReadValue<byte>(addresses["BattleRushExtension"] + 0x2C, out var phase))
                    return SonicFrontiers.BossRushAct.None;

                return levelid switch
                {
                    SonicFrontiers.LevelID.Island_Kronos_BossRush => phase switch
                    {
                        0 => SonicFrontiers.BossRushAct.b1_1,
                        1 => SonicFrontiers.BossRushAct.b1_2,
                        2 => SonicFrontiers.BossRushAct.b1_3,
                        3 => SonicFrontiers.BossRushAct.b1_4,
                        4 => SonicFrontiers.BossRushAct.b1_5,
                        5 => SonicFrontiers.BossRushAct.b1_6,
                        6 => SonicFrontiers.BossRushAct.b1_7,
                        7 => SonicFrontiers.BossRushAct.b1_8,
                        8 => SonicFrontiers.BossRushAct.b1_9,
                        9 => SonicFrontiers.BossRushAct.b1_10,
                        _ => SonicFrontiers.BossRushAct.None,
                    },
                    SonicFrontiers.LevelID.Island_Ares_BossRush => phase switch
                    {
                        0 => SonicFrontiers.BossRushAct.b2_1,
                        1 => SonicFrontiers.BossRushAct.b2_2,
                        2 => SonicFrontiers.BossRushAct.b2_3,
                        3 => SonicFrontiers.BossRushAct.b2_4,
                        4 => SonicFrontiers.BossRushAct.b2_5,
                        5 => SonicFrontiers.BossRushAct.b2_6,
                        6 => SonicFrontiers.BossRushAct.b2_7,
                        7 => SonicFrontiers.BossRushAct.b2_8,
                        8 => SonicFrontiers.BossRushAct.b2_9,
                        9 => SonicFrontiers.BossRushAct.b2_10,
                        _ => SonicFrontiers.BossRushAct.None,
                    },
                    SonicFrontiers.LevelID.Island_Chaos_BossRush => phase switch
                    {
                        0 => SonicFrontiers.BossRushAct.b3_1,
                        1 => SonicFrontiers.BossRushAct.b3_2,
                        2 => SonicFrontiers.BossRushAct.b3_3,
                        3 => SonicFrontiers.BossRushAct.b3_4,
                        4 => SonicFrontiers.BossRushAct.b3_5,
                        5 => SonicFrontiers.BossRushAct.b3_6,
                        6 => SonicFrontiers.BossRushAct.b3_7,
                        7 => SonicFrontiers.BossRushAct.b3_8,
                        8 => SonicFrontiers.BossRushAct.b3_9,
                        _ => SonicFrontiers.BossRushAct.None,
                    },
                    SonicFrontiers.LevelID.Island_Ouranos_BossRush => phase switch
                    {
                        0 => SonicFrontiers.BossRushAct.b4_1,
                        1 => SonicFrontiers.BossRushAct.b4_2,
                        2 => SonicFrontiers.BossRushAct.b4_3,
                        3 => SonicFrontiers.BossRushAct.b4_4,
                        4 => SonicFrontiers.BossRushAct.b4_5,
                        5 => SonicFrontiers.BossRushAct.b4_6,
                        6 => SonicFrontiers.BossRushAct.b4_7,
                        7 => SonicFrontiers.BossRushAct.b4_8,
                        8 => SonicFrontiers.BossRushAct.b4_9,
                        9 => SonicFrontiers.BossRushAct.b4_10,
                        10 => SonicFrontiers.BossRushAct.b4_11,
                        _ => SonicFrontiers.BossRushAct.None,
                    },
                    _ => SonicFrontiers.BossRushAct.None,
                };
            });

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
            BossRushAct.Update();

            // Get the game flags for story mode
            //Flags = new DeepPointer(addresses["baseAddress"], 0x28, 0x110, 0x40, 0x50).Deref<StoryFlags>(game);
            
            Flags = new DeepPointer(addresses["baseAddress"], 0x28, 0x110, 0x40, 0x0).Deref<StoryFlags>(game);
            Flags.ValidateRTTI(RTTI["UserElement::save::app"]);
            foreach (var entry in SplitBools)
                entry.Value.Update();
            
            MusicNotes.Update();
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                var gamemodeflag = game.ReadValue<byte>(addresses["APPLICATIONSEQUENCE"] + 0x122);
                var gamemodeflag2 = game.ReadValue<byte>(addresses["APPLICATIONSEQUENCE"] + 0x123);
                if (Status.Current == SonicFrontiers.Status.ArcadeMode || gamemodeflag.BitCheck(0))
                    CurrentGameMode = GameMode.Arcade;
                else if (Status.Current == SonicFrontiers.Status.CyberMode || gamemodeflag.BitCheck(7) || (gamemodeflag2 & 0b1111) != 0) //kronos,ares,chaos,ouranos,all = bits 7, (next byte) 0,1,2,3
                    CurrentGameMode = GameMode.CyberspaceChallenge;
                else if (Status.Current == SonicFrontiers.Status.BattleMode || LevelID.Current == SonicFrontiers.LevelID.Island_Kronos_BossRush || LevelID.Current == SonicFrontiers.LevelID.Island_Ares_BossRush || LevelID.Current == SonicFrontiers.LevelID.Island_Chaos_BossRush || LevelID.Current == SonicFrontiers.LevelID.Island_Ouranos_BossRush)
                    CurrentGameMode = GameMode.BossRush;
                else
                    CurrentGameMode = GameMode.Story;
            }

            StoryModeCyberSpaceCompletionFlag.Update();
            IsInTutorial = Enums.LevelID.TryGetValue(game.ReadString(addresses["GAMEMODE"] + 0xF8, 5, out var _string) ? _string : " ", out var value) && value == SonicFrontiers.LevelID.Tutorial;
            
            // I'm not happy I use 3 different variables to define the behaviour in the final QTE, but heh, it works
            IsInEndQTE = LevelID.Current == SonicFrontiers.LevelID.Boss_TheEnd && !addresses["QTE"].IsZero() && (IntPtr)game.ReadValue<long>(addresses["QTE"]) == RTTI["EventQTEInput::evt::app"];
            QTEStatus.Update();
            EndQTECount.Update();

            // If the timer is not running (eg. a run has been reset) these variables need to be reset
            if (state.CurrentPhase == TimerPhase.NotRunning)
            {
                if (AccumulatedIGT != TimeSpan.Zero)
                    AccumulatedIGT = TimeSpan.Zero;

                if (AlreadyTriggeredBools.Count > 0)
                    AlreadyTriggeredBools.Clear();
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
            GameVersion = game.MainModuleWow64Safe().ModuleMemorySize switch
            {
                0x162C8000 => GameVersion.v1_01,
                0x1661B000 => GameVersion.v1_10,
                0x1622F000 => GameVersion.v1_20, // Speed update (March 23rd, 2023)
                0x16418000 => GameVersion.v1_30, // Sonic's birthday update (June 24th, 2023)
                _ => GameVersion.Unknown,
            };

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

            // These offsets are known to change so we will dynamically find them through specific sigscanning
            offsets["cyberstage_igt"] = game.ReadValue<byte>(scanner.ScanOrThrow(new SigScanTarget(4, "F3 0F 11 49 ?? F3 0F 5C 0D")));

            // Defining a new instance of the RTTI class in order to get the vTable addresses of a couple of classes.
            // This makes it incredibly easy to calculate some dynamic offsets later,
            var RTTILIST = new List<string>
            {
                "ApplicationSequenceExtension::game::app",
                "GameModeHsmExtension::game::app",
                "GameModeStageTimeExtension::game::app",
                "UserElement::save::app",
                "EventQTEInput::evt::app",
                "BossGiant::app",
                "BossDragon::app",
                "BossKnight::app"
            };

            // Old game versions do not have Battle Rush
            if (GameVersion != GameVersion.v1_01 && GameVersion != GameVersion.v1_10)
                RTTILIST.Add("GameModeBattleRushExtension::game::app");

            RTTI = new RTTI(scanner, RTTILIST.ToArray());

            RTTI.Add("BossExtension::userdefined", scanner.ScanOrThrow(new SigScanTarget(8, "E8 ???????? 48 8D 05 ???????? 31 FF 48 89 03 48 8D 8B ???????? 48 89 BB ???????? 48 8B 53 08") { OnFound = (p, s, addr) => addr + 0x4 + p.ReadValue<int>(addr) }));

            // Invoke whichever game fixes we want to apply
            WFocusChange?.Invoke(this, EventArgs.Empty);
        }

        private void GetPointerAddresses()
        {
            addresses["APPLICATION"] = IntPtr.Zero;
            addresses["APPLICATIONSEQUENCE"] = IntPtr.Zero;
            addresses["GAMEMODE"] = IntPtr.Zero;
            addresses["GAMEMODEEXTENSION"] = IntPtr.Zero;
            addresses["QTE"] = IntPtr.Zero;
            addresses["MUSICNOTES"] = IntPtr.Zero;
            addresses["HsmExtension"] = IntPtr.Zero;
            addresses["StageTimeExtension"] = IntPtr.Zero;
            addresses["BattleRushExtension"] = IntPtr.Zero;
            offsets["GameModeExtensionCount"] = 0;

            var _base = (IntPtr)game.ReadValue<long>(addresses["baseAddress"]);
            if (!_base.IsZero())
            {
                addresses["APPLICATION"] = (IntPtr)game.ReadValue<long>(_base + offsets["APPLICATION"]);
                if (!addresses["APPLICATION"].IsZero())
                {
                    byte ApplicationSequenceCount = game.ReadValue<byte>(_base + offsets["APPLICATION"] + 0x8);
                    if (ApplicationSequenceCount != 0)
                    {
                        byte[] array = game.ReadBytes(addresses["APPLICATION"], ApplicationSequenceCount * 8);
                        for (int i = 0; i < ApplicationSequenceCount; i++)
                        {
                            IntPtr InstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);
                            if ((IntPtr)game.ReadValue<long>(InstanceAddress) == RTTI["ApplicationSequenceExtension::game::app"])
                            {
                                addresses["APPLICATIONSEQUENCE"] = InstanceAddress;
                                break;
                            }
                        }

                        if (!addresses["APPLICATIONSEQUENCE"].IsZero())
                        {
                            addresses["GAMEMODE"] = (IntPtr)game.ReadValue<long>(addresses["APPLICATIONSEQUENCE"] + offsets["GAMEMODE"]);
                            addresses["GAMEMODEEXTENSION"] = (IntPtr)game.ReadValue<long>(addresses["GAMEMODE"] + offsets["GAMEMODEEXTENSION"]);

                            if (!addresses["GAMEMODEEXTENSION"].IsZero())
                            {
                                offsets["GameModeExtensionCount"] = game.ReadValue<byte>(addresses["GAMEMODE"] + +offsets["GAMEMODEEXTENSION"] + 0x8);
                                if (offsets["GameModeExtensionCount"] != 0)
                                {
                                    array = game.ReadBytes(addresses["GAMEMODEEXTENSION"], offsets["GameModeExtensionCount"] * 8);

                                    for (int i = 0; i < offsets["GameModeExtensionCount"]; i++)
                                    {
                                        IntPtr InstanceAddress = (IntPtr)BitConverter.ToInt64(array, i * 8);
                                        IntPtr temp = (IntPtr)game.ReadValue<long>(InstanceAddress);

                                        if (temp == RTTI["GameModeHsmExtension::game::app"])
                                            addresses["HsmExtension"] = InstanceAddress;
                                        else if (temp == RTTI["GameModeStageTimeExtension::game::app"])
                                            addresses["StageTimeExtension"] = InstanceAddress;
                                        else if (GameVersion != GameVersion.v1_01 && GameVersion != GameVersion.v1_10 && temp == RTTI["GameModeBattleRushExtension::game::app"])
                                            addresses["BattleRushExtension"] = InstanceAddress;

                                        if (!addresses["HsmExtension"].IsZero() &&
                                            !addresses["StageTimeExtension"].IsZero() &&
                                            (GameVersion != GameVersion.v1_01 && GameVersion != GameVersion.v1_10 && !addresses["BattleRushExtension"].IsZero()) )
                                            break;
                                    }
                                }
                            }
                        }                                                                                   
                    }
                }

                IntPtr _addr = (IntPtr)game.ReadValue<long>(_base + 0x70);
                if (!_addr.IsZero())
                {
                    _addr = (IntPtr)game.ReadValue<long>(_addr + 0xD0);
                    if (!_addr.IsZero())
                    {
                        _addr = (IntPtr)game.ReadValue<long>(_addr + 0x28);
                        if (!_addr.IsZero())
                        {
                            addresses["QTE"] = (IntPtr)game.ReadValue<long>(_addr);
                        }
                    }
                }
            }
        }

        private void GetBossInstance()
        {
            addresses["CURRENTBOSS"] = IntPtr.Zero;
            addresses["CURRENTBOSSSTATUS"] = IntPtr.Zero;
            addresses["CURRENTBOSSSTATUS_WYVERN"] = IntPtr.Zero;

            IntPtr _temp = (IntPtr)game.ReadValue<long>(addresses["baseAddress"]);
            _temp = (IntPtr)game.ReadValue<long>(_temp + 0x70);

            byte count = game.ReadValue<byte>(_temp + 0x658 + 0x8);
            if (count != 0)
            {
                IntPtr BossApp = (IntPtr)game.ReadValue<long>(_temp + 0x658);
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

            if (!addresses["CURRENTBOSS"].IsZero())
            {
                var _addr = (IntPtr)game.ReadValue<long>(addresses["CURRENTBOSS"] + 0xA8);
                if (!_addr.IsZero())
                {
                    IntPtr _addr2 = (IntPtr)game.ReadValue<long>(_addr + 0x0);
                    if (!_addr.IsZero())
                        addresses["CURRENTBOSSSTATUS"] = (IntPtr)game.ReadValue<long>(_addr2 + 0x20);

                    _addr2 = (IntPtr)game.ReadValue<long>(_addr + 0x8);
                    if (!_addr.IsZero())
                    {
                        addresses["CURRENTBOSSSTATUS_WYVERN"] = (IntPtr)game.ReadValue<long>(_addr2 + 0x20);
                    }
                }
            }
        }
    }
}