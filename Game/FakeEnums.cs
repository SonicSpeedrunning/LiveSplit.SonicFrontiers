using System.Collections.Generic;

namespace LiveSplit.SonicFrontiers
{
    readonly struct FakeEnums
    {
        public static readonly Dictionary<string, byte> LevelID = new Dictionary<string, byte>
        {
            { "w6d01", 0 },     // 1-1
            { "w8d01", 1 },     // 1-2
            { "w9d04", 2 },     // 1-3
            { "w6d02", 3 },     // 1-4
            { "w7d04", 4 },     // 1-5
            { "w6d06", 5 },     // 1-6
            { "w9d06", 6 },     // 1-7
            { "w6d05", 7 },     // 2-1
            { "w8d03", 8 },     // 2-2
            { "w7d02", 9 },     // 2-3
            { "w7d06", 10 },    // 2-4
            { "w8d04", 11 },    // 2-5
            { "w6d03", 12 },    // 2-6
            { "w8d05", 13 },    // 2-7
            { "w6d04", 14 },    // 3-1
            { "w6d08", 15 },    // 3-2
            { "w8d02", 16 },    // 3-3
            { "w6d09", 17 },    // 3-4
            { "w6d07", 18 },    // 3-5
            { "w8d06", 19 },    // 3-6
            { "w7d03", 20 },    // 3-7
            { "w7d08", 21 },    // 4-1
            { "w9d02", 22 },    // 4-2
            { "w7d01", 23 },    // 4-3
            { "w9d03", 24 },    // 4-4
            { "w6d10", 25 },    // 4-5
            { "w7d07", 26 },    // 4-6
            { "w9d05", 27 },    // 4-7
            { "w7d05", 28 },    // 4-8
            { "w9d07", 29 },    // 4-9

            { "w1r03", 50 },    // Kronos Island
            { "w2r01", 51 },    // Ares Island
            { "w3r01", 52 },    // Chaos Island
            { "w1r05", 53 },    // Rhea Island
            { "w1r04", 54 },    // Ouranos Island

            { "w1f01", 70 },    // Fishing
            { "w0r01", 71 },    // Main Menu
            { "w5r01", 72 },    // The End (boss)
            { "w5t01", 73 },    // Tutorial stage
        };

        public readonly struct Status
        {
            public static readonly string
                TopMenu = "TopMenu",                    // Main menu screen
                NewGameMenu = "NewGameMenu",            // New game
                ContinueMenu = "ContinueMenu",          // Continue screen - save selection
                ArcadeMode = "ArcadeMode",              // Arcade mode menu
                SelectLanguage = "SelectLanguage",      // Select Language
                ChangingLanguage = "ChangingLanguage",
                ViewLicense = "ViewLicense",            // Copyright menu
                ViewQRCode = "ViewQRCode",              // User's manual
                ExtraContents = "ExtraContents",        // DLC menu
                CheckQuit = "CheckQuit",                // Asking for confirmation before quitting the game
                Quit = "Quit",                          // Quitting the main menu
                PlayTop = "PlayTop",                    // Normal gameplay
                Finish = "Finish",                      // Reached the end of a level
                Result = "Result",                      // Results screen
                Build = "Build";                        // Used while "building" the level, regardless of the actual load. Don't use it
        }

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