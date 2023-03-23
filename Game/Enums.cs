using System.Collections.Generic;

namespace LiveSplit.SonicFrontiers
{
    enum GameMode
    {
        Story,
        Arcade,
        CyberspaceChallenge,
        BossRush,
    }

    enum LevelID
    {
        w1_1,
        w1_2,
        w1_3,
        w1_4,
        w1_5,
        w1_6,
        w1_7,
        w2_1,
        w2_2,
        w2_3,
        w2_4,
        w2_5,
        w2_6,
        w2_7,
        w3_1,
        w3_2,
        w3_3,
        w3_4,
        w3_5,
        w3_6,
        w3_7,
        w4_1,
        w4_2,
        w4_3,
        w4_4,
        w4_5,
        w4_6,
        w4_7,
        w4_8,
        w4_9,
        Island_Kronos,
        Island_Ares,
        Island_Chaos,
        Island_Rhea,
        Island_Ouranos,
        Fishing,
        MainMenu,
        Boss_TheEnd,
        Tutorial,
        None,
        Island_Kronos_BossRush,
        Island_Ares_BossRush,
        Island_Chaos_BossRush,
        Island_Ouranos_BossRush,
    }

    enum Status
    {
        TopMenu,
        NewGameMenu,
        ContinueMenu,
        ArcadeMode,
        BattleMode,
        CyberMode,
        SelectLanguage,
        ChangingLanguage,
        ViewLicense,
        ViewQRCode,
        ExtraContents,
        CheckQuit,
        Quit,
        PlayTop,
        Finish,
        Result,
        Build,
        StageResult,
        Default, // Used as default
    }

    enum QTEResolveStatus : byte
    {
        NotCompleted = 0,
        Completed = 1,
        Failed = 2
    }

    enum BossRushAct
    {
        None,
        b1_1,
        b1_2,
        b1_3,
        b1_4,
        b1_5,
        b1_6,
        b1_7,
        b1_8,
        b1_9,
        b1_10,
        b2_1,
        b2_2,
        b2_3,
        b2_4,
        b2_5,
        b2_6,
        b2_7,
        b2_8,
        b2_9,
        b2_10,
        b3_1,
        b3_2,
        b3_3,
        b3_4,
        b3_5,
        b3_6,
        b3_7,
        b3_8,
        b3_9,
        b4_1,
        b4_2,
        b4_3,
        b4_4,
        b4_5,
        b4_6,
        b4_7,
        b4_8,
        b4_9,
        b4_10,
        b4_11,
    }

