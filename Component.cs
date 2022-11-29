using System.Xml;
using System.Windows.Forms;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System.Threading.Tasks;

namespace LiveSplit.SonicFrontiers
{
    partial class SonicFrontiersComponent : LogicComponent
    {
        public override string ComponentName => "Sonic Frontiers - Autosplitter";
        private Settings Settings { get; set; }
        private readonly TimerModel timer;
        private readonly Watchers watchers;


        public SonicFrontiersComponent(LiveSplitState state)
        {
            timer = new TimerModel { CurrentState = state };
            Settings = new Settings();
            watchers = new Watchers(state, "SonicFrontiers");

            watchers.WFocusChange += (s, e) => OnWFocusChange(s, Settings.WFocus);
            Settings.WFocusChange += OnWFocusChange;

            if (timer.CurrentState.CurrentTimingMethod == TimingMethod.RealTime)
                Task.Run(AskGameTime);
        }

        private void AskGameTime()
        {
            var timingMessage = MessageBox.Show(
                "This autosplitter supports Time without Loads (Game Time).\n" +
                "LiveSplit is currently set to show Real Time (RTA).\n" +
                "Would you like to set the timing method to Game Time?",
                "LiveSplit - Sonic Frontiers",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question
            );

            if (timingMessage == DialogResult.Yes)
                timer.CurrentState.CurrentTimingMethod = TimingMethod.GameTime;
        }

        public void OnWFocusChange(object sender, bool b)
        {
            watchers.ScreenFocus(b);
        }

        public override void Dispose()
        {
            Settings.WFocusChange -= OnWFocusChange;
            watchers.WFocusChange -= (s, e) => OnWFocusChange(s, Settings.WFocus); ;
            Settings.Dispose();
            watchers.Dispose();
        }

        public override XmlNode GetSettings(XmlDocument document) { return this.Settings.GetSettings(document); }

        public override Control GetSettingsControl(LayoutMode mode) { return this.Settings; }

        public override void SetSettings(XmlNode settings) { this.Settings.SetSettings(settings); }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            // If LiveSplit is not connected to the game, of course there's no point in going further
            if (!watchers.Init()) return;

            // Main update logic is inside the watcher class inorder to expose unneded stuff to the outside
            watchers.Update();

            switch (timer.CurrentState.CurrentPhase)
            {
                case TimerPhase.NotRunning:
                    if (Start()) timer.Start();
                    break;
                case TimerPhase.Running:
                case TimerPhase.Paused:
                    timer.CurrentState.IsGameTimePaused = IsLoading();
                    if (GameTime() == null ? false : true) timer.CurrentState.SetGameTime(GameTime());
                    if (Reset()) timer.Reset();
                    else if (Split()) timer.Split();
                    break;
            }
        }
    }
}
