using JHelper.Common.Collections;
using JHelper.Common.MemoryUtils;
using JHelper.Common.ProcessInterop;
using JHelper.Common.ProcessInterop.API;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.SonicFrontiers;
using LiveSplit.SonicFrontiers.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LiveSplit.SonicFrontiers;

partial class Memory
{
    /// <summary>
    /// Simple counter used for lazily update the watchers
    /// </summary>
    protected MemStateTracker StateTracker { get; } = new();

    /// <summary>
    /// The version of the game.
    /// </summary>
    public GameVersion GameVersion { get; }

    /// <summary>
    /// The Hedgehog engine instance used for interacting with the game engine.
    /// </summary>
    private HedgehogEngine2 Engine { get; }

    /// <summary>
    /// Watches the status of the HSM (Hierarchical State Machine).
    /// </summary>
    public LazyWatcher<string[]> HsmStatus { get; }

    public LazyWatcher<TimeSpan> IGT { get; }
    public TimeSpan AccumulatedIGT { get; protected set; } = default;

    /// <summary>
    /// Watches the level ID in the game.
    /// </summary>
    public LazyWatcher<LevelID> LevelID { get; }

    // Final boss stuff
    public LazyWatcher<byte> EndQTECount { get; }
    private readonly LazyWatcher<QTEResolveStatus> QTEStatus;
    private bool IsInEndQTE = false;
    
    // Another end boss stuff
    public LazyWatcher<byte> AnotherQTECount { get; }
    private bool IsInAnotherBoss = false;
    private bool IsFightingRifleBeast = false;
    

    // Stage completion flags
    public LazyWatcher<bool> StoryModeCyberSpaceCompletionFlag { get; }


    /// <summary>
    /// Watches the current game mode
    /// </summary>
    public LazyWatcher<GameMode> GameMode { get; }


    private bool IsInTutorial = false;

    
    // Story flags
    private StoryFlags Flags;
    public Dictionary<string, LazyWatcher<bool>> SplitBools { get; }
    public List<string> AlreadyTriggeredBools { get; } = new List<string>();

    // Boss Rush
    public LazyWatcher<BossRushAct> BossRushAct { get; }
    
    // Music Notes
    public LazyWatcher<byte[]> MusicNotes;

