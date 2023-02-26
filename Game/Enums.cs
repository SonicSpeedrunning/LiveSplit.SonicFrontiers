using System.Collections.Generic;

namespace LiveSplit.SonicFrontiers
{
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
    }

    enum Status
    {
        TopMenu,
        NewGameMenu,
        ContinueMenu,
        ArcadeMode,
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
        Default, // Used as default
    }

    enum QTEResolveStatus : byte
    {
        NotCompleted = 0,
        Completed = 1,
        Failed = 2
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
        };

        public static readonly Dictionary<string, Status> Status = new Dictionary<string, Status>
        {
            { "TopMenu", SonicFrontiers.Status.TopMenu },                   // Main menu screen
            { "NewGameMenu",SonicFrontiers.Status.NewGameMenu },            // New game
            { "ContinueMenu", SonicFrontiers.Status.ContinueMenu },         // Continue screen - save selection
            { "ArcadeMode", SonicFrontiers.Status.ArcadeMode },             // Arcade mode menu
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
    }
}