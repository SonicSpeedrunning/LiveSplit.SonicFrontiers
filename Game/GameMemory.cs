using JHelper.Common.Collections;
using JHelper.Common.MemoryUtils;
using JHelper.Common.ProcessInterop;
using LiveSplit.ComponentUtil;
using LiveSplit.Model;
using LiveSplit.SonicFrontiers.GameEngine;
using System;
using System.Collections.Generic;

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
    /// Watches the current game mode (e.g., main menu, in-game).
    /// </summary>
    public LazyWatcher<string> GameMode { get; }
    
    


    
    // Story flags
    private StoryFlags Flags;
    public Dictionary<string, LazyWatcher<bool>> SplitBools { get; }
    public List<string> AlreadyTriggeredBools { get; } = new List<string>();

    // Boss Rush
    public FakeMemoryWatcher<BossRushAct> BossRushAct { get; }
    
    // Music Notes
    public FakeMemoryWatcher<byte[]> MusicNotes;

    /// <summary>
    /// Constructor initializing game engine, version, and various game state watchers.
    /// </summary>
    /// <param name="process">The current game process.</param>
    public Memory(ProcessMemory process)
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

        this.GameMode = new LazyWatcher<string>(StateTracker, "GameModeTitle", (current, _) =>
        {
            return Engine.GameMode == string.Empty ? current : Engine.GameMode;
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
            if (GameMode.Current == "GameModeTitle")
                return SonicFrontiers.LevelID.MainMenu;

            if (!Engine.GetService("LevelInfo", out IntPtr pLevelInfo)
                || !process.Read(pLevelInfo, out LevelInfo levelInfo)
                || levelInfo.StageData == IntPtr.Zero
                || !process.Read(levelInfo.StageData, out StageData stageData)
                || stageData.Name == IntPtr.Zero
                || !process.ReadString(stageData.Name, 6, JHelper.Common.ProcessInterop.API.StringType.ASCII, out string id))
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


        MusicNotes = new LazyWatcher<byte[]>(StateTracker, [0, 0, 0, 0, 0, 0, 0, 0], [0, 0, 0, 0, 0, 0, 0, 0], (current, old) =>
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


        StoryModeCyberSpaceCompletionFlag = new LazyWatcher<bool>(StateTracker, false, (_, _) => GameMode.Current == GameMode.Story && LevelID.Current <= SonicFrontiers.LevelID.w4_I && (Status.Current == SonicFrontiers.Status.Result || StoryModeCyberSpaceCompletionFlag.Old));

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
        AnotherQTECount = new FakeMemoryWatcher<byte>(() =>
        {
            if (!IsInAnotherBoss)
                return 0;
            if (QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Failed)
                return 0;
            if (AnotherQTECount.Current == 2 && !IsInAnotherBoss)
                return 0;
            if (AnotherQTECount.Current > 2)
                return 0;
            
            if (IsInAnotherBoss && QTEStatus.Changed && QTEStatus.Current == QTEResolveStatus.Completed)
                return (byte)(AnotherQTECount.Current + 1);
            else
                return AnotherQTECount.Current;

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
            { "Kronos_GigantosStart",   new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Kronos && !addresses["CURRENTBOSSSTATUS"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS"], 255, out var value) && value == "WalkingBase") },
            { "Kronos_SuperSonic",      new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Kronos && !addresses["CURRENTBOSSSTATUS"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS"], 255, out var value) && value == "BattlePhaseParent") },
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
            { "Ares_WyvernStart",       new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSSSTATUS_WYVERN"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS_WYVERN"], 255, out var value) && value == "PatrolTop") },
            { "Ares_WyvernRun",         new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSSSTATUS_WYVERN"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS_WYVERN"], 255, out var value) && value == "EventRise") },
            { "Ares_SuperSonic",        new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Ares && !addresses["CURRENTBOSSSTATUS_WYVERN"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS_WYVERN"], 255, out var value) && value == "BattleTop") },
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
            { "Chaos_SuperSonic",       new LazyWatcher<bool>(StateTracker, false, (_, _) => LevelID.Current == SonicFrontiers.LevelID.Island_Chaos && !addresses["CURRENTBOSSSTATUS"].IsZero() && game.ReadString(addresses["CURRENTBOSSSTATUS"], 255, out var value) && value == "Battle1Top") },
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
    }

    internal void Update(ProcessMemory process, Settings settings)
    {
        ApplyHWNDpatch(process, settings);
        StateTracker.Tick();

        if (Engine.GetService("SaveManager", out IntPtr pSaveManager)
            && pSaveManager != IntPtr.Zero
            && process.Read(pSaveManager, out SaveManager saveManager)
            && saveManager.SaveInterface != IntPtr.Zero
            && process.ReadPointer(saveManager.SaveInterface + 0x98, out IntPtr element)
            && element != IntPtr.Zero
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
        IsInEndQTE = LevelID.Current == SonicFrontiers.LevelID.Boss_TheEnd && !addresses["QTE"].IsZero() && (IntPtr)game.ReadValue<long>(addresses["QTE"]) == RTTI["EventQTEInput::evt::app"];
        IsInAnotherBoss = IsFightingRifleBeast &&
                          !addresses["QTE"].IsZero() && (IntPtr)game.ReadValue<long>(addresses["QTE"]) == RTTI["EventQTEInput::evt::app"];
        

        

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
    /// Applies or removes the game patch to control focus behavior.
    /// </summary>
    /// <param name="process">The current process memory instance.</param>
    /// <param name="settings">The settings specifying whether to apply the patch.</param>
    private void ApplyHWNDpatch(ProcessMemory process, Settings settings)
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
    internal bool? IsLoading(Settings settings)
    {
        return !Engine.GetExtension("GameModeHsmExtension", out _) || CurrentGameMode == GameMode.Arcade || CurrentGameMode == GameMode.CyberspaceChallenge || CurrentGameMode == GameMode.BossRush
                        || (LevelID.Current != SonicFrontiers.LevelID.MainMenu && GameModeLoad)
                        || (IsInTutorial && (LevelID.Current <= SonicFrontiers.LevelID.w4_9 || watchers.LevelID.Current == SonicFrontiers.LevelID.Fishing))
                        || (LevelID.Current != SonicFrontiers.LevelID.MainMenu && (GameVersion == GameVersion.Unknown || GameVersion == GameVersion.v1_10) && Status.Current == Status.Finish);
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
        offsets["QTE"] = 0xD0;
        if (GameVersion == GameVersion.v1_10 || GameVersion == GameVersion.Unknown)
        {
            offsets["QTE"] = 0xE0;
        }
        // These offsets are known to change so we will dynamically find them through specific sigscanning
        IntPtr igtPtr = scanner.Scan(new SigScanTarget(4, "F3 0F 11 49 ?? F3 0F 5C 0D"));
        IntPtr igtsubOffset = igtPtr + 5;
        if (igtPtr == IntPtr.Zero)
        {
            igtPtr = scanner.ScanOrThrow(new SigScanTarget(4, "F3 0F 11 49 ?? F3 41 0F 58 ?? F3 0F 5C 0D"));
            igtsubOffset = igtPtr + 10;
        }
        offsets["cyberstage_igt"] = game.ReadValue<byte>(igtPtr);
        addresses["igt_subtraction"] = igtsubOffset + 0x4 + game.ReadValue<int>(igtsubOffset);

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
            "BossKnight::app",
            "BossRifleBeast::app" //new!
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
        addresses["AnotherFinalBoss"] = IntPtr.Zero;
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
                _addr = (IntPtr)game.ReadValue<long>(_addr + offsets["QTE"]);
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
}