    /// <summary>
    /// Constructor initializing game engine, version, and various game state watchers.
    /// </summary>
    /// <param name="process">The current game process.</param>
    public Memory(ProcessMemory process, LiveSplitState liveSplitState)
    {
        Engine = new HedgehogEngine2(process);
        StateTracker.OnTick = () => Engine.Update(process);

        // Determine the game version based on module memory size.
        // Mostly unused in the autosplitter, might be needed in the future.
        GameVersion = process.MainModule.ModuleMemorySize switch
        {
            0x162C8000 => GameVersion.v1_01,
            0x1661B000 => GameVersion.v1_10,
            0x1622F000 => GameVersion.v1_20,
            0x16418000 => GameVersion.v1_30,
            0x16204000 => GameVersion.v1_42,
            _ => GameVersion.Unknown,
        };

        // Initialize LazyWatchers for observing game data
        HsmStatus = new LazyWatcher<string[]>(StateTracker, [string.Empty, string.Empty, string.Empty, string.Empty], [string.Empty, string.Empty, string.Empty, string.Empty], (current, old) =>
        {
            int no_of_details;
            IntPtr addr;

            using (ArrayRental<byte> buf = new(stackalloc byte[0x10]))
            {
                // Check for valid game mode and read status from memory
                if (!Engine.GetExtension("GameModeHsmExtension", out IntPtr instance)
                    || !process.ReadArray(instance + 0x38, buf.Span))
                {
                    // Return the current status if read fails
                    for (int i = 0; i < 4; i++)
                        old[i] = current[i];

                    return old;
                }

                // Determine the number of details to read (max 4)
                no_of_details = buf.Span[8];
                if (no_of_details > 128)
                    no_of_details = 0;
                else if (no_of_details > 4)
                    no_of_details = 4;

                unsafe
                {
                    fixed (byte* ptr = buf.Span)
                        addr = (IntPtr)(*(long*)ptr);
                }
            }

            using (ArrayRental<long> details = new(stackalloc long[no_of_details]))
            {
                // Read the details from memory
                if (!process.ReadArray(addr, details.Span))
                {
                    // Return the current status if read fails
                    for (int i = 0; i < 4; i++)
                        old[i] = current[i];

                    return old;
                }

                // Look up each detail and store it in the return array
                for (int i = 0; i < no_of_details; i++)
                {
                    if (Engine.RTTI.Lookup((IntPtr)details.Span[i], out string value))
                        old[i] = value;
                }
            }

            for (int i = no_of_details; i < 4; i++)
                old[i] = string.Empty;

            return old;
        });

        LevelID = new LazyWatcher<LevelID>(StateTracker, SonicFrontiers.LevelID.MainMenu, (current, _) =>
        {
            if (Engine.GameMode == "GameModeTitle")
                return SonicFrontiers.LevelID.MainMenu;

            if (!Engine.GetService("LevelInfo", out IntPtr pLevelInfo)
                || !process.Read(pLevelInfo, out LevelInfo levelInfo)
                || !process.Read(levelInfo.stageData.Value, out StageData stageData)
                || !process.ReadString(stageData.Name.Value, 6, JHelper.Common.ProcessInterop.API.StringType.ASCII, out string id))
                return current;

            return id switch
            {
                "w6d01" => SonicFrontiers.LevelID.w1_1,    // 1-1
                "w8d01" => SonicFrontiers.LevelID.w1_2,    // 1-2
                "w9d04" => SonicFrontiers.LevelID.w1_3,    // 1-3
                "w6d02" => SonicFrontiers.LevelID.w1_4,    // 1-4
                "w7d04" => SonicFrontiers.LevelID.w1_5,    // 1-5
                "w6d06" => SonicFrontiers.LevelID.w1_6,    // 1-6
                "w9d06" => SonicFrontiers.LevelID.w1_7,    // 1-7
                "w6d05" => SonicFrontiers.LevelID.w2_1,    // 2-1
                "w8d03" => SonicFrontiers.LevelID.w2_2,    // 2-2
                "w7d02" => SonicFrontiers.LevelID.w2_3,    // 2-3
                "w7d06" => SonicFrontiers.LevelID.w2_4,    // 2-4
                "w8d04" => SonicFrontiers.LevelID.w2_5,    // 2-5
                "w6d03" => SonicFrontiers.LevelID.w2_6,    // 2-6
                "w8d05" => SonicFrontiers.LevelID.w2_7,    // 2-7
                "w6d04" => SonicFrontiers.LevelID.w3_1,    // 3-1
                "w6d08" => SonicFrontiers.LevelID.w3_2,    // 3-2
                "w8d02" => SonicFrontiers.LevelID.w3_3,    // 3-3
                "w6d09" => SonicFrontiers.LevelID.w3_4,    // 3-4
                "w6d07" => SonicFrontiers.LevelID.w3_5,    // 3-5
                "w8d06" => SonicFrontiers.LevelID.w3_6,    // 3-6
                "w7d03" => SonicFrontiers.LevelID.w3_7,    // 3-7
                "w7d08" => SonicFrontiers.LevelID.w4_1,    // 4-1
                "w9d02" => SonicFrontiers.LevelID.w4_2,    // 4-2
                "w7d01" => SonicFrontiers.LevelID.w4_3,    // 4-3
                "w9d03" => SonicFrontiers.LevelID.w4_4,    // 4-4
                "w6d10" => SonicFrontiers.LevelID.w4_5,    // 4-5
                "w7d07" => SonicFrontiers.LevelID.w4_6,    // 4-6
                "w9d05" => SonicFrontiers.LevelID.w4_7,    // 4-7
                "w7d05" => SonicFrontiers.LevelID.w4_8,    // 4-8
                "w9d07" => SonicFrontiers.LevelID.w4_9,    // 4-9
                "w6d21" => SonicFrontiers.LevelID.w4_A,    // 4-A
                "w6d22" => SonicFrontiers.LevelID.w4_B,    // 4-B
                "w6d23" => SonicFrontiers.LevelID.w4_C,    // 4-C
                "w7d21" => SonicFrontiers.LevelID.w4_D,    // 4-D
                "w7d22" => SonicFrontiers.LevelID.w4_E,    // 4-E
                "w7d23" => SonicFrontiers.LevelID.w4_F,    // 4-F
                "w9d21" => SonicFrontiers.LevelID.w4_G,    // 4-G
                "w9d22" => SonicFrontiers.LevelID.w4_H,    // 4-H
                "w9d23" => SonicFrontiers.LevelID.w4_I,    // 4-I
                "w1r03" => SonicFrontiers.LevelID.Island_Kronos,    // Kronos Island
                "w2r01" => SonicFrontiers.LevelID.Island_Ares,      // Ares Island
                "w3r01" => SonicFrontiers.LevelID.Island_Chaos,     // Chaos Island
                "w1r05" => SonicFrontiers.LevelID.Island_Rhea,      // Rhea Island
                "w1r04" => SonicFrontiers.LevelID.Island_Ouranos,   // Ouranos Island
                "w1r06" => SonicFrontiers.LevelID.Island_Another_Ouranos, //Another Story Ouranos
                "w1f01" => SonicFrontiers.LevelID.Fishing,          // Fishing
                "w0r01" => SonicFrontiers.LevelID.MainMenu,         // Main Menu
                "w5r01" => SonicFrontiers.LevelID.Boss_TheEnd,      // The End (boss)
                "w5t01" => SonicFrontiers.LevelID.Tutorial,         // Tutorial stage
                "w1b01" => SonicFrontiers.LevelID.Island_Kronos_BossRush,
                "w2b01" => SonicFrontiers.LevelID.Island_Ares_BossRush,
                "w3b01" => SonicFrontiers.LevelID.Island_Chaos_BossRush,
                "w1b02" => SonicFrontiers.LevelID.Island_Ouranos_BossRush,
                "w1h01" => SonicFrontiers.LevelID.Hacking_01,       // Chaos hacking
                "w1h02" => SonicFrontiers.LevelID.Hacking_02,       // Ouranos bridge hacking 
                "w1h03" => SonicFrontiers.LevelID.Hacking_03,       // Ouranos pyramid hacking
                _ => current,
            };
        });

        GameMode = new LazyWatcher<GameMode>(StateTracker, SonicFrontiers.GameMode.Story, (current, _) =>
        {
            if (liveSplitState.CurrentPhase == TimerPhase.NotRunning)
            {
                if (HsmStatus.Current[1] == "ArcadeMode" || Engine.ApplicationSequenceExtensionFlags0.BitCheck(0))
                    return SonicFrontiers.GameMode.Arcade;
                else if (HsmStatus.Current[1] == "CyberMode" || Engine.ApplicationSequenceExtensionFlags0.BitCheck(7) || (Engine.ApplicationSequenceExtensionFlags1 & 0b1111) != 0) //kronos,ares,chaos,ouranos,all = bits 7, (next byte) 0,1,2,3
                    return SonicFrontiers.GameMode.CyberspaceChallenge;
                else if (HsmStatus.Current[1] == "BattleMode" || LevelID.Current == SonicFrontiers.LevelID.Island_Kronos_BossRush || LevelID.Current == SonicFrontiers.LevelID.Island_Ares_BossRush || LevelID.Current == SonicFrontiers.LevelID.Island_Chaos_BossRush || LevelID.Current == SonicFrontiers.LevelID.Island_Ouranos_BossRush)
                    return SonicFrontiers.GameMode.BossRush;
                else
                    return SonicFrontiers.GameMode.Story;
            }
            return current;
        });

        IGT = new LazyWatcher<TimeSpan>(StateTracker, TimeSpan.Zero, (_, _) =>
        {
            if (Engine.GetExtension("GameModeStageTimeExtension", out IntPtr stageTimeExtension))
            {
                float igt = process.Read<float>(stageTimeExtension + 0x34) - Engine.IGTSubtraction;
                if (igt < 0)
                    igt = 0;
                return TimeSpan.FromSeconds(Math.Truncate(igt * 100) / 100);
            }
            else if (Engine.GetExtension("GameModeBattleRushExtension", out IntPtr battleRushExtension))
            {
                float igt = process.Read<float>(battleRushExtension + 0x38);
                return TimeSpan.FromSeconds(Math.Truncate(igt * 100) / 100);
            }
            else
            {
                return TimeSpan.Zero;
            }
        });

        MusicNotes = new LazyWatcher<byte[]>(StateTracker, [0, 0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0, 0, 0], (_, old) =>
        {
            old[0] = Flags._AD2;
            old[1] = Flags._AD3;
            old[2] = Flags._AD4;
            old[3] = Flags._AD5;
            old[4] = Flags._AD6;
            old[5] = Flags._AD7;
            old[6] = Flags._AD8;
            old[7] = Flags._AD9;

            return old;
        });

        StoryModeCyberSpaceCompletionFlag = new LazyWatcher<bool>(StateTracker, false, (current, old) => GameMode.Current == SonicFrontiers.GameMode.Story && LevelID.Current <= SonicFrontiers.LevelID.w4_I && (HsmStatus.Current[1] == "Result" || old));

        QTEStatus = new LazyWatcher<QTEResolveStatus>(StateTracker, QTEResolveStatus.NotCompleted, (_, _) =>
        {
            if (!IsInEndQTE || !Engine.GetObject("EventQTEInput", out IntPtr qte) || !process.Read<QTEResolveStatus>(qte + 0x254, out var status))
                return QTEResolveStatus.NotCompleted;
            else
                return status;
        });

        EndQTECount = new LazyWatcher<byte>(StateTracker, 0, (current, old) =>
        {
            if (LevelID.Current != SonicFrontiers.LevelID.Boss_TheEnd)
                return 0;
            if (QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Failed)
                return 0;
            if (current == 3 && !IsInEndQTE)
                return 0;
            if (current > 3)
                return 0;

            if (IsInEndQTE && QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Completed)
                return (byte)(current + 1);
            else
                return current;
        });

        AnotherQTECount = new LazyWatcher<byte>(StateTracker, 0, (current, _) =>
        {
            if (!IsInAnotherBoss)
                return 0;
            if (QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Failed)
                return 0;
            if (current == 2 && !IsInAnotherBoss)
                return 0;
            if (current > 2)
                return 0;
            
            if (IsInAnotherBoss && QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Completed)
                return (byte)(current + 1);
            else
                return current;

        });

        // Split bools
        SplitBools = new Dictionary<string, LazyWatcher<bool>>
        {
            //Another Story
            { "Amy_First",              new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Amy_First)},
            { "Amy_Second",             new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Amy_Second)},
            { "Knuckles_First",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Knuckles_First)},
            { "Knuckles_Second",        new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Knuckles_Second)},
            { "Tails_First",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Tails_First)},
            { "Tails_Second",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Tails_Second)},
            { "Sonic_Tower1",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Sonic_Tower1)},
            { "Sonic_Tower2",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Sonic_Tower2)},
            { "Sonic_Tower3",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Sonic_Tower3)},
            { "Sonic_Tower4",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Sonic_Tower4)},
            { "Sonic_MasterTrial",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Sonic_MasterTrial)},
            
            
            //Skills (Manual Unlocks)
            { "Skill_Cyloop",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_Cyloop) },
            { "Skill_PhantomRush",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_PhantomRush) },
            { "Skill_AirTrick",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_AirTrick) },
            { "Skill_StompAttack",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_StompAttack) },
            { "Skill_QuickCyloop",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_QuickCyloop) },
            { "Skill_SonicBoom",        new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_SonicBoom) },
            { "Skill_WildRush",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_WildRush) },
            { "Skill_LoopKick",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_LoopKick) },
            { "Skill_HomingShot",       new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_HomingShot) },
            { "Skill_AutoCombo",        new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_AutoCombo) },
            { "Skill_SpinSlash",        new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_SpinSlash) },
            { "Skill_RecoverySmash",    new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Skill_RecoverySmash) },

            // Kronos
            { "Kronos_Ninja",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_Ninja) },
            { "Kronos_Door",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_Door) },
            { "Kronos_Amy",             new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_Amy) },
            { "Kronos_GigantosFirst",   new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_GigantosFirst) },
            { "Kronos_GreenCE",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_GreenCE) },
            { "Kronos_CyanCE",          new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_CyanCE) },
            { "Kronos_Tombstones",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_Tombstones) },
            { "Kronos_BlueCE",          new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_BlueCE) },
            { "Kronos_RedCE",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_RedCE) },
            { "Kronos_YellowCE",        new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_YellowCE) },
            { "Kronos_WhiteCE",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Kronos_WhiteCE) },
            { "Kronos_GigantosStart",   new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Kronos && ScanBossHsm(process, "BossGiant", "WalkingBase")) },
            { "Kronos_SuperSonic",      new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Kronos && ScanBossHsm(process, "BossGiant", "BattlePhaseParent")) },
            { "Island_Kronos_story",    new LazyWatcher<bool>(StateTracker, false, (_, _) =>  LevelID.Old == SonicFrontiers.LevelID.Island_Kronos && LevelID.Current == SonicFrontiers.LevelID.Island_Ares) },
            { "Island_Kronos_fishing",  new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Kronos) },

            // Ares
            { "Ares_Knuckles",          new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_Knuckles) },
            { "Ares_WyvernFirst",       new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_WyvernFirst) },
            { "Ares_Water",             new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_Water) },
            // { "Ares_KocoRoundup",    new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_KocoRoundup },
            { "Ares_Crane",             new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_Crane) },
            { "Ares_GreenCE",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_GreenCE) },
            { "Ares_CyanCE",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_CyanCE) },
            { "Ares_BlueCE",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_BlueCE) },
            { "Ares_RedCE",             new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_RedCE) },
            { "Ares_YellowCE",          new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_YellowCE) },
            { "Ares_WhiteCE",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ares_WhiteCE) },
            { "Ares_WyvernStart",       new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && ScanBossHsm(process, "BossDragon", "PatrolTop")) },
            { "Ares_WyvernRun",         new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && ScanBossHsm(process, "BossDragon", "EventRise")) },
            { "Ares_SuperSonic",        new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && ScanBossHsm(process, "BossDragon", "BattleTop")) },
            { "Island_Ares_story",      new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Old == SonicFrontiers.LevelID.Island_Ares && LevelID.Current == SonicFrontiers.LevelID.Island_Chaos) },
            { "Island_Ares_fishing",    new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Ares) },

            // Chaos
            { "Chaos_Tails",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_Tails) },
            { "Chaos_KnightFirst",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_KnightFirst) },
            { "Chaos_Hacking",          new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_Hacking) },
            { "Chaos_GreenCE",          new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_GreenCE) },
            { "Chaos_CyanCE",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_CyanCE) },
            { "Chaos_PinballStart",     new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_PinballStart) },
            { "Chaos_PinballEnd",       new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_PinballEnd)},
            { "Chaos_BlueCE",           new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_BlueCE) },
            { "Chaos_RedCE",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_RedCE) },
            { "Chaos_YellowCE",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_YellowCE) },
            { "Chaos_WhiteCE",          new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_WhiteCE) },
            { "Chaos_KnightStart",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Chaos_KnightStart) },
            { "Chaos_SuperSonic",       new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Chaos && ScanBossHsm(process, "BossKnight", "Battle1Top")) },
            { "Island_Chaos_story",     new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Old == SonicFrontiers.LevelID.Island_Chaos && LevelID.Current == SonicFrontiers.LevelID.Island_Rhea) },
            { "Island_Chaos_fishing",   new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Chaos) },

            // Rhea
            { "Rhea_Tower1",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Rhea_Tower1) },
            { "Rhea_Tower2",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Rhea_Tower2) },
            { "Rhea_Tower3",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Rhea_Tower3) },
            { "Rhea_Tower4",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Rhea_Tower4) },
            { "Rhea_Tower5",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Rhea_Tower5) },
            { "Rhea_Tower6",            new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Rhea_Tower6) },
            { "Island_Rhea_story",      new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Old == SonicFrontiers.LevelID.Island_Rhea && LevelID.Current == SonicFrontiers.LevelID.Island_Ouranos) },

            // Ouranos
            { "Ouranos_Bridge",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ouranos_Bridge) },
            { "Ouranos_SupremeDefeated",new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Old == SonicFrontiers.LevelID.Island_Ouranos && LevelID.Current == SonicFrontiers.LevelID.Boss_TheEnd) },
            { "Ouranos_BlueCE",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ouranos_BlueCE) },
            { "Ouranos_RedCE",          new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ouranos_RedCE) },
            { "Ouranos_GreenCE",        new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ouranos_GreenCE) },
            { "Ouranos_YellowCE",       new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ouranos_YellowCE) },
            { "Ouranos_CyanCE",         new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ouranos_CyanCE) },
            //{ "Ouranos_WhiteCE",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ouranos_WhiteCE) },
            { "Ouranos_FinalDoor",      new LazyWatcher<bool>(StateTracker, false, (_, _) => Flags.Ouranos_FinalDoor) },
            { "Island_Ouranos_fishing", new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Old == SonicFrontiers.LevelID.Fishing && LevelID.Current == SonicFrontiers.LevelID.Island_Ouranos) }
        };
        
        BossRushAct = new LazyWatcher<BossRushAct>(StateTracker, SonicFrontiers.BossRushAct.None, (_, _) =>
        {
            
            var levelid = LevelID.Current;
            if (levelid != SonicFrontiers.LevelID.Island_Kronos_BossRush && levelid != SonicFrontiers.LevelID.Island_Ares_BossRush && levelid != SonicFrontiers.LevelID.Island_Chaos_BossRush && levelid != SonicFrontiers.LevelID.Island_Ouranos_BossRush)
                return SonicFrontiers.BossRushAct.None;

            if (!Engine.GetExtension("BattleRushExtension", out IntPtr battleRushExt)
                || !process.Read(battleRushExt + 0x2C, out byte phase))
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
    }

    internal void Update(ProcessMemory process, FrontiersSettings settings, LiveSplitState state)
    {
        ApplyHWNDpatch(process, settings);
        StateTracker.Tick();

        if (Engine.GetService("SaveManager", out IntPtr pSaveManager)
            && process.Read(pSaveManager, out SaveManager saveManager)
            && process.ReadPointer(saveManager.saveInterface.Value + 0x98, out IntPtr element)
            && process.ReadPointer(element, out IntPtr userElement))
        {
            using (ArrayRental<StoryFlags> flags = new ArrayRental<StoryFlags>(1))
            {
                var span = flags.Span;
                if (process.ReadArray(userElement, span))
                    Flags = span[0];
            }
        }

        

        StoryModeCyberSpaceCompletionFlag.Update();
        IsInTutorial = LevelID.Current == SonicFrontiers.LevelID.Tutorial;
        
        
        // I'm not happy I use 3 different variables to define the behaviour in the final QTE, but heh, it works
        IsInEndQTE = LevelID.Current == SonicFrontiers.LevelID.Boss_TheEnd && Engine.GetObject("EventQTEInput", out _);
        IsInAnotherBoss = IsFightingRifleBeast && Engine.GetObject("EventQTEInput", out _);
        

        

        // If the timer is not running (eg. a run has been reset) these variables need to be reset
        if (state.CurrentPhase == TimerPhase.NotRunning)
        {
            if (AccumulatedIGT != TimeSpan.Zero)
                AccumulatedIGT = TimeSpan.Zero;

            if (AlreadyTriggeredBools.Count > 0)
                AlreadyTriggeredBools.Clear();
        }
        if (GameMode.Current != SonicFrontiers.GameMode.Story)
        {
            // When exiting a stage, or whenever the IGT resets, this will keep track of the time you accumulated so far
            if (IGT.Current == TimeSpan.Zero && IGT.Old != TimeSpan.Zero)
                AccumulatedIGT += IGT.Old;
        }
    }

    /// <summary>
    /// Applies or removes the game patch to control focus behavior.
    /// </summary>
    /// <param name="process">The current process memory instance.</param>
    /// <param name="settings">The settings specifying whether to apply the patch.</param>
    private void ApplyHWNDpatch(ProcessMemory process, FrontiersSettings settings)
    {
        IntPtr address = Engine.HWndAddress;
        bool setting = settings.WFocus;

        if (process.Read<byte>(address, out byte val))
        {
            // If the game is unpatched and we want the patch to be applied
            if (val == 0x74 && setting)
                process.Write<byte>(Engine.HWndAddress, 0xEB); // Apply patch

            // If the game is patched and we want the patch to be removed
            else if (val == 0xEB && !setting)
                process.Write<byte>(Engine.HWndAddress, 0x74); // Remove patch
        }
    }

    /// <summary>
    /// Determines if the game is currently loading.
    /// </summary>
    /// <returns>True if the game is loading; otherwise, false.</returns>
    internal bool? IsLoading(FrontiersSettings settings)
    {
        //all these modes use IGT only
        if (GameMode.Current == SonicFrontiers.GameMode.Arcade || GameMode.Current == SonicFrontiers.GameMode.CyberspaceChallenge || GameMode.Current == SonicFrontiers.GameMode.BossRush)
        {
            Log.Warning($"IN IGT GAMEMODE {GameMode.Current}");
            return null;
        }

        if (!Engine.GetExtension("GameModeHsmExtension", out _)) {
            Log.Warning("CANNOT FIND HSM EXTENSION");
            return true;
        }
        
        if (LevelID.Current != SonicFrontiers.LevelID.MainMenu && Engine.GameMode == "GameModeLoad")
        {
            Log.Warning("GameModeLoad");
            return true;
        }
        if (IsInTutorial && (LevelID.Current <= SonicFrontiers.LevelID.w4_9 || LevelID.Current == SonicFrontiers.LevelID.Fishing))
        {
            Log.Warning("IN TUTORIAL");
            return true;
        }
        //fix for cyberspace taking randomly longer to finish in >=1.20
        if (LevelID.Current != SonicFrontiers.LevelID.MainMenu && (GameVersion == GameVersion.Unknown || GameVersion == GameVersion.v1_10) && HsmStatus.Current[1] == "Finish")
        {
            Log.Warning("FINISHING CYBERSPACE STAGE");
            return true;
        }
        if ((Engine.GameMode != "GameModeTitle"))
        {
            if (!Engine.GetObject("Sonic", out IntPtr sonicPtr))
            {
                Log.Warning("Sonic Not Found");
                Log.Info("HSMStatus: " + HsmStatus.Current[1]);
                return true;
            }
            else
            {
                Log.Info("HSMStatus: " + HsmStatus.Current[1]);
            }

        }
        
        return false;

    }

    internal bool Start(FrontiersSettings settings)
    {
        if (HsmStatus.Current[1] == "Quit" && HsmStatus.Old[1] == "NewGameMenu")
        {
            return settings.StoryStart;
        }
        else if(settings.ArcadeStart && GameMode.Current == SonicFrontiers.GameMode.CyberspaceChallenge)
        {
            return LevelID.Changed && LevelID.Old == SonicFrontiers.LevelID.MainMenu;
        }
        else if(settings.BossRushStart && GameMode.Current == SonicFrontiers.GameMode.BossRush)
        {
            return LevelID.Changed && LevelID.Old == SonicFrontiers.LevelID.MainMenu;
        }
        else if(LevelID.Current == SonicFrontiers.LevelID.Island_Another_Ouranos &&
            LevelID.Old == SonicFrontiers.LevelID.Island_Ouranos)
        {
            return true;
        }
        else if(settings.IslandILStart && LevelID.Old == SonicFrontiers.LevelID.MainMenu && (LevelID.Current == SonicFrontiers.LevelID.Island_Ares || LevelID.Current == SonicFrontiers.LevelID.Island_Chaos || LevelID.Current == SonicFrontiers.LevelID.Island_Rhea || LevelID.Current == SonicFrontiers.LevelID.Island_Ouranos))
        {
            AlreadyTriggeredBools.Add("Island_Kronos_story");
            return true;
        }
        return false;
    }

    internal bool Split(FrontiersSettings settings)
    {
        if (GameMode.Current == SonicFrontiers.GameMode.Arcade || GameMode.Current == SonicFrontiers.GameMode.CyberspaceChallenge)
        {
            if (!CheckArcadeSplit(LevelID.Old, settings))
            {
                return false;
            }
            if (LevelID.Old == SonicFrontiers.LevelID.w4_9 && settings.w4_9_arcade_soon)
            {
                return HsmStatus.Changed && HsmStatus.Current[1] == "Finish";
            }
            return HsmStatus.Changed && HsmStatus.Old[1] == "Result";
        }

        if (GameMode.Current == SonicFrontiers.GameMode.BossRush)
        {
            return BossRushAct.Old switch
            {
                SonicFrontiers.BossRushAct.b1_1 => settings.Boss1_1 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_2,
                SonicFrontiers.BossRushAct.b1_2 => settings.Boss1_2 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_3,
                SonicFrontiers.BossRushAct.b1_3 => settings.Boss1_3 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_4,
                SonicFrontiers.BossRushAct.b1_4 => settings.Boss1_4 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_5,
                SonicFrontiers.BossRushAct.b1_5 => settings.Boss1_5 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_6,
                SonicFrontiers.BossRushAct.b1_6 => settings.Boss1_6 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_7,
                SonicFrontiers.BossRushAct.b1_7 => settings.Boss1_7 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_8,
                SonicFrontiers.BossRushAct.b1_8 => settings.Boss1_8 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_9,
                SonicFrontiers.BossRushAct.b1_9 => settings.Boss1_9 && BossRushAct.Current == SonicFrontiers.BossRushAct.b1_10,
                SonicFrontiers.BossRushAct.b1_10 => settings.Boss1_10 && ((IGT.Old > IGT.Current && IGT.Current == TimeSpan.Zero)), // || (Status.Changed && Status.Current == Status.StageResult)),
                SonicFrontiers.BossRushAct.b2_1 => settings.Boss2_1 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_2,
                SonicFrontiers.BossRushAct.b2_2 => settings.Boss2_2 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_3,
                SonicFrontiers.BossRushAct.b2_3 => settings.Boss2_3 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_4,
                SonicFrontiers.BossRushAct.b2_4 => settings.Boss2_4 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_5,
                SonicFrontiers.BossRushAct.b2_5 => settings.Boss2_5 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_6,
                SonicFrontiers.BossRushAct.b2_6 => settings.Boss2_6 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_7,
                SonicFrontiers.BossRushAct.b2_7 => settings.Boss2_7 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_8,
                SonicFrontiers.BossRushAct.b2_8 => settings.Boss2_8 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_9,
                SonicFrontiers.BossRushAct.b2_9 => settings.Boss2_9 && BossRushAct.Current == SonicFrontiers.BossRushAct.b2_10,
                SonicFrontiers.BossRushAct.b2_10 => settings.Boss2_10 && (IGT.Old > IGT.Current && IGT.Current == TimeSpan.Zero), // || (Status.Changed && Status.Current == Status.StageResult)),
                SonicFrontiers.BossRushAct.b3_1 => settings.Boss3_1 && BossRushAct.Current == SonicFrontiers.BossRushAct.b3_2,
                SonicFrontiers.BossRushAct.b3_2 => settings.Boss3_2 && BossRushAct.Current == SonicFrontiers.BossRushAct.b3_3,
                SonicFrontiers.BossRushAct.b3_3 => settings.Boss3_3 && BossRushAct.Current == SonicFrontiers.BossRushAct.b3_4,
                SonicFrontiers.BossRushAct.b3_4 => settings.Boss3_4 && BossRushAct.Current == SonicFrontiers.BossRushAct.b3_5,
                SonicFrontiers.BossRushAct.b3_5 => settings.Boss3_5 && BossRushAct.Current == SonicFrontiers.BossRushAct.b3_6,
                SonicFrontiers.BossRushAct.b3_6 => settings.Boss3_6 && BossRushAct.Current == SonicFrontiers.BossRushAct.b3_7,
                SonicFrontiers.BossRushAct.b3_7 => settings.Boss3_7 && BossRushAct.Current == SonicFrontiers.BossRushAct.b3_8,
                SonicFrontiers.BossRushAct.b3_8 => settings.Boss3_8 && BossRushAct.Current == SonicFrontiers.BossRushAct.b3_9,
                SonicFrontiers.BossRushAct.b3_9 => settings.Boss3_9 && (IGT.Old > IGT.Current && IGT.Current == TimeSpan.Zero), // || (Status.Changed && Status.Current == Status.StageResult)),
                SonicFrontiers.BossRushAct.b4_1 => settings.Boss4_1 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_2,
                SonicFrontiers.BossRushAct.b4_2 => settings.Boss4_2 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_3,
                SonicFrontiers.BossRushAct.b4_3 => settings.Boss4_3 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_4,
                SonicFrontiers.BossRushAct.b4_4 => settings.Boss4_4 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_5,
                SonicFrontiers.BossRushAct.b4_5 => settings.Boss4_5 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_6,
                SonicFrontiers.BossRushAct.b4_6 => settings.Boss4_6 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_7,
                SonicFrontiers.BossRushAct.b4_7 => settings.Boss4_7 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_8,
                SonicFrontiers.BossRushAct.b4_8 => settings.Boss4_8 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_9,
                SonicFrontiers.BossRushAct.b4_9 => settings.Boss4_9 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_10,
                SonicFrontiers.BossRushAct.b4_10 => settings.Boss4_10 && BossRushAct.Current == SonicFrontiers.BossRushAct.b4_11,
                SonicFrontiers.BossRushAct.b4_11 => settings.Boss4_11 && HsmStatus.Changed && HsmStatus.Current[1] == "StageResult",
                _ => false
            };
        }

        //Story Mode

        foreach (var flag in SplitBools.Where(b => AlreadyTriggeredBools.Contains(b.Key)))
        {
            if (CheckBoolSplit(flag.Key, settings) && !flag.Value.Old && flag.Value.Current)
            {
                AlreadyTriggeredBools.Add(flag.Key);
                return true;
            }
        }

        //Music Notes
        if (settings.MusicNoteAny && MusicNotes.Changed && MusicNotes.Old != null)
        {
            return true;
        }
        if (MusicNotes.Old == null)
        {
            Log.Warning("musicnotes old is null");
        }

        //Island Swap
        if (settings.IslandSwapSplit && LevelID.Current == SonicFrontiers.LevelID.Island_Kronos)
        {

            if (LevelID.Old == SonicFrontiers.LevelID.Island_Ares || LevelID.Old == SonicFrontiers.LevelID.Island_Chaos || LevelID.Old == SonicFrontiers.LevelID.Island_Rhea || LevelID.Old == SonicFrontiers.LevelID.Island_Ouranos)
            {
                return true;
            }
        }

        //Enter Cyberspace Stage
        if (settings.EnterCyberspaceSplit && (LevelID.Current < SonicFrontiers.LevelID.Island_Kronos || LevelID.Current == SonicFrontiers.LevelID.Fishing))
        {
            if (LevelID.Old == SonicFrontiers.LevelID.Island_Ares || LevelID.Old == SonicFrontiers.LevelID.Island_Chaos || LevelID.Old == SonicFrontiers.LevelID.Island_Another_Ouranos || LevelID.Old == SonicFrontiers.LevelID.Island_Ouranos)
            {
                return true;
            }
        }

        //Start Hacking
        if((settings.Chaos_HackingStart && LevelID.Current == SonicFrontiers.LevelID.Hacking_01 && LevelID.Old == SonicFrontiers.LevelID.Island_Chaos)
            || (settings.Ouranos_FirstHackingStart && LevelID.Current == SonicFrontiers.LevelID.Hacking_02 && LevelID.Old == SonicFrontiers.LevelID.Island_Ouranos)
            || (settings.Ouranos_SecondHackingStart && LevelID.Current == SonicFrontiers.LevelID.Hacking_03 && LevelID.Old == SonicFrontiers.LevelID.Island_Ouranos)){
            return true;
        }

        //Cyberspace within story mode
        return CheckStorySplit(LevelID.Old, settings) && StoryModeCyberSpaceCompletionFlag.Old && !StoryModeCyberSpaceCompletionFlag.Current;
    }
    private bool CheckArcadeSplit(LevelID input, FrontiersSettings settings) => input switch
    {
        SonicFrontiers.LevelID.w1_1 => settings.w1_1_arcade,
        SonicFrontiers.LevelID.w1_2 => settings.w1_2_arcade,
        SonicFrontiers.LevelID.w1_3 => settings.w1_3_arcade,
        SonicFrontiers.LevelID.w1_4 => settings.w1_4_arcade,
        SonicFrontiers.LevelID.w1_5 => settings.w1_5_arcade,
        SonicFrontiers.LevelID.w1_6 => settings.w1_6_arcade,
        SonicFrontiers.LevelID.w1_7 => settings.w1_7_arcade,
        SonicFrontiers.LevelID.w2_1 => settings.w2_1_arcade,
        SonicFrontiers.LevelID.w2_2 => settings.w2_2_arcade,
        SonicFrontiers.LevelID.w2_3 => settings.w2_3_arcade,
        SonicFrontiers.LevelID.w2_4 => settings.w2_4_arcade,
        SonicFrontiers.LevelID.w2_5 => settings.w2_5_arcade,
        SonicFrontiers.LevelID.w2_6 => settings.w2_6_arcade,
        SonicFrontiers.LevelID.w2_7 => settings.w2_7_arcade,
        SonicFrontiers.LevelID.w3_1 => settings.w3_1_arcade,
        SonicFrontiers.LevelID.w3_2 => settings.w3_2_arcade,
        SonicFrontiers.LevelID.w3_3 => settings.w3_3_arcade,
        SonicFrontiers.LevelID.w3_4 => settings.w3_4_arcade,
        SonicFrontiers.LevelID.w3_5 => settings.w3_5_arcade,
        SonicFrontiers.LevelID.w3_6 => settings.w3_6_arcade,
        SonicFrontiers.LevelID.w3_7 => settings.w3_7_arcade,
        SonicFrontiers.LevelID.w4_1 => settings.w4_1_arcade,
        SonicFrontiers.LevelID.w4_2 => settings.w4_2_arcade,
        SonicFrontiers.LevelID.w4_3 => settings.w4_3_arcade,
        SonicFrontiers.LevelID.w4_4 => settings.w4_4_arcade,
        SonicFrontiers.LevelID.w4_5 => settings.w4_5_arcade,
        SonicFrontiers.LevelID.w4_6 => settings.w4_6_arcade,
        SonicFrontiers.LevelID.w4_7 => settings.w4_7_arcade,
        SonicFrontiers.LevelID.w4_8 => settings.w4_8_arcade,
        SonicFrontiers.LevelID.w4_9 => settings.w4_9_arcade,
        _ => false,
    };

    private bool CheckStorySplit(LevelID input, FrontiersSettings settings) => input switch
    {
        SonicFrontiers.LevelID.w1_1 => settings.w1_1_story,
        SonicFrontiers.LevelID.w1_2 => settings.w1_2_story,
        SonicFrontiers.LevelID.w1_3 => settings.w1_3_story,
        SonicFrontiers.LevelID.w1_4 => settings.w1_4_story,
        SonicFrontiers.LevelID.w1_5 => settings.w1_5_story,
        SonicFrontiers.LevelID.w1_6 => settings.w1_6_story,
        SonicFrontiers.LevelID.w1_7 => settings.w1_7_story,
        SonicFrontiers.LevelID.w2_1 => settings.w2_1_story,
        SonicFrontiers.LevelID.w2_2 => settings.w2_2_story,
        SonicFrontiers.LevelID.w2_3 => settings.w2_3_story,
        SonicFrontiers.LevelID.w2_4 => settings.w2_4_story,
        SonicFrontiers.LevelID.w2_5 => settings.w2_5_story,
        SonicFrontiers.LevelID.w2_6 => settings.w2_6_story,
        SonicFrontiers.LevelID.w2_7 => settings.w2_7_story,
        SonicFrontiers.LevelID.w3_1 => settings.w3_1_story,
        SonicFrontiers.LevelID.w3_2 => settings.w3_2_story,
        SonicFrontiers.LevelID.w3_3 => settings.w3_3_story,
        SonicFrontiers.LevelID.w3_4 => settings.w3_4_story,
        SonicFrontiers.LevelID.w3_5 => settings.w3_5_story,
        SonicFrontiers.LevelID.w3_6 => settings.w3_6_story,
        SonicFrontiers.LevelID.w3_7 => settings.w3_7_story,
        SonicFrontiers.LevelID.w4_1 => settings.w4_1_story,
        SonicFrontiers.LevelID.w4_2 => settings.w4_2_story,
        SonicFrontiers.LevelID.w4_3 => settings.w4_3_story,
        SonicFrontiers.LevelID.w4_4 => settings.w4_4_story,
        SonicFrontiers.LevelID.w4_5 => settings.w4_5_story,
        SonicFrontiers.LevelID.w4_6 => settings.w4_6_story,
        SonicFrontiers.LevelID.w4_7 => settings.w4_7_story,
        SonicFrontiers.LevelID.w4_8 => settings.w4_8_story,
        SonicFrontiers.LevelID.w4_9 => settings.w4_9_story,
        SonicFrontiers.LevelID.w4_A => settings.w4_A_story,
        SonicFrontiers.LevelID.w4_B => settings.w4_B_story,
        SonicFrontiers.LevelID.w4_C => settings.w4_C_story,
        SonicFrontiers.LevelID.w4_D => settings.w4_D_story,
        SonicFrontiers.LevelID.w4_E => settings.w4_E_story,
        SonicFrontiers.LevelID.w4_F => settings.w4_F_story,
        SonicFrontiers.LevelID.w4_G => settings.w4_G_story,
        SonicFrontiers.LevelID.w4_H => settings.w4_H_story,
        SonicFrontiers.LevelID.w4_I => settings.w4_I_story,
        _ => false,
    };

    private bool CheckBoolSplit(string key, FrontiersSettings settings) => key switch
    {
        "Amy_First" => settings.Amy_First,
        "Knuckles_First" => settings.Knuckles_First,
        "Tails_First" => settings.Tails_First,
        "Amy_Second" => settings.Amy_Second,
        "Knuckles_Second" => settings.Knuckles_Second,
        "Tails_Second" => settings.Tails_Second,
        "Sonic_MasterTrial" => settings.Sonic_MasterTrial,
        "Sonic_Tower1" => settings.Sonic_Tower1,
        "Sonic_Tower2" => settings.Sonic_Tower2,
        "Sonic_Tower3" => settings.Sonic_Tower3,
        "Sonic_Tower4" => settings.Sonic_Tower4,
        "Skill_Cyloop" => settings.Skill_Cyloop,
        "Skill_PhantomRush" => settings.Skill_PhantomRush,
        "Skill_AirTrick" => settings.Skill_AirTrick,
        "Skill_StompAttack" => settings.Skill_StompAttack,
        "Skill_QuickCyloop" => settings.Skill_QuickCyloop,
        "Skill_SonicBoom" => settings.Skill_SonicBoom,
        "Skill_WildRush" => settings.Skill_WildRush,
        "Skill_LoopKick" => settings.Skill_LoopKick,
        "Skill_HomingShot" => settings.Skill_HomingShot,
        "Skill_AutoCombo" => settings.Skill_AutoCombo,
        "Skill_SpinSlash" => settings.Skill_SpinSlash,
        "Skill_RecoverySmash" => settings.Skill_RecoverySmash,
        "Kronos_Ninja" => settings.Kronos_Ninja,
        "Kronos_Door" => settings.Kronos_Door,
        "Kronos_Amy" => settings.Kronos_Amy,
        "Kronos_GigantosFirst" => settings.Kronos_GigantosFirst,
        "Kronos_GreenCE" => settings.Kronos_GreenCE,
        "Kronos_CyanCE" => settings.Kronos_CyanCE,
        "Kronos_Tombstones" => settings.Kronos_Tombstones,
        "Kronos_BlueCE" => settings.Kronos_BlueCE,
        "Kronos_RedCE" => settings.Kronos_RedCE,
        "Kronos_YellowCE" => settings.Kronos_YellowCE,
        "Kronos_WhiteCE" => settings.Kronos_WhiteCE,
        "Kronos_GigantosStart" => settings.Kronos_GigantosStart,
        "Kronos_SuperSonic" => settings.Kronos_SuperSonic,
        "Island_Kronos_story" => settings.Island_Kronos_story,
        "Island_Kronos_fishing" => settings.Island_Kronos_fishing,
        "Ares_Knuckles" => settings.Ares_Knuckles,
        "Ares_WyvernFirst" => settings.Ares_WyvernFirst,
        "Ares_Water" => settings.Ares_Water,
        // "Ares_KocoRoundup"  => settings.Ares_KocoRoundup,
        "Ares_Crane" => settings.Ares_Crane,
        "Ares_GreenCE" => settings.Ares_GreenCE,
        "Ares_CyanCE" => settings.Ares_CyanCE,
        "Ares_BlueCE" => settings.Ares_BlueCE,
        "Ares_RedCE" => settings.Ares_RedCE,
        "Ares_YellowCE" => settings.Ares_YellowCE,
        "Ares_WhiteCE" => settings.Ares_WhiteCE,
        "Ares_WyvernStart" => settings.Ares_WyvernStart,
        "Ares_WyvernRun" => settings.Ares_WyvernRun,
        "Ares_SuperSonic" => settings.Ares_SuperSonic,
        "Island_Ares_story" => settings.Island_Ares_story,
        "Island_Ares_fishing" => settings.Island_Ares_fishing,
        "Chaos_Tails" => settings.Chaos_Tails,
        "Chaos_KnightFirst" => settings.Chaos_KnightFirst,
        "Chaos_Hacking" => settings.Chaos_Hacking,
        "Chaos_GreenCE" => settings.Chaos_GreenCE,
        "Chaos_CyanCE" => settings.Chaos_CyanCE,
        "Chaos_PinballStart" => settings.Chaos_PinballStart,
        "Chaos_PinballEnd" => settings.Chaos_PinballEnd,
        "Chaos_BlueCE" => settings.Chaos_BlueCE,
        "Chaos_RedCE" => settings.Chaos_RedCE,
        "Chaos_YellowCE" => settings.Chaos_YellowCE,
        "Chaos_WhiteCE" => settings.Chaos_WhiteCE,
        "Chaos_KnightStart" => settings.Chaos_KnightStart,
        "Chaos_SuperSonic" => settings.Chaos_SuperSonic,
        "Island_Chaos_story" => settings.Island_Chaos_story,
        "Island_Chaos_fishing" => settings.Island_Chaos_fishing,
        "Rhea_Tower1" => settings.Rhea_Tower1,
        "Rhea_Tower2" => settings.Rhea_Tower2,
        "Rhea_Tower3" => settings.Rhea_Tower3,
        "Rhea_Tower4" => settings.Rhea_Tower4,
        "Rhea_Tower5" => settings.Rhea_Tower5,
        "Rhea_Tower6" => settings.Rhea_Tower6,
        "Island_Rhea_story" => settings.Island_Rhea_story,
        "Ouranos_FirstHackingStart" => settings.Ouranos_FirstHackingStart,
        "Ouranos_Bridge" => settings.Ouranos_Bridge,
        "Ouranos_SupremeDefeated" => settings.Ouranos_SupremeDefeated,
        "Ouranos_BlueCE" => settings.Ouranos_BlueCE,
        "Ouranos_RedCE" => settings.Ouranos_RedCE,
        "Ouranos_GreenCE" => settings.Ouranos_GreenCE,
        "Ouranos_YellowCE" => settings.Ouranos_YellowCE,
        "Ouranos_CyanCE" => settings.Ouranos_CyanCE,
        //"Ouranos_WhiteCE" => settings.Ouranos_WhiteCE,
        "Island_Ouranos_fishing" => settings.Island_Ouranos_fishing,
        "Ouranos_SecondHackingStart" => settings.Ouranos_SecondHackingStart,
        "Ouranos_FinalDoor" => settings.Ouranos_FinalDoor,
        _ => false,
    };
    
    internal bool Reset(FrontiersSettings settings)
    {
        return HsmStatus.Current[1] == "Quit" && HsmStatus.Old[1] == "NewGameMenu" && settings.AutoReset;
    }

    private bool ScanBossHsm(ProcessMemory process, string bossName, string bossMove)
    {
        if (!Engine.GetObject(bossName, out IntPtr boss) || !process.Read(boss, out Boss bossData))
            return false;

        IntPtr gocHsm2 = IntPtr.Zero;

        using (ArrayRental<Address<long>> buf = new(bossData.noOfElements))
        {
            if (!process.ReadArray(bossData.array.Value, buf.Span))
                return false;

            foreach (var entry in buf.Span)
            {
                if (!process.Read(entry.Value, out BossHsm bossHsm)
                    || !process.ReadPointer(bossHsm.statik.Value, out IntPtr name)
                    || !process.ReadPointer(name, out name)
                    || !process.ReadString(name, 127, StringType.ASCII, out string val)
                    || val != "GOCHsm2")
                    continue;

                using (ArrayRental<Address<long>> newBuf = new(bossHsm.noOfElements))
                {
                    if (!process.ReadArray(bossData.array.Value, newBuf.Span))
                        return false;

                    foreach (var element in newBuf.Span)
                    {
                        if (Engine.RTTI.Lookup(element.Value, out val) && val == bossMove)
                            return true;
                    }
                }
            }
        }

        return false;
    }


    /// <summary>
    /// This function is essentially equivalent of the init descriptor in script-based autosplitters.
    /// Everything you want to be executed when the game gets hooked needs to be put here.
    /// The main purpose of this function is to perform sigscanning and get memory addresses and offsets
    /// needed by the autosplitter.
    /// </summary>
    
    /*
    private void GetAddresses()
    {

        // These offsets are known to change so we will dynamically find them through specific sigscanning
        IntPtr igtPtr = scanner.Scan(new SigScanTarget(4, "F3 0F 11 49 ?? F3 0F 5C 0D"));
        IntPtr igtsubOffset = igtPtr + 5;
        if (igtPtr == IntPtr.Zero)
        {
            igtPtr = scanner.ScanOrThrow(new SigScanTarget(4, "F3 0F 11 49 ?? F3 41 0F 58 ?? F3 0F 5C 0D"));
            igtsubOffset = igtPtr + 10;
        }

        // Defining a new instance of the RTTI class in order to get the vTable addresses of a couple of classes.
        // This makes it incredibly easy to calculate some dynamic offsets later,
        var RTTILIST = new List<string>
        {

            "BossRifleBeast::app" //new!
        };

        // Old game versions do not have Battle Rush
        if (GameVersion != GameVersion.v1_01 && GameVersion != GameVersion.v1_10)
            RTTILIST.Add("GameModeBattleRushExtension::game::app");
    }
    */
}