namespace LiveSplit.SonicFrontiers
{
    static class CyberSpaceLevels
    {
        public const string
            w1_1 = "w6d01",
            w1_2 = "w8d01",
            w1_3 = "w9d04",
            w1_4 = "w6d02",
            w1_5 = "w7d04",
            w1_6 = "w6d06",
            w1_7 = "w9d06",
            w2_1 = "w6d05",
            w2_2 = "w8d03",
            w2_3 = "w7d02",
            w2_4 = "w7d06",
            w2_5 = "w8d04",
            w2_6 = "w6d03",
            w2_7 = "w8d05",
            w3_1 = "w6d04",
            w3_2 = "w6d08",
            w3_3 = "w8d02",
            w3_4 = "w6d09",
            w3_5 = "w6d07",
            w3_6 = "w8d06",
            w3_7 = "w7d03",
            w4_1 = "w7d08",
            w4_2 = "w9d02",
            w4_3 = "w7d01",
            w4_4 = "w9d03",
            w4_5 = "w6d10",
            w4_6 = "w7d07",
            w4_7 = "w9d05",
            w4_8 = "w7d05",
            w4_9 = "w9d07";
    }

    static class Islands
    {
        public const string
            Kronos = "w1r03",
            Ares = "w2r01",
            Chaos = "w3r01",
            Rhea = "w1r05",
            Ouranos = "w1r04";
    }

    static class SpecialLevels
    {
        public const string
            MainMenu = "w0r01",
            TheEndBoss = "w5r01";
    }

    static class Status
    {
        public const string
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
