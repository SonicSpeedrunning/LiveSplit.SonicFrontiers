using System;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.SonicFrontiers
{
    public partial class Settings : UserControl
    {
        // General
        public bool StoryStart { get; set; }
        public bool ArcadeStart { get; set; }
        public bool Arcade1_1 { get; set; }
        public bool Kronos { get; set; }
        public bool Ares { get; set; }
        public bool Chaos { get; set; }
        public bool Rhea { get; set; }
        public bool FinalBoss { get; set; }
        public bool KronosFirst { get; set; }
        public bool AresFirst { get; set; }
        public bool ChaosFirst { get; set; }
        public bool RheaFirst { get; set; }


        // Story mode
        public bool w6d01_story { get; set; }
        public bool w8d01_story { get; set; }
        public bool w9d04_story { get; set; }
        public bool w6d02_story { get; set; }
        public bool w7d04_story { get; set; }
        public bool w6d06_story { get; set; }
        public bool w9d06_story { get; set; }
        public bool w6d05_story { get; set; }
        public bool w8d03_story { get; set; }
        public bool w7d02_story { get; set; }
        public bool w7d06_story { get; set; }
        public bool w8d04_story { get; set; }
        public bool w6d03_story { get; set; }
        public bool w8d05_story { get; set; }
        public bool w6d04_story { get; set; }
        public bool w6d08_story { get; set; }
        public bool w8d02_story { get; set; }
        public bool w6d09_story { get; set; }
        public bool w6d07_story { get; set; }
        public bool w8d06_story { get; set; }
        public bool w7d03_story { get; set; }
        public bool w7d08_story { get; set; }
        public bool w9d02_story { get; set; }
        public bool w7d01_story { get; set; }
        public bool w9d03_story { get; set; }
        public bool w6d10_story { get; set; }
        public bool w7d07_story { get; set; }
        public bool w9d05_story { get; set; }
        public bool w7d05_story { get; set; }
        public bool w9d07_story { get; set; }


        // Arcade mode
        public bool w6d01_arcade { get; set; }
        public bool w8d01_arcade { get; set; }
        public bool w9d04_arcade { get; set; }
        public bool w6d02_arcade { get; set; }
        public bool w7d04_arcade { get; set; }
        public bool w6d06_arcade { get; set; }
        public bool w9d06_arcade { get; set; }
        public bool w6d05_arcade { get; set; }
        public bool w8d03_arcade { get; set; }
        public bool w7d02_arcade { get; set; }
        public bool w7d06_arcade { get; set; }
        public bool w8d04_arcade { get; set; }
        public bool w6d03_arcade { get; set; }
        public bool w8d05_arcade { get; set; }
        public bool w6d04_arcade { get; set; }
        public bool w6d08_arcade { get; set; }
        public bool w8d02_arcade { get; set; }
        public bool w6d09_arcade { get; set; }
        public bool w6d07_arcade { get; set; }
        public bool w8d06_arcade { get; set; }
        public bool w7d03_arcade { get; set; }
        public bool w7d08_arcade { get; set; }
        public bool w9d02_arcade { get; set; }
        public bool w7d01_arcade { get; set; }
        public bool w9d03_arcade { get; set; }
        public bool w6d10_arcade { get; set; }
        public bool w7d07_arcade { get; set; }
        public bool w9d05_arcade { get; set; }
        public bool w7d05_arcade { get; set; }
        public bool w9d07_arcade { get; set; }
        public bool w9d07_arcade_soon { get; set; }


        // Fishing
        public bool Fishing { get; set; }
        public bool w1r03_fish { get; set; }
        public bool w2r01_fish { get; set; }
        public bool w3r01_fish { get; set; }
        public bool w1r04_fish { get; set; }
        public bool w1r03_fish_first { get; set; }
        public bool w2r01_fish_first { get; set; }
        public bool w3r01_fish_first { get; set; }
        public bool w1r04_fish_first { get; set; }



        public Settings()
        {
            InitializeComponent();
            label6.Text = "Autosplitter version: v" + System.Diagnostics.FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion;

            // General settings
            chkStoryStart.DataBindings.Add("Checked", this, "StoryStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkArcadeStart.DataBindings.Add("Checked", this, "ArcadeStart", false, DataSourceUpdateMode.OnPropertyChanged);
            chkArcade1_1.DataBindings.Add("Checked", this, "Arcade1_1", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronos.DataBindings.Add("Checked", this, "Kronos", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAres.DataBindings.Add("Checked", this, "Ares", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaos.DataBindings.Add("Checked", this, "Chaos", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRhea.DataBindings.Add("Checked", this, "Rhea", false, DataSourceUpdateMode.OnPropertyChanged);
            chkKronosFirst.DataBindings.Add("Checked", this, "KronosFirst", false, DataSourceUpdateMode.OnPropertyChanged);
            chkAresFirst.DataBindings.Add("Checked", this, "AresFirst", false, DataSourceUpdateMode.OnPropertyChanged);
            chkChaosFirst.DataBindings.Add("Checked", this, "ChaosFirst", false, DataSourceUpdateMode.OnPropertyChanged);
            chkRheaFirst.DataBindings.Add("Checked", this, "RheaFirst", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFinalBoss.DataBindings.Add("Checked", this, "FinalBoss", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_1_story.DataBindings.Add("Checked", this, "w6d01_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_2_story.DataBindings.Add("Checked", this, "w8d01_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_3_story.DataBindings.Add("Checked", this, "w9d04_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_4_story.DataBindings.Add("Checked", this, "w6d02_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_5_story.DataBindings.Add("Checked", this, "w7d04_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_6_story.DataBindings.Add("Checked", this, "w6d06_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_7_story.DataBindings.Add("Checked", this, "w9d06_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_1_story.DataBindings.Add("Checked", this, "w6d05_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_2_story.DataBindings.Add("Checked", this, "w8d03_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_3_story.DataBindings.Add("Checked", this, "w7d02_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_4_story.DataBindings.Add("Checked", this, "w7d06_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_5_story.DataBindings.Add("Checked", this, "w8d04_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_6_story.DataBindings.Add("Checked", this, "w6d03_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_7_story.DataBindings.Add("Checked", this, "w8d05_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_1_story.DataBindings.Add("Checked", this, "w6d04_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_2_story.DataBindings.Add("Checked", this, "w6d08_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_3_story.DataBindings.Add("Checked", this, "w8d02_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_4_story.DataBindings.Add("Checked", this, "w6d09_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_5_story.DataBindings.Add("Checked", this, "w6d07_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_6_story.DataBindings.Add("Checked", this, "w8d06_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_7_story.DataBindings.Add("Checked", this, "w7d03_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_1_story.DataBindings.Add("Checked", this, "w7d08_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_2_story.DataBindings.Add("Checked", this, "w9d02_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_3_story.DataBindings.Add("Checked", this, "w7d01_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_4_story.DataBindings.Add("Checked", this, "w9d03_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_5_story.DataBindings.Add("Checked", this, "w6d10_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_6_story.DataBindings.Add("Checked", this, "w7d07_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_7_story.DataBindings.Add("Checked", this, "w9d05_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_8_story.DataBindings.Add("Checked", this, "w7d05_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_9_story.DataBindings.Add("Checked", this, "w9d07_story", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_1_arcade.DataBindings.Add("Checked", this, "w6d01_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_2_arcade.DataBindings.Add("Checked", this, "w8d01_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_3_arcade.DataBindings.Add("Checked", this, "w9d04_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_4_arcade.DataBindings.Add("Checked", this, "w6d02_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_5_arcade.DataBindings.Add("Checked", this, "w7d04_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_6_arcade.DataBindings.Add("Checked", this, "w6d06_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk1_7_arcade.DataBindings.Add("Checked", this, "w9d06_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_1_arcade.DataBindings.Add("Checked", this, "w6d05_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_2_arcade.DataBindings.Add("Checked", this, "w8d03_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_3_arcade.DataBindings.Add("Checked", this, "w7d02_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_4_arcade.DataBindings.Add("Checked", this, "w7d06_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_5_arcade.DataBindings.Add("Checked", this, "w8d04_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_6_arcade.DataBindings.Add("Checked", this, "w6d03_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk2_7_arcade.DataBindings.Add("Checked", this, "w8d05_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_1_arcade.DataBindings.Add("Checked", this, "w6d04_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_2_arcade.DataBindings.Add("Checked", this, "w6d08_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_3_arcade.DataBindings.Add("Checked", this, "w8d02_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_4_arcade.DataBindings.Add("Checked", this, "w6d09_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_5_arcade.DataBindings.Add("Checked", this, "w6d07_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_6_arcade.DataBindings.Add("Checked", this, "w8d06_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk3_7_arcade.DataBindings.Add("Checked", this, "w7d03_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_1_arcade.DataBindings.Add("Checked", this, "w7d08_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_2_arcade.DataBindings.Add("Checked", this, "w9d02_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_3_arcade.DataBindings.Add("Checked", this, "w7d01_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_4_arcade.DataBindings.Add("Checked", this, "w9d03_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_5_arcade.DataBindings.Add("Checked", this, "w6d10_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_6_arcade.DataBindings.Add("Checked", this, "w7d07_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_7_arcade.DataBindings.Add("Checked", this, "w9d05_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_8_arcade.DataBindings.Add("Checked", this, "w7d05_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_9_arcade.DataBindings.Add("Checked", this, "w9d07_arcade", false, DataSourceUpdateMode.OnPropertyChanged);
            chk4_9_arcade_soon.DataBindings.Add("Checked", this, "w9d07_arcade_soon", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishing.DataBindings.Add("Checked", this, "Fishing", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishingKronos.DataBindings.Add("Checked", this, "w1r03_fish", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishingAres.DataBindings.Add("Checked", this, "w2r01_fish", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishingChaos.DataBindings.Add("Checked", this, "w3r01_fish", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishingOuranos.DataBindings.Add("Checked", this, "w1r04_fish", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishingKronosFirst.DataBindings.Add("Checked", this, "w1r03_fish_first", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishingAresFirst.DataBindings.Add("Checked", this, "w2r01_fish_first", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishingChaosFirst.DataBindings.Add("Checked", this, "w3r01_fish_first", false, DataSourceUpdateMode.OnPropertyChanged);
            chkFishingOuranosFirst.DataBindings.Add("Checked", this, "w1r04_fish_first", false, DataSourceUpdateMode.OnPropertyChanged);


            // Default Values
            StoryStart = ArcadeStart = Arcade1_1 = true;
            Kronos = Ares = Chaos = Rhea = FinalBoss = true;
            KronosFirst = AresFirst = ChaosFirst = RheaFirst = true;

            // Cyber Space - story mode
            w6d01_story = w8d01_story = w9d04_story = w6d02_story = w7d04_story = w6d06_story = w9d06_story = false;
            w6d05_story = w8d03_story = w7d02_story = w7d06_story = w8d04_story = w6d03_story = w8d05_story = false;
            w6d04_story = w6d08_story = w8d02_story = w6d09_story = w6d07_story = w8d06_story = w7d03_story = false;
            w7d08_story = w9d02_story = w7d01_story = w9d03_story = w6d10_story = w7d07_story = w9d05_story = w7d05_story = w9d07_story = false;

            // Cyber Space - arcade mode
            w6d01_arcade = w8d01_arcade = w9d04_arcade = w6d02_arcade = w7d04_arcade = w6d06_arcade = w9d06_arcade = true;
            w6d05_arcade = w8d03_arcade = w7d02_arcade = w7d06_arcade = w8d04_arcade = w6d03_arcade = w8d05_arcade = true;
            w6d04_arcade = w6d08_arcade = w8d02_arcade = w6d09_arcade = w6d07_arcade = w8d06_arcade = w7d03_arcade = true;
            w7d08_arcade = w9d02_arcade = w7d01_arcade = w9d03_arcade = w6d10_arcade = w7d07_arcade = w9d05_arcade = w7d05_arcade = w9d07_arcade = w9d07_arcade_soon = true;

            // Fishing
            Fishing = true;
            w1r03_fish = w2r01_fish = w3r01_fish = w1r04_fish = true;
            w1r03_fish_first = w2r01_fish_first = w3r01_fish_first = w1r04_fish_first = true;
        }

        public XmlNode GetSettings(XmlDocument doc)
        {
            XmlElement settingsNode = doc.CreateElement("Settings");
            settingsNode.AppendChild(ToElement(doc, "StoryStart", StoryStart));
            settingsNode.AppendChild(ToElement(doc, "ArcadeStart", ArcadeStart));
            settingsNode.AppendChild(ToElement(doc, "Arcade1_1", Arcade1_1));
            settingsNode.AppendChild(ToElement(doc, "Kronos", Kronos));
            settingsNode.AppendChild(ToElement(doc, "Ares", Ares));
            settingsNode.AppendChild(ToElement(doc, "Chaos", Chaos));
            settingsNode.AppendChild(ToElement(doc, "Rhea", Rhea));
            settingsNode.AppendChild(ToElement(doc, "KronosFirst", KronosFirst));
            settingsNode.AppendChild(ToElement(doc, "AresFirst", AresFirst));
            settingsNode.AppendChild(ToElement(doc, "ChaosFirst", ChaosFirst));
            settingsNode.AppendChild(ToElement(doc, "RheaFirst", RheaFirst));
            settingsNode.AppendChild(ToElement(doc, "FinalBoss", FinalBoss));
            settingsNode.AppendChild(ToElement(doc, "w6d01_story", w6d01_story));
            settingsNode.AppendChild(ToElement(doc, "w8d01_story", w8d01_story));
            settingsNode.AppendChild(ToElement(doc, "w9d04_story", w9d04_story));
            settingsNode.AppendChild(ToElement(doc, "w6d02_story", w6d02_story));
            settingsNode.AppendChild(ToElement(doc, "w7d04_story", w7d04_story));
            settingsNode.AppendChild(ToElement(doc, "w6d06_story", w6d06_story));
            settingsNode.AppendChild(ToElement(doc, "w9d06_story", w9d06_story));
            settingsNode.AppendChild(ToElement(doc, "w6d05_story", w6d05_story));
            settingsNode.AppendChild(ToElement(doc, "w8d03_story", w8d03_story));
            settingsNode.AppendChild(ToElement(doc, "w7d02_story", w7d02_story));
            settingsNode.AppendChild(ToElement(doc, "w7d06_story", w7d06_story));
            settingsNode.AppendChild(ToElement(doc, "w8d04_story", w8d04_story));
            settingsNode.AppendChild(ToElement(doc, "w6d03_story", w6d03_story));
            settingsNode.AppendChild(ToElement(doc, "w8d05_story", w8d05_story));
            settingsNode.AppendChild(ToElement(doc, "w6d04_story", w6d04_story));
            settingsNode.AppendChild(ToElement(doc, "w6d08_story", w6d08_story));
            settingsNode.AppendChild(ToElement(doc, "w8d02_story", w8d02_story));
            settingsNode.AppendChild(ToElement(doc, "w6d09_story", w6d09_story));
            settingsNode.AppendChild(ToElement(doc, "w6d07_story", w6d07_story));
            settingsNode.AppendChild(ToElement(doc, "w8d06_story", w8d06_story));
            settingsNode.AppendChild(ToElement(doc, "w7d03_story", w7d03_story));
            settingsNode.AppendChild(ToElement(doc, "w7d08_story", w7d08_story));
            settingsNode.AppendChild(ToElement(doc, "w9d02_story", w9d02_story));
            settingsNode.AppendChild(ToElement(doc, "w7d01_story", w7d01_story));
            settingsNode.AppendChild(ToElement(doc, "w9d03_story", w9d03_story));
            settingsNode.AppendChild(ToElement(doc, "w6d10_story", w6d10_story));
            settingsNode.AppendChild(ToElement(doc, "w7d07_story", w7d07_story));
            settingsNode.AppendChild(ToElement(doc, "w9d05_story", w9d05_story));
            settingsNode.AppendChild(ToElement(doc, "w7d05_story", w7d05_story));
            settingsNode.AppendChild(ToElement(doc, "w9d07_story", w9d07_story));
            settingsNode.AppendChild(ToElement(doc, "w6d01_arcade", w6d01_arcade));
            settingsNode.AppendChild(ToElement(doc, "w8d01_arcade", w8d01_arcade));
            settingsNode.AppendChild(ToElement(doc, "w9d04_arcade", w9d04_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d02_arcade", w6d02_arcade));
            settingsNode.AppendChild(ToElement(doc, "w7d04_arcade", w7d04_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d06_arcade", w6d06_arcade));
            settingsNode.AppendChild(ToElement(doc, "w9d06_arcade", w9d06_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d05_arcade", w6d05_arcade));
            settingsNode.AppendChild(ToElement(doc, "w8d03_arcade", w8d03_arcade));
            settingsNode.AppendChild(ToElement(doc, "w7d02_arcade", w7d02_arcade));
            settingsNode.AppendChild(ToElement(doc, "w7d06_arcade", w7d06_arcade));
            settingsNode.AppendChild(ToElement(doc, "w8d04_arcade", w8d04_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d03_arcade", w6d03_arcade));
            settingsNode.AppendChild(ToElement(doc, "w8d05_arcade", w8d05_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d04_arcade", w6d04_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d08_arcade", w6d08_arcade));
            settingsNode.AppendChild(ToElement(doc, "w8d02_arcade", w8d02_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d09_arcade", w6d09_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d07_arcade", w6d07_arcade));
            settingsNode.AppendChild(ToElement(doc, "w8d06_arcade", w8d06_arcade));
            settingsNode.AppendChild(ToElement(doc, "w7d03_arcade", w7d03_arcade));
            settingsNode.AppendChild(ToElement(doc, "w7d08_arcade", w7d08_arcade));
            settingsNode.AppendChild(ToElement(doc, "w9d02_arcade", w9d02_arcade));
            settingsNode.AppendChild(ToElement(doc, "w7d01_arcade", w7d01_arcade));
            settingsNode.AppendChild(ToElement(doc, "w9d03_arcade", w9d03_arcade));
            settingsNode.AppendChild(ToElement(doc, "w6d10_arcade", w6d10_arcade));
            settingsNode.AppendChild(ToElement(doc, "w7d07_arcade", w7d07_arcade));
            settingsNode.AppendChild(ToElement(doc, "w9d05_arcade", w9d05_arcade));
            settingsNode.AppendChild(ToElement(doc, "w7d05_arcade", w7d05_arcade));
            settingsNode.AppendChild(ToElement(doc, "w9d07_arcade", w9d07_arcade));
            settingsNode.AppendChild(ToElement(doc, "w9d07_arcade_soon", w9d07_arcade_soon));
            settingsNode.AppendChild(ToElement(doc, "Fishing", Fishing));
            settingsNode.AppendChild(ToElement(doc, "w1r03_fish", w1r03_fish));
            settingsNode.AppendChild(ToElement(doc, "w2r01_fish", w2r01_fish));
            settingsNode.AppendChild(ToElement(doc, "w3r01_fish", w3r01_fish));
            settingsNode.AppendChild(ToElement(doc, "w1r04_fish", w1r04_fish));
            settingsNode.AppendChild(ToElement(doc, "w1r03_fish_first", w1r03_fish_first));
            settingsNode.AppendChild(ToElement(doc, "w1r03_fish_first", w1r03_fish_first));
            settingsNode.AppendChild(ToElement(doc, "w1r03_fish_first", w1r03_fish_first));
            settingsNode.AppendChild(ToElement(doc, "w1r03_fish_first", w1r03_fish_first));
            return settingsNode;
        }

        public void SetSettings(XmlNode settings)
        {
            StoryStart = ParseBool(settings, "StoryStart", true);
            ArcadeStart = ParseBool(settings, "ArcadeStart", true);
            Arcade1_1 = ParseBool(settings, "Arcade1_1", true);
            Kronos = ParseBool(settings, "Kronos", true);
            Ares = ParseBool(settings, "Ares", true);
            Chaos = ParseBool(settings, "Chaos", true);
            Rhea = ParseBool(settings, "Rhea", true);
            KronosFirst = ParseBool(settings, "KronosFirst", true);
            AresFirst = ParseBool(settings, "AresFirst", true);
            ChaosFirst = ParseBool(settings, "ChaosFirst", true);
            RheaFirst = ParseBool(settings, "RheaFirst", true);
            FinalBoss = ParseBool(settings, "FinalBoss", true);
            w6d01_story = ParseBool(settings, "w6d01_story", false);
            w8d01_story = ParseBool(settings, "w8d01_story", false);
            w9d04_story = ParseBool(settings, "w9d04_story", false);
            w6d02_story = ParseBool(settings, "w6d02_story", false);
            w7d04_story = ParseBool(settings, "w7d04_story", false);
            w6d06_story = ParseBool(settings, "w6d06_story", false);
            w9d06_story = ParseBool(settings, "w9d06_story", false);
            w6d05_story = ParseBool(settings, "w6d05_story", false);
            w8d03_story = ParseBool(settings, "w8d03_story", false);
            w7d02_story = ParseBool(settings, "w7d02_story", false);
            w7d06_story = ParseBool(settings, "w7d06_story", false);
            w8d04_story = ParseBool(settings, "w8d04_story", false);
            w6d03_story = ParseBool(settings, "w6d03_story", false);
            w8d05_story = ParseBool(settings, "w8d05_story", false);
            w6d04_story = ParseBool(settings, "w6d04_story", false);
            w6d08_story = ParseBool(settings, "w6d08_story", false);
            w8d02_story = ParseBool(settings, "w8d02_story", false);
            w6d09_story = ParseBool(settings, "w6d09_story", false);
            w6d07_story = ParseBool(settings, "w6d07_story", false);
            w8d06_story = ParseBool(settings, "w8d06_story", false);
            w7d03_story = ParseBool(settings, "w7d03_story", false);
            w7d08_story = ParseBool(settings, "w7d08_story", false);
            w9d02_story = ParseBool(settings, "w9d02_story", false);
            w7d01_story = ParseBool(settings, "w7d01_story", false);
            w9d03_story = ParseBool(settings, "w9d03_story", false);
            w6d10_story = ParseBool(settings, "w6d10_story", false);
            w7d07_story = ParseBool(settings, "w7d07_story", false);
            w9d05_story = ParseBool(settings, "w9d05_story", false);
            w7d05_story = ParseBool(settings, "w7d05_story", false);
            w9d07_story = ParseBool(settings, "w9d07_story", false);
            w6d01_arcade = ParseBool(settings, "w6d01_arcade", true);
            w8d01_arcade = ParseBool(settings, "w8d01_arcade", true);
            w9d04_arcade = ParseBool(settings, "w9d04_arcade", true);
            w6d02_arcade = ParseBool(settings, "w6d02_arcade", true);
            w7d04_arcade = ParseBool(settings, "w7d04_arcade", true);
            w6d06_arcade = ParseBool(settings, "w6d06_arcade", true);
            w9d06_arcade = ParseBool(settings, "w9d06_arcade", true);
            w6d05_arcade = ParseBool(settings, "w6d05_arcade", true);
            w8d03_arcade = ParseBool(settings, "w8d03_arcade", true);
            w7d02_arcade = ParseBool(settings, "w7d02_arcade", true);
            w7d06_arcade = ParseBool(settings, "w7d06_arcade", true);
            w8d04_arcade = ParseBool(settings, "w8d04_arcade", true);
            w6d03_arcade = ParseBool(settings, "w6d03_arcade", true);
            w8d05_arcade = ParseBool(settings, "w8d05_arcade", true);
            w6d04_arcade = ParseBool(settings, "w6d04_arcade", true);
            w6d08_arcade = ParseBool(settings, "w6d08_arcade", true);
            w8d02_arcade = ParseBool(settings, "w8d02_arcade", true);
            w6d09_arcade = ParseBool(settings, "w6d09_arcade", true);
            w6d07_arcade = ParseBool(settings, "w6d07_arcade", true);
            w8d06_arcade = ParseBool(settings, "w8d06_arcade", true);
            w7d03_arcade = ParseBool(settings, "w7d03_arcade", true);
            w7d08_arcade = ParseBool(settings, "w7d08_arcade", true);
            w9d02_arcade = ParseBool(settings, "w9d02_arcade", true);
            w7d01_arcade = ParseBool(settings, "w7d01_arcade", true);
            w9d03_arcade = ParseBool(settings, "w9d03_arcade", true);
            w6d10_arcade = ParseBool(settings, "w6d10_arcade", true);
            w7d07_arcade = ParseBool(settings, "w7d07_arcade", true);
            w9d05_arcade = ParseBool(settings, "w9d05_arcade", true);
            w7d05_arcade = ParseBool(settings, "w7d05_arcade", true);
            w9d07_arcade = ParseBool(settings, "w9d07_arcade", true);
            w9d07_arcade_soon = ParseBool(settings, "w9d07_arcade_soon", true);
        }

        static bool ParseBool(XmlNode settings, string setting, bool default_ = false)
        {
            return settings[setting] != null ? (Boolean.TryParse(settings[setting].InnerText, out bool val) ? val : default_) : default_;
        }

        static XmlElement ToElement<T>(XmlDocument document, string name, T value)
        {
            XmlElement str = document.CreateElement(name);
            str.InnerText = value.ToString();
            return str;
        }

        private void ChkArcadeStart_CheckedChanged(object sender, EventArgs e)
        {
            this.chkArcade1_1.Enabled = this.chkArcadeStart.Checked;
        }

        public bool GetSetting(string v)
        {
            bool t;
            try
            {
                t = (bool)this.GetType().GetProperty(v).GetValue(this, null);
            }
            catch
            {
                return false;
            }
            return t;
        }

        private void Chk4_9_arcade_CheckedChanged(object sender, EventArgs e)
        {
            chk4_9_arcade_soon.Enabled = chk4_9_arcade.Checked;
        }

        private void ChkKronos_CheckedChanged(object sender, EventArgs e)
        {
            chkKronosFirst.Enabled = chkKronos.Checked;
        }

        private void ChkAres_CheckedChanged(object sender, EventArgs e)
        {
            chkAresFirst.Enabled = chkAres.Checked;
        }

        private void ChkChaos_CheckedChanged(object sender, EventArgs e)
        {
            chkChaosFirst.Enabled = chkChaos.Checked;
        }

        private void ChkRhea_CheckedChanged(object sender, EventArgs e)
        {
            chkRheaFirst.Enabled = chkRhea.Checked;
        }

        private void chkFishing_CheckedChanged(object sender, EventArgs e)
        {
            chkFishingAres.Enabled = chkFishing.Checked;
            chkFishingChaos.Enabled = chkFishing.Checked;
            chkFishingKronos.Enabled = chkFishing.Checked;
            chkFishingOuranos.Enabled = chkFishing.Checked;
            chkFishingKronosFirst.Enabled = chkFishing.Checked && chkFishingKronos.Checked;
            chkFishingAresFirst.Enabled = chkFishing.Checked && chkFishingAres.Checked;
            chkFishingChaosFirst.Enabled = chkFishing.Checked && chkFishingChaos.Checked;
            chkFishingOuranosFirst.Enabled = chkFishing.Checked && chkFishingOuranos.Checked;
        }
    }
}
