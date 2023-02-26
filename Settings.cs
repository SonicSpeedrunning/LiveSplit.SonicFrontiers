using System;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.SonicFrontiers
{
    public partial class Settings : UserControl
    {
        // General
        public bool WFocus { get; set; }
        public bool StoryStart { get; set; }
        public bool ArcadeStart { get; set; }
        public bool Arcade1_1 { get; set; }

        // Story - Kronos
        public bool Kronos_Ninja { get; set; }
        public bool Kronos_Door { get; set; }
        public bool Kronos_Amy { get; set; }
        public bool Kronos_GigantosFirst { get; set; }
        public bool Kronos_Tombstones { get; set; }
        public bool Kronos_GigantosStart { get; set; }
        public bool Kronos_SuperSonic { get; set; }
        public bool Island_Kronos_story { get; set; }
        public bool w1_1_story { get; set; }
        public bool w1_2_story { get; set; }
        public bool w1_3_story { get; set; }
        public bool w1_4_story { get; set; }
        public bool w1_5_story { get; set; }
        public bool w1_6_story { get; set; }
        public bool w1_7_story { get; set; }
        public bool Kronos_BlueCE { get; set; }
        public bool Kronos_RedCE { get; set; }
        public bool Kronos_YellowCE { get; set; }
        public bool Kronos_WhiteCE { get; set; }
        public bool Kronos_GreenCE { get; set; }
        public bool Kronos_CyanCE { get; set; }
        public bool Island_Kronos_fishing { get; set; }

        // Story - Ares
        public bool Ares_Knuckles { get; set; }
        public bool Ares_WyvernFirst { get; set; }
        public bool Ares_Water { get; set; }
        public bool Ares_Crane { get; set; }
        public bool Ares_GreenCE { get; set; }
        public bool Ares_CyanCE { get; set; }
        public bool Ares_WyvernStart { get; set; }
        public bool Ares_WyvernRun { get; set; }
        public bool Ares_SuperSonic { get; set; }
        public bool Island_Ares_story { get; set; }
        public bool w2_1_story { get; set; }
        public bool w2_2_story { get; set; }
        public bool w2_3_story { get; set; }
        public bool w2_4_story { get; set; }
        public bool w2_5_story { get; set; }
        public bool w2_6_story { get; set; }
        public bool w2_7_story { get; set; }
        public bool Ares_BlueCE { get; set; }
        public bool Ares_RedCE { get; set; }
        public bool Ares_YellowCE { get; set; }
        public bool Ares_WhiteCE { get; set; }
        public bool Island_Ares_fishing { get; set; }

        // Chaos
        public bool Chaos_Tails { get; set; }
        public bool Chaos_KnightFirst { get; set; }
        public bool Chaos_Hacking { get; set; }
        public bool Chaos_GreenCE { get; set; }
        public bool Chaos_CyanCE { get; set; }
        public bool Chaos_PinballStart { get; set; }
        public bool Chaos_PinballEnd { get; set; }
        public bool Chaos_BlueCE { get; set; }
        public bool Chaos_RedCE { get; set; }
        public bool Chaos_YellowCE { get; set; }
        public bool Chaos_WhiteCE { get; set; }
        public bool Chaos_KnightStart { get; set; }
        public bool Chaos_SuperSonic { get; set; }
        public bool Island_Chaos_story { get; set; }
        public bool Island_Chaos_fishing { get; set; }
        public bool w3_1_story { get; set; }
        public bool w3_2_story { get; set; }
        public bool w3_3_story { get; set; }
        public bool w3_4_story { get; set; }
        public bool w3_5_story { get; set; }
        public bool w3_6_story { get; set; }
        public bool w3_7_story { get; set; }

        // Story - Rhea
        public bool Rhea_Tower1 { get; set; }
        public bool Rhea_Tower2 { get; set; }
        public bool Rhea_Tower3 { get; set; }
        public bool Rhea_Tower4 { get; set; }
        public bool Rhea_Tower5 { get; set; }
        public bool Rhea_Tower6 { get; set; }
        public bool Island_Rhea_story { get; set; }

        // Story - Ouranos
        public bool Ouranos_Bridge { get; set; }
        public bool Ouranos_SupremeDefeated { get; set; }
        public bool FinalBoss { get; set; }
        public bool w4_1_story { get; set; }
        public bool w4_2_story { get; set; }
        public bool w4_3_story { get; set; }
        public bool w4_4_story { get; set; }
        public bool w4_5_story { get; set; }
        public bool w4_6_story { get; set; }
        public bool w4_7_story { get; set; }
        public bool w4_8_story { get; set; }
        public bool w4_9_story { get; set; }
        public bool Ouranos_BlueCE { get; set; }
        public bool Ouranos_RedCE { get; set; }
        public bool Ouranos_GreenCE { get; set; }
        public bool Ouranos_YellowCE { get; set; }
        public bool Ouranos_CyanCE { get; set; }
        public bool Ouranos_WhiteCE { get; set; }
        public bool Island_Ouranos_fishing { get; set; }

        // Arcade mode
        public bool w1_1_arcade { get; set; }
        public bool w1_2_arcade { get; set; }
        public bool w1_3_arcade { get; set; }
        public bool w1_4_arcade { get; set; }
        public bool w1_5_arcade { get; set; }
        public bool w1_6_arcade { get; set; }
        public bool w1_7_arcade { get; set; }
        public bool w2_1_arcade { get; set; }
        public bool w2_2_arcade { get; set; }
        public bool w2_3_arcade { get; set; }
        public bool w2_4_arcade { get; set; }
        public bool w2_5_arcade { get; set; }
        public bool w2_6_arcade { get; set; }
        public bool w2_7_arcade { get; set; }
        public bool w3_1_arcade { get; set; }
        public bool w3_2_arcade { get; set; }
        public bool w3_3_arcade { get; set; }
        public bool w3_4_arcade { get; set; }
        public bool w3_5_arcade { get; set; }
        public bool w3_6_arcade { get; set; }
        public bool w3_7_arcade { get; set; }
        public bool w4_1_arcade { get; set; }
        public bool w4_2_arcade { get; set; }
        public bool w4_3_arcade { get; set; }
        public bool w4_4_arcade { get; set; }
        public bool w4_5_arcade { get; set; }
        public bool w4_6_arcade { get; set; }
        public bool w4_7_arcade { get; set; }
        public bool w4_8_arcade { get; set; }
        public bool w4_9_arcade { get; set; }
        public bool w4_9_arcade_soon { get; set; }


        public Settings()
        {
            InitializeComponent();
            autosplitterVersion.Text = "Autosplitter version: v" + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;

            // General settings
            chkFocus.DataBindings.Add("Checked", this, "WFocus", false, DataSourceUpdateMode.OnPropertyChanged);
            chkStoryStart.DataBindings.Add("Checked", this, "StoryStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkArcadeStart.DataBindings.Add("Checked", this, "ArcadeStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkArcade1_1.DataBindings.Add("Checked", this, "Arcade1_1", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_Ninja.DataBindings.Add("Checked", this, "Kronos_Ninja", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_Door.DataBindings.Add("Checked", this, "Kronos_Door", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_Amy.DataBindings.Add("Checked", this, "Kronos_Amy", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_GigantosFirst.DataBindings.Add("Checked", this, "Kronos_GigantosFirst", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_Tombstones.DataBindings.Add("Checked", this, "Kronos_Tombstones", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_GigantosStart.DataBindings.Add("Checked", this, "Kronos_GigantosStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_SuperSonic.DataBindings.Add("Checked", this, "Kronos_SuperSonic", false, DataSourceUpdateMode.OnPropertyChanged);
            chk50_story.DataBindings.Add("Checked", this, "Island_Kronos_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk0_story.DataBindings.Add("Checked", this, "w1_1_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_story.DataBindings.Add("Checked", this, "w1_2_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_story.DataBindings.Add("Checked", this, "w1_3_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_story.DataBindings.Add("Checked", this, "w1_4_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_story.DataBindings.Add("Checked", this, "w1_5_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk5_story.DataBindings.Add("Checked", this, "w1_6_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk6_story.DataBindings.Add("Checked", this, "w1_7_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_BlueCE.DataBindings.Add("Checked", this, "Kronos_BlueCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_RedCE.DataBindings.Add("Checked", this, "Kronos_RedCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_YellowCE.DataBindings.Add("Checked", this, "Kronos_YellowCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_WhiteCE.DataBindings.Add("Checked", this, "Kronos_WhiteCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_GreenCE.DataBindings.Add("Checked", this, "Kronos_GreenCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_CyanCE.DataBindings.Add("Checked", this, "Kronos_CyanCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chk50_fishing.DataBindings.Add("Checked", this, "Island_Kronos_fishing", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_Knuckles.DataBindings.Add("Checked", this, "Ares_Knuckles", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_WyvernFirst.DataBindings.Add("Checked", this, "Ares_WyvernFirst", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_Water.DataBindings.Add("Checked", this, "Ares_Water", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_Crane.DataBindings.Add("Checked", this, "Ares_Crane", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_GreenCE.DataBindings.Add("Checked", this, "Ares_GreenCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_CyanCE.DataBindings.Add("Checked", this, "Ares_CyanCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_WyvernStart.DataBindings.Add("Checked", this, "Ares_WyvernStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_WyvernRun.DataBindings.Add("Checked", this, "Ares_WyvernRun", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_SuperSonic.DataBindings.Add("Checked", this, "Ares_SuperSonic", false, DataSourceUpdateMode.OnPropertyChanged);
            chk51_story.DataBindings.Add("Checked", this, "Island_Ares_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk7_story.DataBindings.Add("Checked", this, "w2_1_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk8_story.DataBindings.Add("Checked", this, "w2_2_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk9_story.DataBindings.Add("Checked", this, "w2_3_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk10_story.DataBindings.Add("Checked", this, "w2_4_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk11_story.DataBindings.Add("Checked", this, "w2_5_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk12_story.DataBindings.Add("Checked", this, "w2_6_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk13_story.DataBindings.Add("Checked", this, "w2_7_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_BlueCE.DataBindings.Add("Checked", this, "Ares_BlueCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_RedCE.DataBindings.Add("Checked", this, "Ares_RedCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_YellowCE.DataBindings.Add("Checked", this, "Ares_YellowCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_WhiteCE.DataBindings.Add("Checked", this, "Ares_WhiteCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chk51_fishing.DataBindings.Add("Checked", this, "Island_Ares_fishing", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_Tails.DataBindings.Add("Checked", this, "Chaos_Tails", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_KnightFirst.DataBindings.Add("Checked", this, "Chaos_KnightFirst", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_Hacking.DataBindings.Add("Checked", this, "Chaos_Hacking", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_GreenCE.DataBindings.Add("Checked", this, "Chaos_GreenCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_CyanCE.DataBindings.Add("Checked", this, "Chaos_CyanCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_PinballStart.DataBindings.Add("Checked", this, "Chaos_PinballStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_PinballEnd.DataBindings.Add("Checked", this, "Chaos_PinballEnd", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_BlueCE.DataBindings.Add("Checked", this, "Chaos_BlueCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_RedCE.DataBindings.Add("Checked", this, "Chaos_RedCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_YellowCE.DataBindings.Add("Checked", this, "Chaos_YellowCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_WhiteCE.DataBindings.Add("Checked", this, "Chaos_WhiteCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_KnightStart.DataBindings.Add("Checked", this, "Chaos_KnightStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos_SuperSonic.DataBindings.Add("Checked", this, "Chaos_SuperSonic", false, DataSourceUpdateMode.OnPropertyChanged);
            chk52_story.DataBindings.Add("Checked", this, "Island_Chaos_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk52_fishing.DataBindings.Add("Checked", this, "Island_Chaos_fishing", false, DataSourceUpdateMode.OnPropertyChanged);
            chk14_story.DataBindings.Add("Checked", this, "w3_1_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk15_story.DataBindings.Add("Checked", this, "w3_2_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk16_story.DataBindings.Add("Checked", this, "w3_3_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk17_story.DataBindings.Add("Checked", this, "w3_4_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk18_story.DataBindings.Add("Checked", this, "w3_5_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk19_story.DataBindings.Add("Checked", this, "w3_6_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk20_story.DataBindings.Add("Checked", this, "w3_7_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower1.DataBindings.Add("Checked", this, "Rhea_Tower1", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower2.DataBindings.Add("Checked", this, "Rhea_Tower2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower3.DataBindings.Add("Checked", this, "Rhea_Tower3", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower4.DataBindings.Add("Checked", this, "Rhea_Tower4", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower5.DataBindings.Add("Checked", this, "Rhea_Tower5", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower6.DataBindings.Add("Checked", this, "Rhea_Tower6", false, DataSourceUpdateMode.OnPropertyChanged);
            chk53_story.DataBindings.Add("Checked", this, "Island_Rhea_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_Bridge.DataBindings.Add("Checked", this, "Ouranos_Bridge", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_SupremeDefeated.DataBindings.Add("Checked", this, "Ouranos_SupremeDefeated", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFinalBoss.DataBindings.Add("Checked", this, "FinalBoss", false, DataSourceUpdateMode.OnPropertyChanged);
            chk21_story.DataBindings.Add("Checked", this, "w4_1_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk22_story.DataBindings.Add("Checked", this, "w4_2_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk23_story.DataBindings.Add("Checked", this, "w4_3_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk24_story.DataBindings.Add("Checked", this, "w4_4_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk25_story.DataBindings.Add("Checked", this, "w4_5_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk26_story.DataBindings.Add("Checked", this, "w4_6_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk27_story.DataBindings.Add("Checked", this, "w4_7_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk28_story.DataBindings.Add("Checked", this, "w4_8_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk29_story.DataBindings.Add("Checked", this, "w4_9_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_BlueCE.DataBindings.Add("Checked", this, "Ouranos_BlueCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_RedCE.DataBindings.Add("Checked", this, "Ouranos_RedCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_GreenCE.DataBindings.Add("Checked", this, "Ouranos_GreenCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_YellowCE.DataBindings.Add("Checked", this, "Ouranos_YellowCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_CyanCE.DataBindings.Add("Checked", this, "Ouranos_CyanCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_WhiteCE.DataBindings.Add("Checked", this, "Ouranos_WhiteCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chk54_fishing.DataBindings.Add("Checked", this, "Island_Ouranos_fishing", false, DataSourceUpdateMode.OnPropertyChanged);
            chk0_arcade.DataBindings.Add("Checked", this, "w1_1_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_arcade.DataBindings.Add("Checked", this, "w1_2_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_arcade.DataBindings.Add("Checked", this, "w1_3_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_arcade.DataBindings.Add("Checked", this, "w1_4_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_arcade.DataBindings.Add("Checked", this, "w1_5_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk5_arcade.DataBindings.Add("Checked", this, "w1_6_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk6_arcade.DataBindings.Add("Checked", this, "w1_7_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk7_arcade.DataBindings.Add("Checked", this, "w2_1_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk8_arcade.DataBindings.Add("Checked", this, "w2_2_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk9_arcade.DataBindings.Add("Checked", this, "w2_3_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk10_arcade.DataBindings.Add("Checked", this, "w2_4_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk11_arcade.DataBindings.Add("Checked", this, "w2_5_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk12_arcade.DataBindings.Add("Checked", this, "w2_6_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk13_arcade.DataBindings.Add("Checked", this, "w2_7_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk14_arcade.DataBindings.Add("Checked", this, "w3_1_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk15_arcade.DataBindings.Add("Checked", this, "w3_2_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk16_arcade.DataBindings.Add("Checked", this, "w3_3_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk17_arcade.DataBindings.Add("Checked", this, "w3_4_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk18_arcade.DataBindings.Add("Checked", this, "w3_5_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk19_arcade.DataBindings.Add("Checked", this, "w3_6_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk20_arcade.DataBindings.Add("Checked", this, "w3_7_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk21_arcade.DataBindings.Add("Checked", this, "w4_1_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk22_arcade.DataBindings.Add("Checked", this, "w4_2_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk23_arcade.DataBindings.Add("Checked", this, "w4_3_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk24_arcade.DataBindings.Add("Checked", this, "w4_4_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk25_arcade.DataBindings.Add("Checked", this, "w4_5_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk26_arcade.DataBindings.Add("Checked", this, "w4_6_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk27_arcade.DataBindings.Add("Checked", this, "w4_7_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk28_arcade.DataBindings.Add("Checked", this, "w4_8_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk29_arcade.DataBindings.Add("Checked", this, "w4_9_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk29_arcade_soon.DataBindings.Add("Checked", this, "w4_9_arcade_soon", false, DataSourceUpdateMode.OnPropertyChanged);

            // Default Values
            WFocus = false;
            StoryStart = ArcadeStart = Arcade1_1 = true;

            // Kronos
            Kronos_Ninja = Kronos_Door = Kronos_Amy = Kronos_GigantosFirst = Kronos_Tombstones = false;
            Island_Kronos_story = Island_Kronos_fishing = true;
            w1_1_story = w1_2_story = w1_3_story = w1_4_story = w1_5_story = w1_6_story = w1_7_story = false;
            Kronos_BlueCE = Kronos_RedCE = Kronos_YellowCE = Kronos_WhiteCE = Kronos_GreenCE = Kronos_CyanCE  = Kronos_GigantosStart = Kronos_SuperSonic = false;

            // Ares
            Ares_Knuckles = Ares_WyvernFirst = Ares_Water = Ares_Crane = false;
            Ares_WyvernRun = Ares_WyvernStart = Ares_SuperSonic = false;
            Island_Ares_story = Island_Ares_fishing = true;
            w2_1_story = w2_2_story = w2_3_story = w2_4_story = w2_5_story = w2_6_story = w2_7_story = false;
            Ares_GreenCE = Ares_CyanCE = Ares_BlueCE = Ares_RedCE = Ares_YellowCE = Ares_YellowCE = false;

            // Chaos
            Chaos_Tails = Chaos_KnightFirst = Chaos_Hacking = false;
            Chaos_GreenCE = Chaos_CyanCE = Chaos_PinballEnd = Chaos_PinballStart = false;
            Chaos_BlueCE = Chaos_RedCE = Chaos_YellowCE = Chaos_WhiteCE = false;
            Chaos_KnightStart = Chaos_SuperSonic = false;
            Island_Chaos_story = Island_Chaos_fishing = true;
            w3_1_story = w3_2_story = w3_3_story = w3_4_story = w3_5_story = w3_6_story = w3_7_story = false;

            // Rhea
            Rhea_Tower1 = Rhea_Tower2 = Rhea_Tower3 = Rhea_Tower4 = Rhea_Tower5 = Rhea_Tower6 = false;
            Island_Rhea_story = true;

            // Ouranos
            Ouranos_Bridge = Ouranos_SupremeDefeated = false;
            FinalBoss = true;
            w4_1_story = w4_2_story = w4_3_story = w4_4_story = w4_5_story = w4_6_story = w4_7_story = w4_8_story = w4_9_story = false;
            Ouranos_BlueCE = Ouranos_RedCE = Ouranos_GreenCE = Ouranos_YellowCE = Ouranos_CyanCE = Ouranos_WhiteCE = false;
            Island_Ouranos_fishing = true;

            // Arcade mode
            w1_1_arcade = w1_2_arcade = w1_3_arcade = w1_4_arcade = w1_5_arcade = w1_6_arcade = w1_7_arcade = true;
            w2_1_arcade = w2_2_arcade = w2_3_arcade = w2_4_arcade = w2_5_arcade = w2_6_arcade = w2_7_arcade = true;
            w3_1_arcade = w3_2_arcade = w3_3_arcade = w3_4_arcade = w3_5_arcade = w3_6_arcade = w3_7_arcade = true;
            w4_1_arcade = w4_2_arcade = w4_3_arcade = w4_4_arcade = w4_5_arcade = w4_6_arcade = w4_7_arcade = w4_8_arcade = w4_9_arcade = true;
            w4_9_arcade_soon = true;
        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings");
            settingsNode.AppendChild(ToElement(doc, "WFocus", WFocus));
            settingsNode.AppendChild(ToElement(doc, "StoryStart", StoryStart));
            settingsNode.AppendChild(ToElement(doc, "ArcadeStart", ArcadeStart));
            settingsNode.AppendChild(ToElement(doc, "Kronos_Ninja", Kronos_Ninja));
            settingsNode.AppendChild(ToElement(doc, "Kronos_Door", Kronos_Door));
            settingsNode.AppendChild(ToElement(doc, "Kronos_Amy", Kronos_Amy));
            settingsNode.AppendChild(ToElement(doc, "Kronos_GigantosFirst", Kronos_GigantosFirst));
            settingsNode.AppendChild(ToElement(doc, "Kronos_Tombstones", Kronos_Tombstones));
            settingsNode.AppendChild(ToElement(doc, "Kronos_GigantosStart", Kronos_GigantosStart));
            settingsNode.AppendChild(ToElement(doc, "Kronos_SuperSonic", Kronos_SuperSonic));
            settingsNode.AppendChild(ToElement(doc, "Island_Kronos_story", Island_Kronos_story));
            settingsNode.AppendChild(ToElement(doc, "w1_1_story", w1_1_story));
            settingsNode.AppendChild(ToElement(doc, "w1_2_story", w1_2_story));
            settingsNode.AppendChild(ToElement(doc, "w1_3_story", w1_3_story));
            settingsNode.AppendChild(ToElement(doc, "w1_4_story", w1_4_story));
            settingsNode.AppendChild(ToElement(doc, "w1_5_story", w1_5_story));
            settingsNode.AppendChild(ToElement(doc, "w1_6_story", w1_6_story));
            settingsNode.AppendChild(ToElement(doc, "w1_7_story", w1_7_story));
            settingsNode.AppendChild(ToElement(doc, "Kronos_BlueCE", Kronos_BlueCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_RedCE", Kronos_RedCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_YellowCE", Kronos_YellowCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_WhiteCE", Kronos_WhiteCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_GreenCE", Kronos_GreenCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_CyanCE", Kronos_CyanCE));
            settingsNode.AppendChild(ToElement(doc, "Island_Kronos_fishing", Island_Kronos_fishing));
            settingsNode.AppendChild(ToElement(doc, "Ares_Knuckles", Ares_Knuckles));
            settingsNode.AppendChild(ToElement(doc, "Ares_WyvernFirst", Ares_WyvernFirst));
            settingsNode.AppendChild(ToElement(doc, "Ares_Water", Ares_Water));
            settingsNode.AppendChild(ToElement(doc, "Ares_Crane", Ares_Crane));
            settingsNode.AppendChild(ToElement(doc, "Ares_GreenCE", Ares_GreenCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_CyanCE", Ares_CyanCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_WyvernStart", Ares_WyvernStart));
            settingsNode.AppendChild(ToElement(doc, "Ares_WyvernRun", Ares_WyvernRun));
            settingsNode.AppendChild(ToElement(doc, "Ares_SuperSonic", Ares_SuperSonic));
            settingsNode.AppendChild(ToElement(doc, "Island_Ares_story", Island_Ares_story));
            settingsNode.AppendChild(ToElement(doc, "w2_1_story", w2_1_story));
            settingsNode.AppendChild(ToElement(doc, "w2_2_story", w2_2_story));
            settingsNode.AppendChild(ToElement(doc, "w2_3_story", w2_3_story));
            settingsNode.AppendChild(ToElement(doc, "w2_4_story", w2_4_story));
            settingsNode.AppendChild(ToElement(doc, "w2_5_story", w2_5_story));
            settingsNode.AppendChild(ToElement(doc, "w2_6_story", w2_6_story));
            settingsNode.AppendChild(ToElement(doc, "w2_7_story", w2_7_story));
            settingsNode.AppendChild(ToElement(doc, "Ares_BlueCE", Ares_BlueCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_RedCE", Ares_RedCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_YellowCE", Ares_YellowCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_WhiteCE", Ares_WhiteCE));
            settingsNode.AppendChild(ToElement(doc, "Island_Ares_fishing", Island_Ares_fishing));
            settingsNode.AppendChild(ToElement(doc, "Chaos_Tails", Chaos_Tails));
            settingsNode.AppendChild(ToElement(doc, "Chaos_KnightFirst", Chaos_KnightFirst));
            settingsNode.AppendChild(ToElement(doc, "Chaos_Hacking", Chaos_Hacking));
            settingsNode.AppendChild(ToElement(doc, "Chaos_GreenCE", Chaos_GreenCE));
            settingsNode.AppendChild(ToElement(doc, "Chaos_CyanCE", Chaos_CyanCE));
            settingsNode.AppendChild(ToElement(doc, "Chaos_PinballStart", Chaos_PinballStart));
            settingsNode.AppendChild(ToElement(doc, "Chaos_PinballEnd", Chaos_PinballEnd));
            settingsNode.AppendChild(ToElement(doc, "Chaos_BlueCE", Chaos_BlueCE));
            settingsNode.AppendChild(ToElement(doc, "Chaos_RedCE", Chaos_RedCE));
            settingsNode.AppendChild(ToElement(doc, "Chaos_YellowCE", Chaos_YellowCE));
            settingsNode.AppendChild(ToElement(doc, "Chaos_WhiteCE", Chaos_WhiteCE));
            settingsNode.AppendChild(ToElement(doc, "Chaos_KnightStart", Chaos_KnightStart));
            settingsNode.AppendChild(ToElement(doc, "Chaos_SuperSonic", Chaos_SuperSonic));
            settingsNode.AppendChild(ToElement(doc, "Island_Chaos_story", Island_Chaos_story));
            settingsNode.AppendChild(ToElement(doc, "Island_Chaos_fishing", Island_Chaos_fishing));
            settingsNode.AppendChild(ToElement(doc, "w3_1_story", w3_1_story));
            settingsNode.AppendChild(ToElement(doc, "w3_2_story", w3_2_story));
            settingsNode.AppendChild(ToElement(doc, "w3_3_story", w3_3_story));
            settingsNode.AppendChild(ToElement(doc, "w3_4_story", w3_4_story));
            settingsNode.AppendChild(ToElement(doc, "w3_5_story", w3_5_story));
            settingsNode.AppendChild(ToElement(doc, "w3_6_story", w3_6_story));
            settingsNode.AppendChild(ToElement(doc, "w3_7_story", w3_7_story));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower1", Rhea_Tower1));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower2", Rhea_Tower2));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower3", Rhea_Tower3));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower4", Rhea_Tower4));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower5", Rhea_Tower5));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower6", Rhea_Tower6));
            settingsNode.AppendChild(ToElement(doc, "Island_Rhea_story", Island_Rhea_story));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_Bridge", Ouranos_Bridge));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_SupremeDefeated", Ouranos_SupremeDefeated));
            settingsNode.AppendChild(ToElement(doc, "FinalBoss", FinalBoss));
            settingsNode.AppendChild(ToElement(doc, "w4_1_story", w4_1_story));
            settingsNode.AppendChild(ToElement(doc, "w4_2_story", w4_2_story));
            settingsNode.AppendChild(ToElement(doc, "w4_3_story", w4_3_story));
            settingsNode.AppendChild(ToElement(doc, "w4_4_story", w4_4_story));
            settingsNode.AppendChild(ToElement(doc, "w4_5_story", w4_5_story));
            settingsNode.AppendChild(ToElement(doc, "w4_6_story", w4_6_story));
            settingsNode.AppendChild(ToElement(doc, "w4_7_story", w4_7_story));
            settingsNode.AppendChild(ToElement(doc, "w4_8_story", w4_8_story));
            settingsNode.AppendChild(ToElement(doc, "w4_9_story", w4_9_story));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_BlueCE", Ouranos_BlueCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_RedCE", Ouranos_RedCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_GreenCE", Ouranos_GreenCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_YellowCE", Ouranos_YellowCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_CyanCE", Ouranos_CyanCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_WhiteCE", Ouranos_WhiteCE));
            settingsNode.AppendChild(ToElement(doc, "Island_Ouranos_fishing", Island_Ouranos_fishing));
            settingsNode.AppendChild(ToElement(doc, "w1_1_arcade", w1_1_arcade));
            settingsNode.AppendChild(ToElement(doc, "w1_2_arcade", w1_2_arcade));
            settingsNode.AppendChild(ToElement(doc, "w1_3_arcade", w1_3_arcade));
            settingsNode.AppendChild(ToElement(doc, "w1_4_arcade", w1_4_arcade));
            settingsNode.AppendChild(ToElement(doc, "w1_5_arcade", w1_5_arcade));
            settingsNode.AppendChild(ToElement(doc, "w1_6_arcade", w1_6_arcade));
            settingsNode.AppendChild(ToElement(doc, "w1_7_arcade", w1_7_arcade));
            settingsNode.AppendChild(ToElement(doc, "w2_1_arcade", w2_1_arcade));
            settingsNode.AppendChild(ToElement(doc, "w2_2_arcade", w2_2_arcade));
            settingsNode.AppendChild(ToElement(doc, "w2_3_arcade", w2_3_arcade));
            settingsNode.AppendChild(ToElement(doc, "w2_4_arcade", w2_4_arcade));
            settingsNode.AppendChild(ToElement(doc, "w2_5_arcade", w2_5_arcade));
            settingsNode.AppendChild(ToElement(doc, "w2_6_arcade", w2_6_arcade));
            settingsNode.AppendChild(ToElement(doc, "w2_7_arcade", w2_7_arcade));
            settingsNode.AppendChild(ToElement(doc, "w3_1_arcade", w3_1_arcade));
            settingsNode.AppendChild(ToElement(doc, "w3_2_arcade", w3_2_arcade));
            settingsNode.AppendChild(ToElement(doc, "w3_3_arcade", w3_3_arcade));
            settingsNode.AppendChild(ToElement(doc, "w3_4_arcade", w3_4_arcade));
            settingsNode.AppendChild(ToElement(doc, "w3_5_arcade", w3_5_arcade));
            settingsNode.AppendChild(ToElement(doc, "w3_6_arcade", w3_6_arcade));
            settingsNode.AppendChild(ToElement(doc, "w3_7_arcade", w3_7_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_1_arcade", w4_1_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_2_arcade", w4_2_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_3_arcade", w4_3_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_4_arcade", w4_4_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_5_arcade", w4_5_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_6_arcade", w4_6_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_7_arcade", w4_7_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_8_arcade", w4_8_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_9_arcade", w4_9_arcade));
            settingsNode.AppendChild(ToElement(doc, "w4_9_arcade_soon", w4_9_arcade_soon));
            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            WFocus = ParseBool(settings, "WFocus", false);
            StoryStart = ParseBool(settings, "StoryStart", true);
            ArcadeStart = ParseBool(settings, "ArcadeStart", true);
            Arcade1_1 = ParseBool(settings, "Arcade1_1", true);
            Kronos_Ninja = ParseBool(settings, "Kronos_Ninja", false);
            Kronos_Door = ParseBool(settings, "Kronos_Door", false);
            Kronos_Amy = ParseBool(settings, "Kronos_Amy", false);
            Kronos_GigantosFirst = ParseBool(settings, "Kronos_GigantosFirst", false);
            Kronos_Tombstones = ParseBool(settings, "Kronos_Tombstones", false);
            Kronos_GigantosStart = ParseBool(settings, "Kronos_GigantosStart", false);
            Kronos_SuperSonic = ParseBool(settings, "Kronos_SuperSonic", false);
            Island_Kronos_story = ParseBool(settings, "Island_Kronos_story", true);
            w1_1_story = ParseBool(settings, "w1_1_story", false);
            w1_2_story = ParseBool(settings, "w1_2_story", false);
            w1_3_story = ParseBool(settings, "w1_3_story", false);
            w1_4_story = ParseBool(settings, "w1_4_story", false);
            w1_5_story = ParseBool(settings, "w1_5_story", false);
            w1_6_story = ParseBool(settings, "w1_6_story", false);
            w1_7_story = ParseBool(settings, "w1_7_story", false);
            Kronos_BlueCE = ParseBool(settings, "Kronos_BlueCE", false);
            Kronos_RedCE = ParseBool(settings, "Kronos_RedCE", false);
            Kronos_YellowCE = ParseBool(settings, "Kronos_YellowCE", false);
            Kronos_WhiteCE = ParseBool(settings, "Kronos_WhiteCE", false);
            Kronos_GreenCE = ParseBool(settings, "Kronos_GreenCE", false);
            Kronos_CyanCE = ParseBool(settings, "Kronos_CyanCE", false);
            Island_Kronos_fishing = ParseBool(settings, "Island_Kronos_fishing", true);
            Ares_Knuckles = ParseBool(settings, "Ares_Knuckles", false);
            Ares_WyvernFirst = ParseBool(settings, "Ares_WyvernFirst", false);
            Ares_Water = ParseBool(settings, "Ares_Water", false);
            Ares_Crane = ParseBool(settings, "Ares_Crane", false);
            Ares_GreenCE = ParseBool(settings, "Ares_GreenCE", false);
            Ares_CyanCE = ParseBool(settings, "Ares_CyanCE", false);
            Ares_WyvernStart = ParseBool(settings, "Ares_WyvernStart", false);
            Ares_WyvernRun = ParseBool(settings, "Ares_WyvernRun", false);
            Ares_SuperSonic = ParseBool(settings, "Ares_SuperSonic", false);
            Island_Ares_story = ParseBool(settings, "Island_Ares_story", true);
            w2_1_story = ParseBool(settings, "w2_1_story", false);
            w2_2_story = ParseBool(settings, "w2_2_story", false);
            w2_3_story = ParseBool(settings, "w2_3_story", false);
            w2_4_story = ParseBool(settings, "w2_4_story", false);
            w2_5_story = ParseBool(settings, "w2_5_story", false);
            w2_6_story = ParseBool(settings, "w2_6_story", false);
            w2_7_story = ParseBool(settings, "w2_7_story", false);
            Ares_BlueCE = ParseBool(settings, "Ares_BlueCE", false);
            Ares_RedCE = ParseBool(settings, "Ares_RedCE", false);
            Ares_YellowCE = ParseBool(settings, "Ares_YellowCE", false);
            Ares_WhiteCE = ParseBool(settings, "Ares_WhiteCE", false);
            Island_Ares_fishing = ParseBool(settings, "Island_Ares_fishing", true);
            Chaos_Tails = ParseBool(settings, "Chaos_Tails", false);
            Chaos_KnightFirst = ParseBool(settings, "Chaos_KnightFirst", false);
            Chaos_Hacking = ParseBool(settings, "Chaos_Hacking", false);
            Chaos_GreenCE = ParseBool(settings, "Chaos_GreenCE", false);
            Chaos_CyanCE = ParseBool(settings, "Chaos_CyanCE", false);
            Chaos_PinballStart = ParseBool(settings, "Chaos_PinballStart", false);
            Chaos_PinballEnd = ParseBool(settings, "Chaos_PinballEnd", false);
            Chaos_BlueCE = ParseBool(settings, "Chaos_BlueCE", false);
            Chaos_RedCE = ParseBool(settings, "Chaos_RedCE", false);
            Chaos_YellowCE = ParseBool(settings, "Chaos_YellowCE", false);
            Chaos_WhiteCE = ParseBool(settings, "Chaos_WhiteCE", false);
            Chaos_KnightStart = ParseBool(settings, "Chaos_KnightStart", false);
            Chaos_SuperSonic = ParseBool(settings, "Chaos_SuperSonic", false);
            Island_Chaos_story = ParseBool(settings, "Island_Chaos_story", true);
            Island_Chaos_fishing = ParseBool(settings, "Island_Chaos_fishing", true);
            w3_1_story = ParseBool(settings, "w3_1_story", false);
            w3_2_story = ParseBool(settings, "w3_2_story", false);
            w3_3_story = ParseBool(settings, "w3_3_story", false);
            w3_4_story = ParseBool(settings, "w3_4_story", false);
            w3_5_story = ParseBool(settings, "w3_5_story", false);
            w3_6_story = ParseBool(settings, "w3_6_story", false);
            w3_7_story = ParseBool(settings, "w3_7_story", false);
            Rhea_Tower1 = ParseBool(settings, "Rhea_Tower1", false);
            Rhea_Tower2 = ParseBool(settings, "Rhea_Tower2", false);
            Rhea_Tower3 = ParseBool(settings, "Rhea_Tower3", false);
            Rhea_Tower4 = ParseBool(settings, "Rhea_Tower4", false);
            Rhea_Tower5 = ParseBool(settings, "Rhea_Tower5", false);
            Rhea_Tower6 = ParseBool(settings, "Rhea_Tower6", false);
            Island_Rhea_story = ParseBool(settings, "Island_Rhea_story", true);
            Ouranos_Bridge = ParseBool(settings, "Ouranos_Bridge", false);
            Ouranos_SupremeDefeated = ParseBool(settings, "Ouranos_SupremeDefeated", false);
            FinalBoss = ParseBool(settings, "FinalBoss", true);
            w4_1_story = ParseBool(settings, "w4_1_story", false);
            w4_2_story = ParseBool(settings, "w4_2_story", false);
            w4_3_story = ParseBool(settings, "w4_3_story", false);
            w4_4_story = ParseBool(settings, "w4_4_story", false);
            w4_5_story = ParseBool(settings, "w4_5_story", false);
            w4_6_story = ParseBool(settings, "w4_6_story", false);
            w4_7_story = ParseBool(settings, "w4_7_story", false);
            w4_8_story = ParseBool(settings, "w4_8_story", false);
            w4_9_story = ParseBool(settings, "w4_9_story", false);
            Ouranos_BlueCE = ParseBool(settings, "Ouranos_BlueCE", false);
            Ouranos_RedCE = ParseBool(settings, "Ouranos_RedCE", false);
            Ouranos_GreenCE = ParseBool(settings, "Ouranos_GreenCE", false);
            Ouranos_YellowCE = ParseBool(settings, "Ouranos_YellowCE", false);
            Ouranos_CyanCE = ParseBool(settings, "Ouranos_CyanCE", false);
            Ouranos_WhiteCE = ParseBool(settings, "Ouranos_WhiteCE", false);
            Island_Ouranos_fishing = ParseBool(settings, "Island_Ouranos_fishing", true);
            w1_1_arcade = ParseBool(settings, "w1_1_arcade", true);
            w1_2_arcade = ParseBool(settings, "w1_2_arcade", true);
            w1_3_arcade = ParseBool(settings, "w1_3_arcade", true);
            w1_4_arcade = ParseBool(settings, "w1_4_arcade", true);
            w1_5_arcade = ParseBool(settings, "w1_5_arcade", true);
            w1_6_arcade = ParseBool(settings, "w1_6_arcade", true);
            w1_7_arcade = ParseBool(settings, "w1_7_arcade", true);
            w2_1_arcade = ParseBool(settings, "w2_1_arcade", true);
            w2_2_arcade = ParseBool(settings, "w2_2_arcade", true);
            w2_3_arcade = ParseBool(settings, "w2_3_arcade", true);
            w2_4_arcade = ParseBool(settings, "w2_4_arcade", true);
            w2_5_arcade = ParseBool(settings, "w2_5_arcade", true);
            w2_6_arcade = ParseBool(settings, "w2_6_arcade", true);
            w2_7_arcade = ParseBool(settings, "w2_7_arcade", true);
            w3_1_arcade = ParseBool(settings, "w3_1_arcade", true);
            w3_2_arcade = ParseBool(settings, "w3_2_arcade", true);
            w3_3_arcade = ParseBool(settings, "w3_3_arcade", true);
            w3_4_arcade = ParseBool(settings, "w3_4_arcade", true);
            w3_5_arcade = ParseBool(settings, "w3_5_arcade", true);
            w3_6_arcade = ParseBool(settings, "w3_6_arcade", true);
            w3_7_arcade = ParseBool(settings, "w3_7_arcade", true);
            w4_1_arcade = ParseBool(settings, "w4_1_arcade", true);
            w4_2_arcade = ParseBool(settings, "w4_2_arcade", true);
            w4_3_arcade = ParseBool(settings, "w4_3_arcade", true);
            w4_4_arcade = ParseBool(settings, "w4_4_arcade", true);
            w4_5_arcade = ParseBool(settings, "w4_5_arcade", true);
            w4_6_arcade = ParseBool(settings, "w4_6_arcade", true);
            w4_7_arcade = ParseBool(settings, "w4_7_arcade", true);
            w4_8_arcade = ParseBool(settings, "w4_8_arcade", true);
            w4_9_arcade = ParseBool(settings, "w4_9_arcade", true);
            w4_9_arcade_soon = ParseBool(settings, "w4_9_arcade_soon", true);
        }

        static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
        {
            return settings[setting] != null ? (bool.TryParse(settings[setting].InnerText, out bool val) ? val : default_) : default_;
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
        }

        private void ChkArcadeStart_CheckedChanged(object sender, EventArgs e)
        {
            chkArcade1_1.Enabled = chkArcadeStart.Checked;
        }

        public bool this[string entry]
        {
            get
            {
                bool t;
                try { t = (bool)this.GetType().GetProperty(entry).GetValue(this, null); }
                catch { return false; }
                return t;
            }
        }

        private void Chk4_9_arcade_CheckedChanged(object sender, EventArgs e)
        {
            chk29_arcade_soon.Enabled = chk29_arcade.Checked;
        }

        private void KronosButton_Click(object sender, EventArgs e)
        {
            chkKronos_Ninja.Checked = false;
            chkKronos_Door.Checked = false;
            chkKronos_Amy.Checked = false;
            chkKronos_GigantosFirst.Checked = false;
            chkKronos_Tombstones.Checked = false;
            chkKronos_GigantosStart.Checked = false;
            chk50_story.Checked = true;
            chk0_story.Checked = false;
            chk1_story.Checked = false;
            chk2_story.Checked = false;
            chk3_story.Checked = false;
            chk4_story.Checked = false;
            chk5_story.Checked = false;
            chk6_story.Checked = false;
            chkKronos_BlueCE.Checked = false;
            chkKronos_RedCE.Checked = false;
            chkKronos_YellowCE.Checked = false;
            chkKronos_WhiteCE.Checked = false;
            chkKronos_GreenCE.Checked = false;
            chkKronos_CyanCE.Checked = false;
            chk50_fishing.Checked = true;
        }

        private void RheaButton_Click(object sender, EventArgs e)
        {
            chkRhea_Tower1.Checked = false;
            chkRhea_Tower2.Checked = false;
            chkRhea_Tower3.Checked = false;
            chkRhea_Tower4.Checked = false;
            chkRhea_Tower5.Checked = false;
            chkRhea_Tower6.Checked = false;
            chk53_story.Checked = true;
        }

        private void OuranosButton_Click(object sender, EventArgs e)
        {
            chkOuranos_Bridge.Checked = false;
            chkOuranos_SupremeDefeated.Checked = false;
            chkFinalBoss.Checked = true;
            chk21_story.Checked = false;
            chk22_story.Checked = false;
            chk23_story.Checked = false;
            chk24_story.Checked = false;
            chk25_story.Checked = false;
            chk26_story.Checked = false;
            chk27_story.Checked = false;
            chk28_story.Checked = false;
            chk29_story.Checked = false;
            chkOuranos_BlueCE.Checked = false;
            chkOuranos_RedCE.Checked = false;
            chkOuranos_GreenCE.Checked = false;
            chkOuranos_YellowCE.Checked = false;
            chkOuranos_CyanCE.Checked = false;
            chkOuranos_WhiteCE.Checked = false;
            chk54_fishing.Checked = true;
        }

        private void ArcadeButton_Click(object sender, EventArgs e)
        {
            chk0_arcade.Checked = true;
            chk1_arcade.Checked = true;
            chk2_arcade.Checked = true;
            chk3_arcade.Checked = true;
            chk4_arcade.Checked = true;
            chk5_arcade.Checked = true;
            chk6_arcade.Checked = true;
            chk7_arcade.Checked = true;
            chk8_arcade.Checked = true;
            chk9_arcade.Checked = true;
            chk10_arcade.Checked = true;
            chk11_arcade.Checked = true;
            chk12_arcade.Checked = true;
            chk13_arcade.Checked = true;
            chk14_arcade.Checked = true;
            chk15_arcade.Checked = true;
            chk16_arcade.Checked = true;
            chk17_arcade.Checked = true;
            chk18_arcade.Checked = true;
            chk19_arcade.Checked = true;
            chk20_arcade.Checked = true;
            chk21_arcade.Checked = true;
            chk22_arcade.Checked = true;
            chk23_arcade.Checked = true;
            chk24_arcade.Checked = true;
            chk25_arcade.Checked = true;
            chk26_arcade.Checked = true;
            chk27_arcade.Checked = true;
            chk28_arcade.Checked = true;
            chk29_arcade.Checked = true;
            chk29_arcade_soon.Checked = true;
        }

        private void chk53_story_CheckedChanged(object sender, EventArgs e)
        {
            rheaWarning.Enabled = chk53_story.Checked;
        }

        private void DiscordLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://discord.gg/WkQW4rv73J");
        }

        private void AresButton_Click(object sender, EventArgs e)
        {
            chkAres_Knuckles.Checked = false;
            chkAres_WyvernFirst.Checked = false;
            chkAres_Water.Checked = false;
            chkAres_Crane.Checked = false;
            chkAres_WyvernStart.Checked = false;
            chkAres_WyvernRun.Checked = false;
            chkAres_SuperSonic.Checked = false;
            chk51_story.Checked = true;
            chk7_story.Checked = false;
            chk8_story.Checked = false;
            chk9_story.Checked = false;
            chk10_story.Checked = false;
            chk11_story.Checked = false;
            chk12_story.Checked = false;
            chk13_story.Checked = false;
            chkAres_BlueCE.Checked = false;
            chkAres_RedCE.Checked = false;
            chkAres_YellowCE.Checked = false;
            chkAres_WhiteCE.Checked = false;
            chkAres_GreenCE.Checked = false;
            chkAres_CyanCE.Checked = false;
            chk51_fishing.Checked = true;
        }

        private void ChaosButton_Click(object sender, EventArgs e)
        {
            chkChaos_Tails.Checked = false;
            chkChaos_KnightFirst.Checked = false;
            chkChaos_Hacking.Checked = false;
            chkChaos_GreenCE.Checked = false;
            chkChaos_CyanCE.Checked = false;
            chkChaos_PinballStart.Checked = false;
            chkChaos_PinballEnd.Checked = false;
            chkChaos_BlueCE.Checked = false;
            chkChaos_RedCE.Checked = false;
            chkChaos_YellowCE.Checked = false;
            chkChaos_WhiteCE.Checked = false;
            chkChaos_KnightStart.Checked = false;
            chkChaos_SuperSonic.Checked = false;
            chk52_story.Checked = true;
            chk52_fishing.Checked = true;
            chk14_story.Checked = false;
            chk15_story.Checked = false;
            chk16_story.Checked = false;
            chk17_story.Checked = false;
            chk18_story.Checked = false;
            chk19_story.Checked = false;
            chk20_story.Checked = false;
        }


        public event EventHandler<bool> WFocusChange;

        private void chkFocus_CheckedChanged(object sender, EventArgs e)
        {
            WFocusChange?.Invoke(this, chkFocus.Checked);
        }
    }
}
