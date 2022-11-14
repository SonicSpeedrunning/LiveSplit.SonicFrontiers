using System;

namespace LiveSplit.SonicFrontiers
{
    partial class SonicFrontiersComponent
    {
        private void LogicUpdate()
        {
            // If the timer is not running (eg. a run has been reset) these variables need to be reset
            if (timer.CurrentState.CurrentPhase == Model.TimerPhase.NotRunning)
            {
                if (watchers.AccumulatedIGT != TimeSpan.Zero) watchers.AccumulatedIGT = TimeSpan.Zero;
                if (watchers.VisitedIslands.Count > 0) watchers.VisitedIslands.Clear();
                watchers.IsInArcade = watchers.GetArcadeFlag() || watchers.Status.Current == Status.ArcadeMode;
            }

            if (watchers.IGT.Current == TimeSpan.Zero && watchers.IGT.Old != TimeSpan.Zero)
                watchers.AccumulatedIGT += watchers.IGT.Old;

            if (timer.CurrentState.CurrentPhase == Model.TimerPhase.Running && watchers.IsInArcade)
                timer.CurrentState.SetGameTime(watchers.IGT.Current + watchers.AccumulatedIGT);
        }

        private bool Start()
        {
            if (watchers.Status.Current == Status.Quit && watchers.Status.Old == Status.NewGameMenu) // Main trigger for story mode
            {
                return Settings.StoryStart;
            }
            else if (Settings.ArcadeStart && watchers.IsInArcade)
            {
                return (Settings.Arcade1_1 ? watchers.LevelID.Current == CyberSpaceLevels.w1_1 : watchers.LevelID.Current != watchers.LevelID.Old) && watchers.LevelID.Old == SpecialLevels.MainMenu;
            }

            return false;
        }

        private bool Split()
        {
            switch (watchers.IsInArcade)
            {
                case true:
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
                case false:
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

                    if (watchers.LevelID.Current == SpecialLevels.TheEndBoss && watchers.EndQTECount == 3)
                    {
                        watchers.EndQTECount = 0;
                        return Settings.FinalBoss;
                    }
                    else
                    {
                        return Settings.GetSetting(watchers.LevelID.Old + "_story") && watchers.StoryModeCyberSpaceCompletionFlag.Old && !watchers.StoryModeCyberSpaceCompletionFlag.Current;
                    }
            }
            return false;
        }

        bool Reset()
        {
            return Settings.Reset && watchers.Status.Current == Status.TopMenu && watchers.Status.Old != watchers.Status.Current;
        }

        bool IsLoading()
        {
            return watchers.IsInArcade || (watchers.LevelID.Current != SpecialLevels.MainMenu && watchers.GameModeExtensionCount == 0)
                || (watchers.IsInTutorial && !watchers.LevelID.Current.Contains("r"));
        }
    }
}
