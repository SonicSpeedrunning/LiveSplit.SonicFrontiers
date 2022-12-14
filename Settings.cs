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
        public bool c50_story { get; set; }
        public bool c0_story { get; set; }
        public bool c1_story { get; set; }
        public bool c2_story { get; set; }
        public bool c3_story { get; set; }
        public bool c4_story { get; set; }
        public bool c5_story { get; set; }
        public bool c6_story { get; set; }
        public bool Kronos_BlueCE { get; set; }
        public bool Kronos_RedCE { get; set; }
        public bool Kronos_YellowCE { get; set; }
        public bool Kronos_WhiteCE { get; set; }
        public bool Kronos_GreenCE { get; set; }
        public bool Kronos_CyanCE { get; set; }
        public bool c50_fishing { get; set; }

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
        public bool c51_story { get; set; }
        public bool c7_story { get; set; }
        public bool c8_story { get; set; }
        public bool c9_story { get; set; }
        public bool c10_story { get; set; }
        public bool c11_story { get; set; }
        public bool c12_story { get; set; }
        public bool c13_story { get; set; }
        public bool Ares_BlueCE { get; set; }
        public bool Ares_RedCE { get; set; }
        public bool Ares_YellowCE { get; set; }
        public bool Ares_WhiteCE { get; set; }
        public bool c51_fishing { get; set; }

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
        public bool c52_story { get; set; }
        public bool c52_fishing { get; set; }
        public bool c14_story { get; set; }
        public bool c15_story { get; set; }
        public bool c16_story { get; set; }
        public bool c17_story { get; set; }
        public bool c18_story { get; set; }
        public bool c19_story { get; set; }
        public bool c20_story { get; set; }

        // Story - Rhea
        public bool Rhea_Tower1 { get; set; }
        public bool Rhea_Tower2 { get; set; }
        public bool Rhea_Tower3 { get; set; }
        public bool Rhea_Tower4 { get; set; }
        public bool Rhea_Tower5 { get; set; }
        public bool Rhea_Tower6 { get; set; }
        public bool c53_story { get; set; }

        // Story - Ouranos
        public bool Ouranos_Bridge { get; set; }
        public bool Ouranos_SupremeDefeated { get; set; }
        public bool FinalBoss { get; set; }
        public bool c21_story { get; set; }
        public bool c22_story { get; set; }
        public bool c23_story { get; set; }
        public bool c24_story { get; set; }
        public bool c25_story { get; set; }
        public bool c26_story { get; set; }
        public bool c27_story { get; set; }
        public bool c28_story { get; set; }
        public bool c29_story { get; set; }
        public bool Ouranos_BlueCE { get; set; }
        public bool Ouranos_RedCE { get; set; }
        public bool Ouranos_GreenCE { get; set; }
        public bool Ouranos_YellowCE { get; set; }
        public bool Ouranos_CyanCE { get; set; }
        public bool Ouranos_WhiteCE { get; set; }
        public bool c54_fishing { get; set; }

        // Arcade mode
        public bool c0_arcade { get; set; }
        public bool c1_arcade { get; set; }
        public bool c2_arcade { get; set; }
        public bool c3_arcade { get; set; }
        public bool c4_arcade { get; set; }
        public bool c5_arcade { get; set; }
        public bool c6_arcade { get; set; }
        public bool c7_arcade { get; set; }
        public bool c8_arcade { get; set; }
        public bool c9_arcade { get; set; }
        public bool c10_arcade { get; set; }
        public bool c11_arcade { get; set; }
        public bool c12_arcade { get; set; }
        public bool c13_arcade { get; set; }
        public bool c14_arcade { get; set; }
        public bool c15_arcade { get; set; }
        public bool c16_arcade { get; set; }
        public bool c17_arcade { get; set; }
        public bool c18_arcade { get; set; }
        public bool c19_arcade { get; set; }
        public bool c20_arcade { get; set; }
        public bool c21_arcade { get; set; }
        public bool c22_arcade { get; set; }
        public bool c23_arcade { get; set; }
        public bool c24_arcade { get; set; }
        public bool c25_arcade { get; set; }
        public bool c26_arcade { get; set; }
        public bool c27_arcade { get; set; }
        public bool c28_arcade { get; set; }
        public bool c29_arcade { get; set; }
        public bool c29_arcade_soon { get; set; }


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
            chk50_story.DataBindings.Add("Checked", this, "c50_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk0_story.DataBindings.Add("Checked", this, "c0_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_story.DataBindings.Add("Checked", this, "c1_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_story.DataBindings.Add("Checked", this, "c2_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_story.DataBindings.Add("Checked", this, "c3_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_story.DataBindings.Add("Checked", this, "c4_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk5_story.DataBindings.Add("Checked", this, "c5_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk6_story.DataBindings.Add("Checked", this, "c6_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_BlueCE.DataBindings.Add("Checked", this, "Kronos_BlueCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_RedCE.DataBindings.Add("Checked", this, "Kronos_RedCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_YellowCE.DataBindings.Add("Checked", this, "Kronos_YellowCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_WhiteCE.DataBindings.Add("Checked", this, "Kronos_WhiteCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_GreenCE.DataBindings.Add("Checked", this, "Kronos_GreenCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos_CyanCE.DataBindings.Add("Checked", this, "Kronos_CyanCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chk50_fishing.DataBindings.Add("Checked", this, "c50_fishing", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_Knuckles.DataBindings.Add("Checked", this, "Ares_Knuckles", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_WyvernFirst.DataBindings.Add("Checked", this, "Ares_WyvernFirst", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_Water.DataBindings.Add("Checked", this, "Ares_Water", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_Crane.DataBindings.Add("Checked", this, "Ares_Crane", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_GreenCE.DataBindings.Add("Checked", this, "Ares_GreenCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_CyanCE.DataBindings.Add("Checked", this, "Ares_CyanCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_WyvernStart.DataBindings.Add("Checked", this, "Ares_WyvernStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_WyvernRun.DataBindings.Add("Checked", this, "Ares_WyvernRun", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_SuperSonic.DataBindings.Add("Checked", this, "Ares_SuperSonic", false, DataSourceUpdateMode.OnPropertyChanged);
            chk51_story.DataBindings.Add("Checked", this, "c51_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk7_story.DataBindings.Add("Checked", this, "c7_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk8_story.DataBindings.Add("Checked", this, "c8_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk9_story.DataBindings.Add("Checked", this, "c9_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk10_story.DataBindings.Add("Checked", this, "c10_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk11_story.DataBindings.Add("Checked", this, "c11_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk12_story.DataBindings.Add("Checked", this, "c12_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk13_story.DataBindings.Add("Checked", this, "c13_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_BlueCE.DataBindings.Add("Checked", this, "Ares_BlueCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_RedCE.DataBindings.Add("Checked", this, "Ares_RedCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_YellowCE.DataBindings.Add("Checked", this, "Ares_YellowCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres_WhiteCE.DataBindings.Add("Checked", this, "Ares_WhiteCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chk51_fishing.DataBindings.Add("Checked", this, "c51_fishing", false, DataSourceUpdateMode.OnPropertyChanged);
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
            chk52_story.DataBindings.Add("Checked", this, "c52_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk52_fishing.DataBindings.Add("Checked", this, "c52_fishing", false, DataSourceUpdateMode.OnPropertyChanged);
            chk14_story.DataBindings.Add("Checked", this, "c14_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk15_story.DataBindings.Add("Checked", this, "c15_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk16_story.DataBindings.Add("Checked", this, "c16_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk17_story.DataBindings.Add("Checked", this, "c17_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk18_story.DataBindings.Add("Checked", this, "c18_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk19_story.DataBindings.Add("Checked", this, "c19_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk20_story.DataBindings.Add("Checked", this, "c20_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower1.DataBindings.Add("Checked", this, "Rhea_Tower1", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower2.DataBindings.Add("Checked", this, "Rhea_Tower2", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower3.DataBindings.Add("Checked", this, "Rhea_Tower3", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower4.DataBindings.Add("Checked", this, "Rhea_Tower4", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower5.DataBindings.Add("Checked", this, "Rhea_Tower5", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea_Tower6.DataBindings.Add("Checked", this, "Rhea_Tower6", false, DataSourceUpdateMode.OnPropertyChanged);
            chk53_story.DataBindings.Add("Checked", this, "c53_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_Bridge.DataBindings.Add("Checked", this, "Ouranos_Bridge", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_SupremeDefeated.DataBindings.Add("Checked", this, "Ouranos_SupremeDefeated", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFinalBoss.DataBindings.Add("Checked", this, "FinalBoss", false, DataSourceUpdateMode.OnPropertyChanged);
            chk21_story.DataBindings.Add("Checked", this, "c21_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk22_story.DataBindings.Add("Checked", this, "c22_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk23_story.DataBindings.Add("Checked", this, "c23_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk24_story.DataBindings.Add("Checked", this, "c24_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk25_story.DataBindings.Add("Checked", this, "c25_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk26_story.DataBindings.Add("Checked", this, "c26_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk27_story.DataBindings.Add("Checked", this, "c27_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk28_story.DataBindings.Add("Checked", this, "c28_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk29_story.DataBindings.Add("Checked", this, "c29_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_BlueCE.DataBindings.Add("Checked", this, "Ouranos_BlueCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_RedCE.DataBindings.Add("Checked", this, "Ouranos_RedCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_GreenCE.DataBindings.Add("Checked", this, "Ouranos_GreenCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_YellowCE.DataBindings.Add("Checked", this, "Ouranos_YellowCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_CyanCE.DataBindings.Add("Checked", this, "Ouranos_CyanCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chkOuranos_WhiteCE.DataBindings.Add("Checked", this, "Ouranos_WhiteCE", false, DataSourceUpdateMode.OnPropertyChanged);
            chk54_fishing.DataBindings.Add("Checked", this, "c54_fishing", false, DataSourceUpdateMode.OnPropertyChanged);
            chk0_arcade.DataBindings.Add("Checked", this, "c0_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_arcade.DataBindings.Add("Checked", this, "c1_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_arcade.DataBindings.Add("Checked", this, "c2_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_arcade.DataBindings.Add("Checked", this, "c3_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_arcade.DataBindings.Add("Checked", this, "c4_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk5_arcade.DataBindings.Add("Checked", this, "c5_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk6_arcade.DataBindings.Add("Checked", this, "c6_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk7_arcade.DataBindings.Add("Checked", this, "c7_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk8_arcade.DataBindings.Add("Checked", this, "c8_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk9_arcade.DataBindings.Add("Checked", this, "c9_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk10_arcade.DataBindings.Add("Checked", this, "c10_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk11_arcade.DataBindings.Add("Checked", this, "c11_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk12_arcade.DataBindings.Add("Checked", this, "c12_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk13_arcade.DataBindings.Add("Checked", this, "c13_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk14_arcade.DataBindings.Add("Checked", this, "c14_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk15_arcade.DataBindings.Add("Checked", this, "c15_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk16_arcade.DataBindings.Add("Checked", this, "c16_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk17_arcade.DataBindings.Add("Checked", this, "c17_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk18_arcade.DataBindings.Add("Checked", this, "c18_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk19_arcade.DataBindings.Add("Checked", this, "c19_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk20_arcade.DataBindings.Add("Checked", this, "c20_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk21_arcade.DataBindings.Add("Checked", this, "c21_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk22_arcade.DataBindings.Add("Checked", this, "c22_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk23_arcade.DataBindings.Add("Checked", this, "c23_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk24_arcade.DataBindings.Add("Checked", this, "c24_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk25_arcade.DataBindings.Add("Checked", this, "c25_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk26_arcade.DataBindings.Add("Checked", this, "c26_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk27_arcade.DataBindings.Add("Checked", this, "c27_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk28_arcade.DataBindings.Add("Checked", this, "c28_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk29_arcade.DataBindings.Add("Checked", this, "c29_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk29_arcade_soon.DataBindings.Add("Checked", this, "c29_arcade_soon", false, DataSourceUpdateMode.OnPropertyChanged);

            // Default Values
            WFocus = false;
            StoryStart = ArcadeStart = Arcade1_1 = true;

            // Kronos
            Kronos_Ninja = Kronos_Door = Kronos_Amy = Kronos_GigantosFirst = Kronos_Tombstones = false;
            c50_story = c50_fishing = true;
            c0_story = c1_story = c2_story = c3_story = c4_story = c50_story = c6_story = false;
            Kronos_BlueCE = Kronos_RedCE = Kronos_YellowCE = Kronos_WhiteCE = Kronos_GreenCE = Kronos_CyanCE  = Kronos_GigantosStart = Kronos_SuperSonic = false;

            // Ares
            Ares_Knuckles = Ares_WyvernFirst = Ares_Water = Ares_Crane = false;
            Ares_WyvernRun = Ares_WyvernStart = Ares_SuperSonic = false;
            c51_story = c51_fishing = true;
            c7_story = c8_story = c9_story = c10_story = c11_story = c12_story = c13_story = false;
            Ares_GreenCE = Ares_CyanCE = Ares_BlueCE = Ares_RedCE = Ares_YellowCE = Ares_YellowCE = false;

            // Chaos
            Chaos_Tails = Chaos_KnightFirst = Chaos_Hacking = false;
            Chaos_GreenCE = Chaos_CyanCE = Chaos_PinballEnd = Chaos_PinballStart = false;
            Chaos_BlueCE = Chaos_RedCE = Chaos_YellowCE = Chaos_WhiteCE = false;
            Chaos_KnightStart = Chaos_SuperSonic = false;
            c52_story = c52_fishing = true;
            c14_story = c15_story = c16_story = c17_story = c18_story = c19_story = c20_story = false;

            // Rhea
            Rhea_Tower1 = Rhea_Tower2 = Rhea_Tower3 = Rhea_Tower4 = Rhea_Tower5 = Rhea_Tower6 = false;
            c53_story = true;

            // Ouranos
            Ouranos_Bridge = Ouranos_SupremeDefeated = false;
            FinalBoss = true;
            c21_story = c22_story = c23_story = c24_story = c25_story = c26_story = c27_story = c28_story = c29_story = false;
            Ouranos_BlueCE = Ouranos_RedCE = Ouranos_GreenCE = Ouranos_YellowCE = Ouranos_CyanCE = Ouranos_WhiteCE = false;
            c54_fishing = true;

            // Arcade mode
            c0_arcade = c1_arcade = c2_arcade = c3_arcade = c4_arcade = c5_arcade = c6_arcade = true;
            c7_arcade = c8_arcade = c9_arcade = c10_arcade = c11_arcade = c12_arcade = c13_arcade = true;
            c14_arcade = c15_arcade = c16_arcade = c17_arcade = c18_arcade = c19_arcade = c20_arcade = true;
            c21_arcade = c22_arcade = c23_arcade = c24_arcade = c25_arcade = c26_arcade = c27_arcade = c28_arcade = c29_arcade = true;
            c29_arcade_soon = true;
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
            settingsNode.AppendChild(ToElement(doc, "c50_story", c50_story));
            settingsNode.AppendChild(ToElement(doc, "c0_story", c0_story));
            settingsNode.AppendChild(ToElement(doc, "c1_story", c1_story));
            settingsNode.AppendChild(ToElement(doc, "c2_story", c2_story));
            settingsNode.AppendChild(ToElement(doc, "c3_story", c3_story));
            settingsNode.AppendChild(ToElement(doc, "c4_story", c4_story));
            settingsNode.AppendChild(ToElement(doc, "c5_story", c5_story));
            settingsNode.AppendChild(ToElement(doc, "c6_story", c6_story));
            settingsNode.AppendChild(ToElement(doc, "Kronos_BlueCE", Kronos_BlueCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_RedCE", Kronos_RedCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_YellowCE", Kronos_YellowCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_WhiteCE", Kronos_WhiteCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_GreenCE", Kronos_GreenCE));
            settingsNode.AppendChild(ToElement(doc, "Kronos_CyanCE", Kronos_CyanCE));
            settingsNode.AppendChild(ToElement(doc, "c50_fishing", c50_fishing));
            settingsNode.AppendChild(ToElement(doc, "Ares_Knuckles", Ares_Knuckles));
            settingsNode.AppendChild(ToElement(doc, "Ares_WyvernFirst", Ares_WyvernFirst));
            settingsNode.AppendChild(ToElement(doc, "Ares_Water", Ares_Water));
            settingsNode.AppendChild(ToElement(doc, "Ares_Crane", Ares_Crane));
            settingsNode.AppendChild(ToElement(doc, "Ares_GreenCE", Ares_GreenCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_CyanCE", Ares_CyanCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_WyvernStart", Ares_WyvernStart));
            settingsNode.AppendChild(ToElement(doc, "Ares_WyvernRun", Ares_WyvernRun));
            settingsNode.AppendChild(ToElement(doc, "Ares_SuperSonic", Ares_SuperSonic));
            settingsNode.AppendChild(ToElement(doc, "c51_story", c51_story));
            settingsNode.AppendChild(ToElement(doc, "c7_story", c7_story));
            settingsNode.AppendChild(ToElement(doc, "c8_story", c8_story));
            settingsNode.AppendChild(ToElement(doc, "c9_story", c9_story));
            settingsNode.AppendChild(ToElement(doc, "c10_story", c10_story));
            settingsNode.AppendChild(ToElement(doc, "c11_story", c11_story));
            settingsNode.AppendChild(ToElement(doc, "c12_story", c12_story));
            settingsNode.AppendChild(ToElement(doc, "c13_story", c13_story));
            settingsNode.AppendChild(ToElement(doc, "Ares_BlueCE", Ares_BlueCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_RedCE", Ares_RedCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_YellowCE", Ares_YellowCE));
            settingsNode.AppendChild(ToElement(doc, "Ares_WhiteCE", Ares_WhiteCE));
            settingsNode.AppendChild(ToElement(doc, "c51_fishing", c51_fishing));
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
            settingsNode.AppendChild(ToElement(doc, "c52_story", c52_story));
            settingsNode.AppendChild(ToElement(doc, "c52_fishing", c52_fishing));
            settingsNode.AppendChild(ToElement(doc, "c14_story", c14_story));
            settingsNode.AppendChild(ToElement(doc, "c15_story", c15_story));
            settingsNode.AppendChild(ToElement(doc, "c16_story", c16_story));
            settingsNode.AppendChild(ToElement(doc, "c17_story", c17_story));
            settingsNode.AppendChild(ToElement(doc, "c18_story", c18_story));
            settingsNode.AppendChild(ToElement(doc, "c19_story", c19_story));
            settingsNode.AppendChild(ToElement(doc, "c20_story", c20_story));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower1", Rhea_Tower1));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower2", Rhea_Tower2));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower3", Rhea_Tower3));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower4", Rhea_Tower4));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower5", Rhea_Tower5));
            settingsNode.AppendChild(ToElement(doc, "Rhea_Tower6", Rhea_Tower6));
            settingsNode.AppendChild(ToElement(doc, "c53_story", c53_story));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_Bridge", Ouranos_Bridge));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_SupremeDefeated", Ouranos_SupremeDefeated));
            settingsNode.AppendChild(ToElement(doc, "FinalBoss", FinalBoss));
            settingsNode.AppendChild(ToElement(doc, "c21_story", c21_story));
            settingsNode.AppendChild(ToElement(doc, "c22_story", c22_story));
            settingsNode.AppendChild(ToElement(doc, "c23_story", c23_story));
            settingsNode.AppendChild(ToElement(doc, "c24_story", c24_story));
            settingsNode.AppendChild(ToElement(doc, "c25_story", c25_story));
            settingsNode.AppendChild(ToElement(doc, "c26_story", c26_story));
            settingsNode.AppendChild(ToElement(doc, "c27_story", c27_story));
            settingsNode.AppendChild(ToElement(doc, "c28_story", c28_story));
            settingsNode.AppendChild(ToElement(doc, "c29_story", c29_story));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_BlueCE", Ouranos_BlueCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_RedCE", Ouranos_RedCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_GreenCE", Ouranos_GreenCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_YellowCE", Ouranos_YellowCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_CyanCE", Ouranos_CyanCE));
            settingsNode.AppendChild(ToElement(doc, "Ouranos_WhiteCE", Ouranos_WhiteCE));
            settingsNode.AppendChild(ToElement(doc, "c54_fishing", c54_fishing));
            settingsNode.AppendChild(ToElement(doc, "c0_arcade", c0_arcade));
            settingsNode.AppendChild(ToElement(doc, "c1_arcade", c1_arcade));
            settingsNode.AppendChild(ToElement(doc, "c2_arcade", c2_arcade));
            settingsNode.AppendChild(ToElement(doc, "c3_arcade", c3_arcade));
            settingsNode.AppendChild(ToElement(doc, "c4_arcade", c4_arcade));
            settingsNode.AppendChild(ToElement(doc, "c5_arcade", c5_arcade));
            settingsNode.AppendChild(ToElement(doc, "c6_arcade", c6_arcade));
            settingsNode.AppendChild(ToElement(doc, "c7_arcade", c7_arcade));
            settingsNode.AppendChild(ToElement(doc, "c8_arcade", c8_arcade));
            settingsNode.AppendChild(ToElement(doc, "c9_arcade", c9_arcade));
            settingsNode.AppendChild(ToElement(doc, "c10_arcade", c10_arcade));
            settingsNode.AppendChild(ToElement(doc, "c11_arcade", c11_arcade));
            settingsNode.AppendChild(ToElement(doc, "c12_arcade", c12_arcade));
            settingsNode.AppendChild(ToElement(doc, "c13_arcade", c13_arcade));
            settingsNode.AppendChild(ToElement(doc, "c14_arcade", c14_arcade));
            settingsNode.AppendChild(ToElement(doc, "c15_arcade", c15_arcade));
            settingsNode.AppendChild(ToElement(doc, "c16_arcade", c16_arcade));
            settingsNode.AppendChild(ToElement(doc, "c17_arcade", c17_arcade));
            settingsNode.AppendChild(ToElement(doc, "c18_arcade", c18_arcade));
            settingsNode.AppendChild(ToElement(doc, "c19_arcade", c19_arcade));
            settingsNode.AppendChild(ToElement(doc, "c20_arcade", c20_arcade));
            settingsNode.AppendChild(ToElement(doc, "c21_arcade", c21_arcade));
            settingsNode.AppendChild(ToElement(doc, "c22_arcade", c22_arcade));
            settingsNode.AppendChild(ToElement(doc, "c23_arcade", c23_arcade));
            settingsNode.AppendChild(ToElement(doc, "c24_arcade", c24_arcade));
            settingsNode.AppendChild(ToElement(doc, "c25_arcade", c25_arcade));
            settingsNode.AppendChild(ToElement(doc, "c26_arcade", c26_arcade));
            settingsNode.AppendChild(ToElement(doc, "c27_arcade", c27_arcade));
            settingsNode.AppendChild(ToElement(doc, "c28_arcade", c28_arcade));
            settingsNode.AppendChild(ToElement(doc, "c29_arcade", c29_arcade));
            settingsNode.AppendChild(ToElement(doc, "c29_arcade_soon", c29_arcade_soon));
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
            c50_story = ParseBool(settings, "c50_story", true);
            c0_story = ParseBool(settings, "c0_story", false);
            c1_story = ParseBool(settings, "c1_story", false);
            c2_story = ParseBool(settings, "c2_story", false);
            c3_story = ParseBool(settings, "c3_story", false);
            c4_story = ParseBool(settings, "c4_story", false);
            c5_story = ParseBool(settings, "c5_story", false);
            c6_story = ParseBool(settings, "c6_story", false);
            Kronos_BlueCE = ParseBool(settings, "Kronos_BlueCE", false);
            Kronos_RedCE = ParseBool(settings, "Kronos_RedCE", false);
            Kronos_YellowCE = ParseBool(settings, "Kronos_YellowCE", false);
            Kronos_WhiteCE = ParseBool(settings, "Kronos_WhiteCE", false);
            Kronos_GreenCE = ParseBool(settings, "Kronos_GreenCE", false);
            Kronos_CyanCE = ParseBool(settings, "Kronos_CyanCE", false);
            c50_fishing = ParseBool(settings, "c50_fishing", true);
            Ares_Knuckles = ParseBool(settings, "Ares_Knuckles", false);
            Ares_WyvernFirst = ParseBool(settings, "Ares_WyvernFirst", false);
            Ares_Water = ParseBool(settings, "Ares_Water", false);
            Ares_Crane = ParseBool(settings, "Ares_Crane", false);
            Ares_GreenCE = ParseBool(settings, "Ares_GreenCE", false);
            Ares_CyanCE = ParseBool(settings, "Ares_CyanCE", false);
            Ares_WyvernStart = ParseBool(settings, "Ares_WyvernStart", false);
            Ares_WyvernRun = ParseBool(settings, "Ares_WyvernRun", false);
            Ares_SuperSonic = ParseBool(settings, "Ares_SuperSonic", false);
            c51_story = ParseBool(settings, "c51_story", true);
            c7_story = ParseBool(settings, "c7_story", false);
            c8_story = ParseBool(settings, "c8_story", false);
            c9_story = ParseBool(settings, "c9_story", false);
            c10_story = ParseBool(settings, "c10_story", false);
            c11_story = ParseBool(settings, "c11_story", false);
            c12_story = ParseBool(settings, "c12_story", false);
            c13_story = ParseBool(settings, "c13_story", false);
            Ares_BlueCE = ParseBool(settings, "Ares_BlueCE", false);
            Ares_RedCE = ParseBool(settings, "Ares_RedCE", false);
            Ares_YellowCE = ParseBool(settings, "Ares_YellowCE", false);
            Ares_WhiteCE = ParseBool(settings, "Ares_WhiteCE", false);
            c51_fishing = ParseBool(settings, "c51_fishing", true);
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
            c52_story = ParseBool(settings, "c52_story", true);
            c52_fishing = ParseBool(settings, "c52_fishing", true);
            c14_story = ParseBool(settings, "c14_story", false);
            c15_story = ParseBool(settings, "c15_story", false);
            c16_story = ParseBool(settings, "c16_story", false);
            c17_story = ParseBool(settings, "c17_story", false);
            c18_story = ParseBool(settings, "c18_story", false);
            c19_story = ParseBool(settings, "c19_story", false);
            c20_story = ParseBool(settings, "c20_story", false);
            Rhea_Tower1 = ParseBool(settings, "Rhea_Tower1", false);
            Rhea_Tower2 = ParseBool(settings, "Rhea_Tower2", false);
            Rhea_Tower3 = ParseBool(settings, "Rhea_Tower3", false);
            Rhea_Tower4 = ParseBool(settings, "Rhea_Tower4", false);
            Rhea_Tower5 = ParseBool(settings, "Rhea_Tower5", false);
            Rhea_Tower6 = ParseBool(settings, "Rhea_Tower6", false);
            c53_story = ParseBool(settings, "c53_story", true);
            Ouranos_Bridge = ParseBool(settings, "Ouranos_Bridge", false);
            Ouranos_SupremeDefeated = ParseBool(settings, "Ouranos_SupremeDefeated", false);
            FinalBoss = ParseBool(settings, "FinalBoss", true);
            c21_story = ParseBool(settings, "c21_story", false);
            c22_story = ParseBool(settings, "c22_story", false);
            c23_story = ParseBool(settings, "c23_story", false);
            c24_story = ParseBool(settings, "c24_story", false);
            c25_story = ParseBool(settings, "c25_story", false);
            c26_story = ParseBool(settings, "c26_story", false);
            c27_story = ParseBool(settings, "c27_story", false);
            c28_story = ParseBool(settings, "c28_story", false);
            c29_story = ParseBool(settings, "c29_story", false);
            Ouranos_BlueCE = ParseBool(settings, "Ouranos_BlueCE", false);
            Ouranos_RedCE = ParseBool(settings, "Ouranos_RedCE", false);
            Ouranos_GreenCE = ParseBool(settings, "Ouranos_GreenCE", false);
            Ouranos_YellowCE = ParseBool(settings, "Ouranos_YellowCE", false);
            Ouranos_CyanCE = ParseBool(settings, "Ouranos_CyanCE", false);
            Ouranos_WhiteCE = ParseBool(settings, "Ouranos_WhiteCE", false);
            c54_fishing = ParseBool(settings, "c54_fishing", true);
            c0_arcade = ParseBool(settings, "c0_arcade", true);
            c1_arcade = ParseBool(settings, "c1_arcade", true);
            c2_arcade = ParseBool(settings, "c2_arcade", true);
            c3_arcade = ParseBool(settings, "c3_arcade", true);
            c4_arcade = ParseBool(settings, "c4_arcade", true);
            c5_arcade = ParseBool(settings, "c5_arcade", true);
            c6_arcade = ParseBool(settings, "c6_arcade", true);
            c7_arcade = ParseBool(settings, "c7_arcade", true);
            c8_arcade = ParseBool(settings, "c8_arcade", true);
            c9_arcade = ParseBool(settings, "c9_arcade", true);
            c10_arcade = ParseBool(settings, "c10_arcade", true);
            c11_arcade = ParseBool(settings, "c11_arcade", true);
            c12_arcade = ParseBool(settings, "c12_arcade", true);
            c13_arcade = ParseBool(settings, "c13_arcade", true);
            c14_arcade = ParseBool(settings, "c14_arcade", true);
            c15_arcade = ParseBool(settings, "c15_arcade", true);
            c16_arcade = ParseBool(settings, "c16_arcade", true);
            c17_arcade = ParseBool(settings, "c17_arcade", true);
            c18_arcade = ParseBool(settings, "c18_arcade", true);
            c19_arcade = ParseBool(settings, "c19_arcade", true);
            c20_arcade = ParseBool(settings, "c20_arcade", true);
            c21_arcade = ParseBool(settings, "c21_arcade", true);
            c22_arcade = ParseBool(settings, "c22_arcade", true);
            c23_arcade = ParseBool(settings, "c23_arcade", true);
            c24_arcade = ParseBool(settings, "c24_arcade", true);
            c25_arcade = ParseBool(settings, "c25_arcade", true);
            c26_arcade = ParseBool(settings, "c26_arcade", true);
            c27_arcade = ParseBool(settings, "c27_arcade", true);
            c28_arcade = ParseBool(settings, "c28_arcade", true);
            c29_arcade = ParseBool(settings, "c29_arcade", true);
            c29_arcade_soon = ParseBool(settings, "c29_arcade_soon", true);
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
