using System;
using System.Collections.Generic;
using LiveSplit.ComponentUtil;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        // Game process
        private readonly string[] processNames = { "SonicFrontiers" };
        private IntPtr baseAddress;

        // Variables 
        public FakeMemoryWatcher<TimeSpan> IGT = new FakeMemoryWatcher<TimeSpan>(TimeSpan.Zero, TimeSpan.Zero);
        public FakeMemoryWatcher<string> Status = new FakeMemoryWatcher<string>(string.Empty, string.Empty);
        public FakeMemoryWatcher<string> LevelID = new FakeMemoryWatcher<string>(string.Empty, string.Empty);
        public FakeMemoryWatcher<bool> IsInQTE = new FakeMemoryWatcher<bool>(false, false);
        public TimeSpan AccumulatedIGT = TimeSpan.Zero;
        public bool IsInTutorial = false;
        public bool IsInArcade = false;
        public int EndQTECount = 0;
        public FakeMemoryWatcher<bool> StoryModeCyberSpaceCompletionFlag = new FakeMemoryWatcher<bool>(false, false);
        public List<string> VisitedIslands = new List<string>();

        // Internal state variables
        private int APPLICATION;
        private int APPLICATIONSEQUENCE;
        private int GAMEMODE;
        public int GameModeExtensionCount;
        private int GAMEMODEEXTENSION;

        // "Static" state variables - meaning they never change once they get set
        private long APPLICATIONSEQUENCE_addr;
        private long QTEADDRESS;
        private long StageTimeExtension;
        private long HsmExtension;

        public void Update()
        {
            if (!GotAddresses)
                try { GetAddresses(); } catch { GotAddresses = false; }

            if (GotAddresses)
            {
                // In the update sequence, first of all we need to retrieve the new dynamic offsets

                // Even though it never happened in my case, this offset can theoretically change dynamically
                // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
                APPLICATIONSEQUENCE = GetApplicationSequence();
                GameModeExtensionCount = GetGameModeExtensionCount();

                // Update the old watchers
                IGT.Old = IGT.Current;
                Status.Old = Status.Current;
                LevelID.Old = LevelID.Current;
                IsInQTE.Old = IsInQTE.Current;
                StoryModeCyberSpaceCompletionFlag.Old = StoryModeCyberSpaceCompletionFlag.Current;

                // Find and define the new values
                IGT.Current = GetIGT();
                Status.Current = GetStatus();
                LevelID.Current = GetCurrentStage();
                IsInQTE.Current = GetQTEInput() == QTEADDRESS;
                StoryModeCyberSpaceCompletionFlag.Current = GetStoryModeCyberSpaceCompletionFlag();

                var tutorialstage = GetTutorialStage();
                IsInTutorial = tutorialstage != null && tutorialstage.Contains("w") && tutorialstage.Contains("t");

                // Final split QTE stuff
                if (LevelID.Current == SpecialLevels.TheEndBoss)
                {
                    if (IsInQTE.Old && !IsInQTE.Current)
                        EndQTECount++;
                }
                else if (EndQTECount > 0)
                {
                    EndQTECount = 0;
                }
            }
        }

        // GetAddresses is essentially the equivalent of init in script-based autosplitters
        private void GetAddresses()
        {
            var scanner = new SignatureScanner(game, game.MainModuleWow64Safe().BaseAddress, game.MainModuleWow64Safe().ModuleMemorySize);
            
            baseAddress = scanner.Scan(new SigScanTarget(1, "E8 ???????? 4C 8B 78 70")
            {
                OnFound = (p, s, addr) =>
                {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x3;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });
            CheckPtr(baseAddress);

            IntPtr ptr;

            // APPLICATION
            ptr = scanner.Scan(new SigScanTarget(3, "48 8B 99 ???????? 48 8B F9 48 8B 81 ???????? 48 8D 34 C3 48 3B DE 74 21"));
            CheckPtr(ptr);
            APPLICATION = game.ReadValue<int>(ptr);

            // APPLICATIONSEQUENCE
            // Even though it never happened in my case, this offset can theoretically change dynamically
            // We need a little of black magic to cope with this. Thanks, Hedgehog Engine 2, for being so BS
            ptr = scanner.Scan(new SigScanTarget(7, "48 83 EC 20 48 8D 05 ???????? 48 89 51 08 48 89 01 48 89 CF 48 8D 05") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
            CheckPtr(ptr);
            APPLICATIONSEQUENCE_addr = (long)ptr;

            // GAMEMODE
            ptr = scanner.Scan(new SigScanTarget(1, "74 31 48 8D 55 E0") { OnFound = (p, s, addr) => addr + 0x31 + 0x4 });
            CheckPtr(ptr);
            GAMEMODE = game.ReadValue<byte>(ptr);

            // GAMEMODEEXTENSION
            ptr = scanner.Scan(new SigScanTarget(8, "E8 ???????? 48 8B BB ???????? 48 8B 83 ???????? 4C 8D 34 C7"));
            CheckPtr(ptr);
            GAMEMODEEXTENSION = game.ReadValue<int>(ptr);

            // StageTimeExtension class
            ptr = scanner.Scan(new SigScanTarget(1, "E9 ???????? 0F 86 15 8A 59 FF")
            {
                OnFound = (p, s, addr) => {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x7;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });
            CheckPtr(ptr);
            StageTimeExtension = (long)ptr;

            // HsmExtension - Hedgehog Scene Manager, maybe? Anyway, it's used to look for the status variable
            ptr = scanner.Scan(new SigScanTarget(1, "E8 ???????? 48 8B F8 48 8D 55 C0")
            {
                OnFound = (p, s, addr) => {
                    var tempAddr = addr + p.ReadValue<int>(addr) + 0x4 + 0x9;
                    tempAddr += p.ReadValue<int>(tempAddr) + 0x4;
                    return tempAddr;
                }
            });
            CheckPtr(ptr);
            HsmExtension = (long)ptr;

            // QTEADDRESS
            ptr = scanner.Scan(new SigScanTarget(13, "C7 83 ???????? ???????? 48 8D 05 ???????? 48 89 BB") { OnFound = (p, s, addr) => addr + p.ReadValue<int>(addr) + 0x4 });
            CheckPtr(ptr);
            QTEADDRESS = (long)ptr;

            // End
            GotAddresses = true;
        }

        private int GetApplicationSequence()
        {
            int value = new DeepPointer(baseAddress, APPLICATION + 0x8).Deref<byte>(game);
            
            if (value == 0)
                return 0;
            
            for (int i = 0; i < value; i++)
            {
                var g = new DeepPointer(baseAddress, APPLICATION, 0x8 * i, 0x0).Deref<long>(game);
                if (g == APPLICATIONSEQUENCE_addr)
                    return 0x8 * i;
            }
            
            return 0;
        }

        private int GetGameModeExtensionCount()
        {
            // Yes, I know we're returning as a byte instead of an int.
            // This is intended so if the value is too high LiveSplit won't be stuck in an (almost) endless loop.
            return new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION + 0x8).Deref<byte>(game);
        }

        private string GetStatus()
        {
            if (GameModeExtensionCount == 0)
                return string.Empty;

            for (int i = 0; i < GameModeExtensionCount; i++)
            {
                var q = new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x0).Deref<long>(game);

                if (q == HsmExtension)
                    return new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x60, 0x20, 0x0).DerefString(game, 255);
            }

            return string.Empty;
        }

        private TimeSpan GetIGT()
        {
            if (GameModeExtensionCount == 0)
                return TimeSpan.Zero;
            
            for (int i = 0; i < GameModeExtensionCount; i++)
            {
                var q = new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x0).Deref<long>(game);
                if (q == StageTimeExtension)
                    return TimeSpan.FromSeconds(Math.Truncate(new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, GAMEMODEEXTENSION, 0x8 * i, 0x28).Deref<float>(game) * 100) / 100);
            }

            return TimeSpan.Zero;
        }

        private string GetCurrentStage()
        {
            return new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, 0xA0).DerefString(game, 5);
        }

        private string GetTutorialStage()
        {
            return new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, GAMEMODE, 0xF8).DerefString(game, 5);
        }

        internal bool GetArcadeFlag()
        {
            byte i = new DeepPointer(baseAddress, APPLICATION, APPLICATIONSEQUENCE, 0x122).Deref<byte>(game);
            return (i & 1) != 0;
        }

        private long GetQTEInput()
        {
            return new DeepPointer(baseAddress, 0x70, 0xD0, 0x28, 0x0, 0x0).Deref<long>(game);
        }


        public bool SwitchedIsland()
        {
            if (LevelID.Old == null || LevelID.Current == null || LevelID.Old == LevelID.Current || LevelID.Old == SpecialLevels.MainMenu || LevelID.Current == SpecialLevels.MainMenu)
                return false;

            return LevelID.Old.Contains("w") && LevelID.Old.Contains("r0") && LevelID.Current.Contains("w") && LevelID.Current.Contains("r0");
        }

        public bool GetStoryModeCyberSpaceCompletionFlag()
        {
            // Workaround for story mode stage end stuff
            // Avoid splitting when restarting a stage in story mode
            if (IsInArcade || (LevelID.Current.Contains("w") && LevelID.Current.Contains("r0")))
                return false;

            if (Status.Current == SonicFrontiers.Status.Result)
                return true;
            else
                return StoryModeCyberSpaceCompletionFlag.Old;
        }
    }
}