using LiveSplit.UI.Components;
using System;
using System.Linq;

namespace LiveSplit.SonicFrontiers
{
    partial class SonicFrontiersComponent : LogicComponent
    {
        private bool Start()
        {
            if (watchers.Status.Current == FakeEnums.Status.Quit && watchers.Status.Old == FakeEnums.Status.NewGameMenu) // Main trigger for story mode
                return Settings.StoryStart;
            else if (Settings.ArcadeStart && watchers.IsInArcade)
                return (Settings.Arcade1_1 ? watchers.LevelID.Current == FakeEnums.LevelID["w6d01"] : watchers.LevelID.Current != watchers.LevelID.Old) && watchers.LevelID.Old == FakeEnums.LevelID["w0r01"];

            return false;
        }

        private bool Split()
        {
            // Arcade mode splitting
            if (watchers.IsInArcade)
            {
                // First, the code checks if autosplitting for the current stage is enabled.
                // If it's not, there's no point in continuing.
                if (!Settings["c" + watchers.LevelID.Old + "_arcade"])
                    return false;

                // In 4-9 there's a specific setting to allow splitting when touching the goal.
                // As this is usually the final split, this is usually a desirable thing to have.
                if (watchers.LevelID.Old == FakeEnums.LevelID["w9d07"] && Settings.c29_arcade_soon)
                    return watchers.Status.Changed && watchers.Status.Current == FakeEnums.Status.Finish;

                return watchers.Status.Changed && watchers.Status.Old == FakeEnums.Status.Result;
            }

            // Story mode

            // Event flags
            foreach (var flag in watchers.SplitBools.Where(b => !watchers.AlreadyTriggeredBools.Contains(b.Key)))
            {
                if (Settings[flag.Key] && !flag.Value.Old && flag.Value.Current)
                {
                    watchers.AlreadyTriggeredBools.Add(flag.Key);
                    return true;
                }
            }

            // Final boss split
            if (Settings.FinalBoss && watchers.LevelID.Current == 72 && watchers.EndQTECount.Old == 3 && watchers.EndQTECount.Current == 0)
                return true;
            
            // Cyber space levels (in story mode)
            return Settings["c" + watchers.LevelID.Old + "_story"] && watchers.StoryModeCyberSpaceCompletionFlag.Old && !watchers.StoryModeCyberSpaceCompletionFlag.Current;
        }

        bool Reset()
        {
            return false;
        }

        bool IsLoading()
        {
            return watchers.IsInArcade || (watchers.LevelID.Current != FakeEnums.LevelID["w0r01"] && watchers.GameModeLoad)
                || (watchers.IsInTutorial && (watchers.LevelID.Current <= 29 || watchers.LevelID.Current == 70));
        }

        private TimeSpan? GameTime()
        {
            if (watchers.IsInArcade)
                return watchers.IGT.Current + watchers.AccumulatedIGT;
            else return null;
        }
    }
}