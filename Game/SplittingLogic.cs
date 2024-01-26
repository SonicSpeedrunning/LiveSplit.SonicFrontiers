using LiveSplit.UI.Components;
using System;
using System.Linq;
using LiveSplit.Options;

namespace LiveSplit.SonicFrontiers
{
    partial class SonicFrontiersComponent : LogicComponent
    {
        private bool Start()
        {
            if (watchers.Status.Current == Status.Quit && watchers.Status.Old == Status.NewGameMenu) // Main trigger for story mode
                return Settings.StoryStart;
            else if (Settings.ArcadeStart && watchers.CurrentGameMode == GameMode.CyberspaceChallenge) //removed Aracde check since it's not a category anymore
                return (watchers.LevelID.Current != watchers.LevelID.Old) && watchers.LevelID.Old == LevelID.MainMenu;
            else if (Settings.BossRushStart && watchers.CurrentGameMode == GameMode.BossRush)
                return watchers.LevelID.Old == LevelID.MainMenu && (watchers.LevelID.Current == LevelID.Island_Kronos_BossRush || watchers.LevelID.Current == LevelID.Island_Ares_BossRush || watchers.LevelID.Current == LevelID.Island_Chaos_BossRush || watchers.LevelID.Current == LevelID.Island_Ouranos_BossRush);
            else if (watchers.LevelID.Current == LevelID.Island_Another_Ouranos &&
                     watchers.LevelID.Old == LevelID.Island_Ouranos)
                return true; // make this a setting
            return false;
        }

