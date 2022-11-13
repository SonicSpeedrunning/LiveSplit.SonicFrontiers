using System;

namespace LiveSplit.SonicFrontiers
{
    partial class SonicFrontiersComponent
    {
        private Watchers watchers;

        public void SplittingLogicUpdate()
        {
            if (!VerifyOrHookGameProcess())
                return;

            watchers.Update();
            UUpdate();
            timer.CurrentState.IsGameTimePaused = IsLoading();

            switch (timer.CurrentState.CurrentPhase)
            {
                case Model.TimerPhase.NotRunning:
                    if (Start()) timer.Start();
                    break;
                case Model.TimerPhase.Running:
                case Model.TimerPhase.Paused:
                    if (Reset()) timer.Reset();
                    else if (Split()) timer.Split();
                    break;
            }
        }

        private void UUpdate()
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
                if (Settings.Arcade1_1)
                    return watchers.LevelID.Current == CyberSpaceLevels.w1_1 && watchers.LevelID.Old == SpecialLevels.MainMenu;
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
                                if (watchers.VisitedIslands.Contains(Islands.Kronos))
                                    return Settings.Kronos && !Settings.KronosFirst;
                                else
                                {
                                    watchers.VisitedIslands.Add(Islands.Kronos);
                                    return Settings.Kronos;
                                }
                            case Islands.Ares:
                                if (watchers.VisitedIslands.Contains(Islands.Ares))
                                    return Settings.Ares && !Settings.AresFirst;
                                else
                                {
                                    watchers.VisitedIslands.Add(Islands.Ares);
                                    return Settings.Ares;
                                }
                            case Islands.Chaos:
                                if (watchers.VisitedIslands.Contains(Islands.Chaos))
                                    return Settings.Chaos && !Settings.ChaosFirst;
                                else
                                {
                                    watchers.VisitedIslands.Add(Islands.Chaos);
                                    return Settings.Chaos;
                                }
                            case Islands.Rhea:
                                if (watchers.VisitedIslands.Contains(Islands.Rhea))
                                    return Settings.Rhea && !Settings.RheaFirst;
                                else
                                {
                                    watchers.VisitedIslands.Add(Islands.Rhea);
                                    return Settings.Rhea;
                                }
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

        bool VerifyOrHookGameProcess()
        {
            try
            {
                if (watchers == null)
                    watchers = new Watchers();
                return watchers.IsGameHooked;
            }
            catch
            {
                return false;
            }
        }
    }
}