    readonly struct Enums
    {
        public static readonly Dictionary<string, LevelID> LevelID = new Dictionary<string, LevelID>
        {
            { "w6d01", SonicFrontiers.LevelID.w1_1 },     // 1-1
            { "w8d01", SonicFrontiers.LevelID.w1_2 },     // 1-2
            { "w9d04", SonicFrontiers.LevelID.w1_3 },     // 1-3
            { "w6d02", SonicFrontiers.LevelID.w1_4 },     // 1-4
            { "w7d04", SonicFrontiers.LevelID.w1_5 },     // 1-5
            { "w6d06", SonicFrontiers.LevelID.w1_6 },     // 1-6
            { "w9d06", SonicFrontiers.LevelID.w1_7 },     // 1-7
            { "w6d05", SonicFrontiers.LevelID.w2_1 },     // 2-1
            { "w8d03", SonicFrontiers.LevelID.w2_2 },     // 2-2
            { "w7d02", SonicFrontiers.LevelID.w2_3 },     // 2-3
            { "w7d06", SonicFrontiers.LevelID.w2_4 },    // 2-4
            { "w8d04", SonicFrontiers.LevelID.w2_5 },    // 2-5
            { "w6d03", SonicFrontiers.LevelID.w2_6 },    // 2-6
            { "w8d05", SonicFrontiers.LevelID.w2_7 },    // 2-7
            { "w6d04", SonicFrontiers.LevelID.w3_1 },    // 3-1
            { "w6d08", SonicFrontiers.LevelID.w3_2 },    // 3-2
            { "w8d02", SonicFrontiers.LevelID.w3_3 },    // 3-3
            { "w6d09", SonicFrontiers.LevelID.w3_4 },    // 3-4
            { "w6d07", SonicFrontiers.LevelID.w3_5 },    // 3-5
            { "w8d06", SonicFrontiers.LevelID.w3_6 },    // 3-6
            { "w7d03", SonicFrontiers.LevelID.w3_7 },    // 3-7
            { "w7d08", SonicFrontiers.LevelID.w4_1 },    // 4-1
            { "w9d02", SonicFrontiers.LevelID.w4_2 },    // 4-2
            { "w7d01", SonicFrontiers.LevelID.w4_3 },    // 4-3
            { "w9d03", SonicFrontiers.LevelID.w4_4 },    // 4-4
            { "w6d10", SonicFrontiers.LevelID.w4_5 },    // 4-5
            { "w7d07", SonicFrontiers.LevelID.w4_6 },    // 4-6
            { "w9d05", SonicFrontiers.LevelID.w4_7 },    // 4-7
            { "w7d05", SonicFrontiers.LevelID.w4_8 },    // 4-8
            { "w9d07", SonicFrontiers.LevelID.w4_9 },    // 4-9
            { "w1r03", SonicFrontiers.LevelID.Island_Kronos },    // Kronos Island
            { "w2r01", SonicFrontiers.LevelID.Island_Ares },    // Ares Island
            { "w3r01", SonicFrontiers.LevelID.Island_Chaos },    // Chaos Island
            { "w1r05", SonicFrontiers.LevelID.Island_Rhea },    // Rhea Island
            { "w1r04", SonicFrontiers.LevelID.Island_Ouranos },    // Ouranos Island
            { "w1f01", SonicFrontiers.LevelID.Fishing },    // Fishing
            { "w0r01", SonicFrontiers.LevelID.MainMenu },    // Main Menu
            { "w5r01", SonicFrontiers.LevelID.Boss_TheEnd },    // The End (boss)
            { "w5t01", SonicFrontiers.LevelID.Tutorial },    // Tutorial stage
            { "w1b01", SonicFrontiers.LevelID.Island_Kronos_BossRush },
            { "w2b01", SonicFrontiers.LevelID.Island_Ares_BossRush },
            { "w3b01", SonicFrontiers.LevelID.Island_Chaos_BossRush },
            { "w1b02", SonicFrontiers.LevelID.Island_Ouranos_BossRush },
        };

        public static readonly Dictionary<string, Status> Status = new Dictionary<string, Status>
        {
            { "TopMenu", SonicFrontiers.Status.TopMenu },                   // Main menu screen
            { "NewGameMenu",SonicFrontiers.Status.NewGameMenu },            // New game
            { "ContinueMenu", SonicFrontiers.Status.ContinueMenu },         // Continue screen - save selection
            { "ArcadeMode", SonicFrontiers.Status.ArcadeMode },             // Arcade mode menu
            { "BattleMode", SonicFrontiers.Status.BattleMode },               // Cyberspace Challenge
            { "CyberMode", SonicFrontiers.Status.CyberMode },               // Cyberspace Challenge
            { "SelectLanguage", SonicFrontiers.Status.SelectLanguage },     // Select Language
            { "ChangingLanguage", SonicFrontiers.Status.ChangingLanguage },
            { "ViewLicense", SonicFrontiers.Status.ViewLicense },           // Copyright menu
            { "ViewQRCode", SonicFrontiers.Status.ViewQRCode },             // User's manual
            { "ExtraContents", SonicFrontiers.Status.ExtraContents },       // DLC menu
            { "CheckQuit", SonicFrontiers.Status.CheckQuit },               // Asking for confirmation before quitting the game
            { "Quit", SonicFrontiers.Status.Quit },                         // Quitting the main menu
            { "PlayTop", SonicFrontiers.Status.PlayTop },                   // Normal gameplay
            { "Finish", SonicFrontiers.Status.Finish },                     // Reached the end of a level
            { "Result", SonicFrontiers.Status.Result },                     // Results screen
            { "StageResult", SonicFrontiers.Status.StageResult },           // Results at the end of boss rush
            { "Build", SonicFrontiers.Status.Build },                       // Used while "building" the level, regardless of the actual load. Don't use it
        };
    }

    // Technically not needed, but this enum keeps track of the currently released patches
    // so it makes easier to deal with version changes, should a patch come that breaks
    // compatibility with the current autosplitter's functions.
    enum GameVersion
    {
        Unknown,
        v1_01 = 0x162C8000,
        v1_10 = 0x1661B000,
        v1_20 = 0x1622F000, // Speed update
    }
}