        private bool Split()
        {
            // Arcade - Cyberspace mode splitting
            if (watchers.CurrentGameMode == GameMode.Arcade || watchers.CurrentGameMode == GameMode.CyberspaceChallenge)
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

            // Boss Rush
            if (watchers.CurrentGameMode == GameMode.BossRush)
            {
                return watchers.BossRushAct.Old switch
                {
                    BossRushAct.b1_1 => Settings.Boss1_1 && watchers.BossRushAct.Current == BossRushAct.b1_2,
                    BossRushAct.b1_2 => Settings.Boss1_2 && watchers.BossRushAct.Current == BossRushAct.b1_3,
                    BossRushAct.b1_3 => Settings.Boss1_3 && watchers.BossRushAct.Current == BossRushAct.b1_4,
                    BossRushAct.b1_4 => Settings.Boss1_4 && watchers.BossRushAct.Current == BossRushAct.b1_5,
                    BossRushAct.b1_5 => Settings.Boss1_5 && watchers.BossRushAct.Current == BossRushAct.b1_6,
                    BossRushAct.b1_6 => Settings.Boss1_6 && watchers.BossRushAct.Current == BossRushAct.b1_7,
                    BossRushAct.b1_7 => Settings.Boss1_7 && watchers.BossRushAct.Current == BossRushAct.b1_8,
                    BossRushAct.b1_8 => Settings.Boss1_8 && watchers.BossRushAct.Current == BossRushAct.b1_9,
                    BossRushAct.b1_9 => Settings.Boss1_9 && watchers.BossRushAct.Current == BossRushAct.b1_10,
                    BossRushAct.b1_10 => Settings.Boss1_10 && ((watchers.IGT.Old > watchers.IGT.Current && watchers.IGT.Current == TimeSpan.Zero)), // || (watchers.Status.Changed && watchers.Status.Current == Status.StageResult)),
                    BossRushAct.b2_1 => Settings.Boss2_1 && watchers.BossRushAct.Current == BossRushAct.b2_2,
                    BossRushAct.b2_2 => Settings.Boss2_2 && watchers.BossRushAct.Current == BossRushAct.b2_3,
                    BossRushAct.b2_3 => Settings.Boss2_3 && watchers.BossRushAct.Current == BossRushAct.b2_4,
                    BossRushAct.b2_4 => Settings.Boss2_4 && watchers.BossRushAct.Current == BossRushAct.b2_5,
                    BossRushAct.b2_5 => Settings.Boss2_5 && watchers.BossRushAct.Current == BossRushAct.b2_6,
                    BossRushAct.b2_6 => Settings.Boss2_6 && watchers.BossRushAct.Current == BossRushAct.b2_7,
                    BossRushAct.b2_7 => Settings.Boss2_7 && watchers.BossRushAct.Current == BossRushAct.b2_8,
                    BossRushAct.b2_8 => Settings.Boss2_8 && watchers.BossRushAct.Current == BossRushAct.b2_9,
                    BossRushAct.b2_9 => Settings.Boss2_9 && watchers.BossRushAct.Current == BossRushAct.b2_10,
                    BossRushAct.b2_10 => Settings.Boss2_10 && (watchers.IGT.Old > watchers.IGT.Current && watchers.IGT.Current == TimeSpan.Zero), // || (watchers.Status.Changed && watchers.Status.Current == Status.StageResult)),
                    BossRushAct.b3_1 => Settings.Boss3_1 && watchers.BossRushAct.Current == BossRushAct.b3_2,
                    BossRushAct.b3_2 => Settings.Boss3_2 && watchers.BossRushAct.Current == BossRushAct.b3_3,
                    BossRushAct.b3_3 => Settings.Boss3_3 && watchers.BossRushAct.Current == BossRushAct.b3_4,
                    BossRushAct.b3_4 => Settings.Boss3_4 && watchers.BossRushAct.Current == BossRushAct.b3_5,
                    BossRushAct.b3_5 => Settings.Boss3_5 && watchers.BossRushAct.Current == BossRushAct.b3_6,
                    BossRushAct.b3_6 => Settings.Boss3_6 && watchers.BossRushAct.Current == BossRushAct.b3_7,
                    BossRushAct.b3_7 => Settings.Boss3_7 && watchers.BossRushAct.Current == BossRushAct.b3_8,
                    BossRushAct.b3_8 => Settings.Boss3_8 && watchers.BossRushAct.Current == BossRushAct.b3_9,
                    BossRushAct.b3_9 => Settings.Boss3_9 && (watchers.IGT.Old > watchers.IGT.Current && watchers.IGT.Current == TimeSpan.Zero), // || (watchers.Status.Changed && watchers.Status.Current == Status.StageResult)),
                    BossRushAct.b4_1 => Settings.Boss4_1 && watchers.BossRushAct.Current == BossRushAct.b4_2,
                    BossRushAct.b4_2 => Settings.Boss4_2 && watchers.BossRushAct.Current == BossRushAct.b4_3,
                    BossRushAct.b4_3 => Settings.Boss4_3 && watchers.BossRushAct.Current == BossRushAct.b4_4,
                    BossRushAct.b4_4 => Settings.Boss4_4 && watchers.BossRushAct.Current == BossRushAct.b4_5,
                    BossRushAct.b4_5 => Settings.Boss4_5 && watchers.BossRushAct.Current == BossRushAct.b4_6,
                    BossRushAct.b4_6 => Settings.Boss4_6 && watchers.BossRushAct.Current == BossRushAct.b4_7,
                    BossRushAct.b4_7 => Settings.Boss4_7 && watchers.BossRushAct.Current == BossRushAct.b4_8,
                    BossRushAct.b4_8 => Settings.Boss4_8 && watchers.BossRushAct.Current == BossRushAct.b4_9,
                    BossRushAct.b4_9 => Settings.Boss4_9 && watchers.BossRushAct.Current == BossRushAct.b4_10,
                    BossRushAct.b4_10 => Settings.Boss4_10 && watchers.BossRushAct.Current == BossRushAct.b4_11,
                    BossRushAct.b4_11 => Settings.Boss4_11 && watchers.Status.Changed && watchers.Status.Current == Status.StageResult,
                    _ => false
                };
            }                                                               

            // Story mode
            // Doublesplit on start prevention
            if (timer.CurrentState.CurrentTime.RealTime > new TimeSpan(0, 0, 2))
            {
                // Event flags
                foreach (var flag in watchers.SplitBools.Where(b => !watchers.AlreadyTriggeredBools.Contains(b.Key)))
                {
                    if (CheckBoolSplit(flag.Key) && !flag.Value.Old && flag.Value.Current)
                    {
                        watchers.AlreadyTriggeredBools.Add(flag.Key);
                        return true;
                    }
                }
            }

            // Music Notes (any)
            if (Settings.MusicNoteAny && watchers.MusicNotes.Old != null && !watchers.MusicNotes.Old.SequenceEqual(watchers.MusicNotes.Current))
            {
                return true;
            } 
            if (watchers.MusicNotes.Old == null)
            {
                Log.Warning("watchers musicnotes old is null");
            }
            
            // Final boss split
            if (Settings.FinalBoss && watchers.LevelID.Current == LevelID.Boss_TheEnd && watchers.EndQTECount.Old == 3 && watchers.EndQTECount.Current == 0)
                return true;
            
            //Another Final Boss Split
            if (watchers.LevelID.Current == LevelID.Island_Another_Ouranos && watchers.AnotherQTECount.Old == 2 &&
                watchers.EndQTECount.Current == 0)
            {
                return true;    
            }
            
            // Cyber space levels (in story mode)
            return CheckStorySplit(watchers.LevelID.Old) && watchers.StoryModeCyberSpaceCompletionFlag.Old && !watchers.StoryModeCyberSpaceCompletionFlag.Current;
        }

