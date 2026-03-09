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
        w4_A,
        w4_B,
        w4_C,
        w4_D,
        w4_E,
        w4_F,
        w4_G,
        w4_H,
        w4_I,
        Island_Kronos,
        Island_Ares,
        Island_Chaos,
        Island_Rhea,
        Island_Ouranos,
        Island_Another_Ouranos,
        Fishing,
        MainMenu,
        Boss_TheEnd,
        Tutorial,
        None,
        Island_Kronos_BossRush,
        Island_Ares_BossRush,
        Island_Chaos_BossRush,
        Island_Ouranos_BossRush,
        Hacking_01,
        Hacking_02,
        Hacking_03,
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
        v1_01,
        v1_10,
        v1_20, // Speed update
        v1_30, // Sonic's birthday update
        v1_42,
    }
}