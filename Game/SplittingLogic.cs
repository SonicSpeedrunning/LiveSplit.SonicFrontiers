using LiveSplit.UI.Components;
using System;

namespace LiveSplit.SonicFrontiers
{
    partial class SonicFrontiersComponent : LogicComponent
    {
        private bool Start()
        {
            if (watchers.Status.Current == Status.Quit && watchers.Status.Old == Status.NewGameMenu) // Main trigger for story mode
                return Settings.StoryStart;
            else if (Settings.ArcadeStart && watchers.IsInArcade)
                return (Settings.Arcade1_1 ? watchers.LevelID.Current == CyberSpaceLevels.w1_1 : watchers.LevelID.Current != watchers.LevelID.Old) && watchers.LevelID.Old == SpecialLevels.MainMenu;

            return false;
        }

        private bool Split()
        {
            // Arcade mode splitting
            if (watchers.IsInArcade)
            {
                if (watchers.LevelID.Old == CyberSpaceLevels.w4_9)
                {
                    if (!Settings.w9d07_arcade)
                        return false;
                    if (Settings.w9d07_arcade_soon)
                        return watchers.Status.Old != watchers.Status.Current && watchers.Status.Current == Status.Finish;
                    else
                        return watchers.Status.Old == Status.Result && watchers.Status.Current != Status.Result;
                }
                else
                {
                    return Settings.GetSetting(watchers.LevelID.Old + "_arcade") && watchers.Status.Old == Status.Result && watchers.Status.Current != Status.Result;
                }
            }

            // Story mode splitting - island switching
            if (watchers.SwitchedIsland())
            {
                switch (watchers.LevelID.Old)
                {
                    case Islands.Kronos:
                        if (!Settings.Kronos) return false;
                        if (watchers.LevelID.Current != Islands.Ares) return !Settings.KronosFirst;

                        if (!watchers.VisitedIslands.Contains(watchers.LevelID.Old))
                        {
                            watchers.VisitedIslands.Add(watchers.LevelID.Old);
                            return true;
                        }
                        else
                            return !Settings.KronosFirst;

                    case Islands.Ares:
                        if (!Settings.Ares) return false;
                        if (watchers.LevelID.Current != Islands.Chaos) return !Settings.AresFirst;
                                
                        if (!watchers.VisitedIslands.Contains(watchers.LevelID.Old))
                        {
                            watchers.VisitedIslands.Add(watchers.LevelID.Old);
                            return true;
                        }
                        else
                            return !Settings.AresFirst;

                    case Islands.Chaos:
                        if (!Settings.Chaos) return false;
                        if (watchers.LevelID.Current != Islands.Rhea) return !Settings.ChaosFirst;

                        if (!watchers.VisitedIslands.Contains(watchers.LevelID.Old))
                        {
                            watchers.VisitedIslands.Add(watchers.LevelID.Old);
                            return true;
                        }
                        else
                            return !Settings.ChaosFirst;


                    case Islands.Rhea:
                        if (!Settings.Chaos) return false;
                        if (watchers.LevelID.Current != Islands.Ouranos) return !Settings.RheaFirst;

                        if (!watchers.VisitedIslands.Contains(watchers.LevelID.Old))
                        {
                            watchers.VisitedIslands.Add(watchers.LevelID.Old);
                            return true;
                        }
                        else
                            return !Settings.RheaFirst;
                }
            }

            // Fishing stages splitting
            if (watchers.LevelID.Old == FishingLevels.Fishing && watchers.LevelID.Changed)
            {
                switch (watchers.LevelID.Current)
                {
                    case Islands.Kronos:
                    case Islands.Ares:
                    case Islands.Chaos:
                    case Islands.Ouranos:
                        if (!Settings.Fishing) return false;
                        if (!watchers.VisitedFishingLevels.Contains(watchers.LevelID.Current))
                        {
                            watchers.VisitedFishingLevels.Add(watchers.LevelID.Current);
                            return Settings.GetSetting(watchers.LevelID.Current + "_fish");
                        }
                        else
                            return !Settings.GetSetting(watchers.LevelID.Current + "_fish_first");
                }
            }

            // Final boss split
            if (watchers.LevelID.Current == SpecialLevels.TheEndBoss && watchers.EndQTECount == 3)
                return Settings.FinalBoss;
            else
                return Settings.GetSetting(watchers.LevelID.Old + "_story") && watchers.StoryModeCyberSpaceCompletionFlag.Old && !watchers.StoryModeCyberSpaceCompletionFlag.Current;
        }

        bool Reset()
        {
            return false;
        }

        bool IsLoading()
        {
            return watchers.IsInArcade || (watchers.LevelID.Current != SpecialLevels.MainMenu && watchers.GameModeLoad)
                || (watchers.IsInTutorial && !watchers.LevelID.Current.Contains("r"));
        }

        private TimeSpan? GameTime()
        {
            if (watchers.IsInArcade)
                return watchers.IGT.Current + watchers.AccumulatedIGT;
            else return null;
        }
    }
}