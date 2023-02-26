using LiveSplit.UI.Components;
using System;
using System.Linq;

namespace LiveSplit.SonicFrontiers
{
    partial class SonicFrontiersComponent : LogicComponent
    {
        private bool Start()
        {
            if (watchers.Status.Current == Status.Quit && watchers.Status.Old == Status.NewGameMenu) // Main trigger for story mode
                return Settings.StoryStart;
            else if (Settings.ArcadeStart && watchers.IsInArcade)
                return (Settings.Arcade1_1 ? watchers.LevelID.Current == LevelID.w1_1 : watchers.LevelID.Current != watchers.LevelID.Old) && watchers.LevelID.Old == LevelID.MainMenu;

            return false;
        }

        private bool Split()
        {
            // Arcade mode splitting
            if (watchers.IsInArcade)
            {
                // First, the code checks if autosplitting for the current stage is enabled.
                // If it's not, there's no point in continuing.
                if (!CheckArcadeSplit(watchers.LevelID.Old))
                    return false;

                // In 4-9 there's a specific setting to allow splitting when touching the goal.
                // As this is usually the final split, this is usually a desirable thing to have.
                if (watchers.LevelID.Old == LevelID.w4_9 && Settings.w4_9_arcade_soon)
                    return watchers.Status.Changed && watchers.Status.Current == Status.Finish;

                return watchers.Status.Changed && watchers.Status.Old == Status.Result;
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
            if (Settings.FinalBoss && watchers.LevelID.Current == LevelID.Boss_TheEnd && watchers.EndQTECount.Old == 3 && watchers.EndQTECount.Current == 0)
                return true;
            
            // Cyber space levels (in story mode)
            return CheckStorySplit(watchers.LevelID.Old) && watchers.StoryModeCyberSpaceCompletionFlag.Old && !watchers.StoryModeCyberSpaceCompletionFlag.Current;
        }

        bool Reset()
        {
            return false;
        }

        bool IsLoading()
        {
            return watchers.IsInArcade || (watchers.LevelID.Current != LevelID.MainMenu && watchers.GameModeLoad)
                || (watchers.IsInTutorial && (watchers.LevelID.Current <= LevelID.w4_9 || watchers.LevelID.Current == LevelID.Fishing));
        }

        private TimeSpan? GameTime()
        {
            if (watchers.IsInArcade)
                return watchers.IGT.Current + watchers.AccumulatedIGT;
            else return null;
        }

        private bool CheckArcadeSplit(LevelID input) => input switch
        {
            LevelID.w1_1 => Settings.w1_1_arcade,
            LevelID.w1_2 => Settings.w1_2_arcade,
            LevelID.w1_3 => Settings.w1_3_arcade,
            LevelID.w1_4 => Settings.w1_4_arcade,
            LevelID.w1_5 => Settings.w1_5_arcade,
            LevelID.w1_6 => Settings.w1_6_arcade,
            LevelID.w1_7 => Settings.w1_7_arcade,
            LevelID.w2_1 => Settings.w2_1_arcade,
            LevelID.w2_2 => Settings.w2_2_arcade,
            LevelID.w2_3 => Settings.w2_3_arcade,
            LevelID.w2_4 => Settings.w2_4_arcade,
            LevelID.w2_5 => Settings.w2_5_arcade,
            LevelID.w2_6 => Settings.w2_6_arcade,
            LevelID.w2_7 => Settings.w2_7_arcade,
            LevelID.w3_1 => Settings.w3_1_arcade,
            LevelID.w3_2 => Settings.w3_2_arcade,
            LevelID.w3_3 => Settings.w3_3_arcade,
            LevelID.w3_4 => Settings.w3_4_arcade,
            LevelID.w3_5 => Settings.w3_5_arcade,
            LevelID.w3_6 => Settings.w3_6_arcade,
            LevelID.w3_7 => Settings.w3_7_arcade,
            LevelID.w4_1 => Settings.w4_1_arcade,
            LevelID.w4_2 => Settings.w4_2_arcade,
            LevelID.w4_3 => Settings.w4_3_arcade,
            LevelID.w4_4 => Settings.w4_4_arcade,
            LevelID.w4_5 => Settings.w4_5_arcade,
            LevelID.w4_6 => Settings.w4_6_arcade,
            LevelID.w4_7 => Settings.w4_7_arcade,
            LevelID.w4_8 => Settings.w4_8_arcade,
            LevelID.w4_9 => Settings.w4_9_arcade,
            _ => false,
        };

        private bool CheckStorySplit(LevelID input) => input switch
        {
            LevelID.w1_1 => Settings.w1_1_story,
            LevelID.w1_2 => Settings.w1_2_story,
            LevelID.w1_3 => Settings.w1_3_story,
            LevelID.w1_4 => Settings.w1_4_story,
            LevelID.w1_5 => Settings.w1_5_story,
            LevelID.w1_6 => Settings.w1_6_story,
            LevelID.w1_7 => Settings.w1_7_story,
            LevelID.w2_1 => Settings.w2_1_story,
            LevelID.w2_2 => Settings.w2_2_story,
            LevelID.w2_3 => Settings.w2_3_story,
            LevelID.w2_4 => Settings.w2_4_story,
            LevelID.w2_5 => Settings.w2_5_story,
            LevelID.w2_6 => Settings.w2_6_story,
            LevelID.w2_7 => Settings.w2_7_story,
            LevelID.w3_1 => Settings.w3_1_story,
            LevelID.w3_2 => Settings.w3_2_story,
            LevelID.w3_3 => Settings.w3_3_story,
            LevelID.w3_4 => Settings.w3_4_story,
            LevelID.w3_5 => Settings.w3_5_story,
            LevelID.w3_6 => Settings.w3_6_story,
            LevelID.w3_7 => Settings.w3_7_story,
            LevelID.w4_1 => Settings.w4_1_story,
            LevelID.w4_2 => Settings.w4_2_story,
            LevelID.w4_3 => Settings.w4_3_story,
            LevelID.w4_4 => Settings.w4_4_story,
            LevelID.w4_5 => Settings.w4_5_story,
            LevelID.w4_6 => Settings.w4_6_story,
            LevelID.w4_7 => Settings.w4_7_story,
            LevelID.w4_8 => Settings.w4_8_story,
            LevelID.w4_9 => Settings.w4_9_story,
            _ => false,
        };
    }
}