        bool Reset()
        {
            return false;
        }

        bool IsLoading()
        {
            return watchers.CurrentGameMode == GameMode.Arcade || watchers.CurrentGameMode == GameMode.CyberspaceChallenge || watchers.CurrentGameMode == GameMode.BossRush
                || (watchers.LevelID.Current != LevelID.MainMenu && watchers.GameModeLoad)
                || (watchers.IsInTutorial && (watchers.LevelID.Current <= LevelID.w4_9 || watchers.LevelID.Current == LevelID.Fishing)) 
                || (watchers.LevelID.Current != LevelID.MainMenu && (watchers.GameVersion == GameVersion.Unknown || watchers.GameVersion == GameVersion.v1_10) && watchers.Status.Current == Status.Finish);
        }

        private TimeSpan? GameTime()
        {
            if (watchers.CurrentGameMode == GameMode.Arcade || watchers.CurrentGameMode == GameMode.CyberspaceChallenge || watchers.CurrentGameMode == GameMode.BossRush)
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

        private bool CheckBoolSplit(string key) => key switch
        {
            "Amy_First" => Settings.Amy_First,
            "Knuckles_First" => Settings.Knuckles_First,
            "Tails_First" => Settings.Tails_First,
            "Amy_Second" => Settings.Amy_Second,
            "Knuckles_Second" => Settings.Knuckles_Second,
            "Tails_Second" => Settings.Tails_Second,
            "Sonic_Tower1" => Settings.Sonic_Tower1,
            "Sonic_Tower2" => Settings.Sonic_Tower2,
            "Sonic_Tower3" => Settings.Sonic_Tower3,
            "Sonic_Tower4" => Settings.Sonic_Tower4,
            "Skill_Cyloop" => Settings.Skill_Cyloop,
            "Skill_PhantomRush" => Settings.Skill_PhantomRush,
            "Skill_AirTrick" => Settings.Skill_AirTrick,
            "Skill_StompAttack" => Settings.Skill_StompAttack,
            "Skill_QuickCyloop" => Settings.Skill_QuickCyloop,
            "Skill_SonicBoom" => Settings.Skill_SonicBoom,
            "Skill_WildRush" => Settings.Skill_WildRush,
            "Skill_LoopKick" => Settings.Skill_LoopKick,
            "Skill_HomingShot" => Settings.Skill_HomingShot,
            "Skill_AutoCombo" => Settings.Skill_AutoCombo,
            "Skill_SpinSlash" => Settings.Skill_SpinSlash,
            "Skill_RecoverySmash" => Settings.Skill_RecoverySmash,
            "Kronos_Ninja" => Settings.Kronos_Ninja,
            "Kronos_Door" => Settings.Kronos_Door,
            "Kronos_Amy" => Settings.Kronos_Amy,
            "Kronos_GigantosFirst" => Settings.Kronos_GigantosFirst,
            "Kronos_GreenCE" => Settings.Kronos_GreenCE,
            "Kronos_CyanCE" => Settings.Kronos_CyanCE,
            "Kronos_Tombstones" => Settings.Kronos_Tombstones,
            "Kronos_BlueCE" => Settings.Kronos_BlueCE,
            "Kronos_RedCE" => Settings.Kronos_RedCE,
            "Kronos_YellowCE" => Settings.Kronos_YellowCE,
            "Kronos_WhiteCE" => Settings.Kronos_WhiteCE,
            "Kronos_GigantosStart" => Settings.Kronos_GigantosStart,
            "Kronos_SuperSonic" => Settings.Kronos_SuperSonic,
            "Island_Kronos_story" => Settings.Island_Kronos_story,
            "Island_Kronos_fishing" => Settings.Island_Kronos_fishing,
            "Ares_Knuckles" => Settings.Ares_Knuckles,
            "Ares_WyvernFirst" => Settings.Ares_WyvernFirst,
            "Ares_Water" => Settings.Ares_Water,
            // "Ares_KocoRoundup"  => Settings.Ares_KocoRoundup,
            "Ares_Crane" => Settings.Ares_Crane,
            "Ares_GreenCE" => Settings.Ares_GreenCE,
            "Ares_CyanCE" => Settings.Ares_CyanCE,
            "Ares_BlueCE" => Settings.Ares_BlueCE,
            "Ares_RedCE" => Settings.Ares_RedCE,
            "Ares_YellowCE" => Settings.Ares_YellowCE,
            "Ares_WhiteCE" => Settings.Ares_WhiteCE,
            "Ares_WyvernStart" => Settings.Ares_WyvernStart,
            "Ares_WyvernRun" => Settings.Ares_WyvernRun,
            "Ares_SuperSonic" => Settings.Ares_SuperSonic,
            "Island_Ares_story" => Settings.Island_Ares_story,
            "Island_Ares_fishing" => Settings.Island_Ares_fishing,
            "Chaos_Tails" => Settings.Chaos_Tails,
            "Chaos_KnightFirst" => Settings.Chaos_KnightFirst,
            "Chaos_Hacking" => Settings.Chaos_Hacking,
            "Chaos_GreenCE" => Settings.Chaos_GreenCE,
            "Chaos_CyanCE" => Settings.Chaos_CyanCE,
            "Chaos_PinballStart" => Settings.Chaos_PinballStart,
            "Chaos_PinballEnd" => Settings.Chaos_PinballEnd,
            "Chaos_BlueCE" => Settings.Chaos_BlueCE,
            "Chaos_RedCE" => Settings.Chaos_RedCE,
            "Chaos_YellowCE" => Settings.Chaos_YellowCE,
            "Chaos_WhiteCE" => Settings.Chaos_WhiteCE,
            "Chaos_KnightStart" => Settings.Chaos_KnightStart,
            "Chaos_SuperSonic" => Settings.Chaos_SuperSonic,
            "Island_Chaos_story" => Settings.Island_Chaos_story,
            "Island_Chaos_fishing" => Settings.Island_Chaos_fishing,
            "Rhea_Tower1" => Settings.Rhea_Tower1,
            "Rhea_Tower2" => Settings.Rhea_Tower2,
            "Rhea_Tower3" => Settings.Rhea_Tower3,
            "Rhea_Tower4" => Settings.Rhea_Tower4,
            "Rhea_Tower5" => Settings.Rhea_Tower5,
            "Rhea_Tower6" => Settings.Rhea_Tower6,
            "Island_Rhea_story" => Settings.Island_Rhea_story,
            "Ouranos_Bridge" => Settings.Ouranos_Bridge,
            "Ouranos_SupremeDefeated" => Settings.Ouranos_SupremeDefeated,
            "Ouranos_BlueCE" => Settings.Ouranos_BlueCE,
            "Ouranos_RedCE" => Settings.Ouranos_RedCE,
            "Ouranos_GreenCE" => Settings.Ouranos_GreenCE,
            "Ouranos_YellowCE" => Settings.Ouranos_YellowCE,
            "Ouranos_CyanCE" => Settings.Ouranos_CyanCE,
            "Ouranos_WhiteCE" => Settings.Ouranos_WhiteCE,
            "Island_Ouranos_fishing" => Settings.Island_Ouranos_fishing,
            "Ouranos_FinalDoor" => Settings.Ouranos_FinalDoor,
            _ => false,
        };
    }
}