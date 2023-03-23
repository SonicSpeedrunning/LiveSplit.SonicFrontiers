using System.Xml;
using System.Windows.Forms;
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;

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
            watchers = new Watchers(state);

            watchers.WFocusChange += (s, e) => OnWFocusChange(s, Settings.WFocus);
            Settings.WFocusChange += OnWFocusChange;

            if (timer.CurrentState.CurrentTimingMethod == TimingMethod.RealTime)
                AskGameTime();
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
            watchers.WFocusChange -= (s, e) => OnWFocusChange(s, Settings.WFocus);
            Settings.Dispose();
            watchers.Dispose();
        }

        public override XmlNode GetSettings(XmlDocument document) { return this.Settings.GetSettings(document); }

        public override Control GetSettingsControl(LayoutMode mode) { return this.Settings; }

        public override void SetSettings(XmlNode settings) { this.Settings.SetSettings(settings); }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            // If LiveSplit is not connected to the game, of course there's no point in going further
            if (!watchers.Init())
                return;

            // Main update logic is inside the watcher class in order to avoid exposing unneded stuff to the outside
            watchers.Update();

            // Main logic: the autosplitter checks for time, reset and splitting conditions only if it's running
            // This prevents, for example, automatic resetting when the run is already complete (TimerPhase.Ended)
            if (timer.CurrentState.CurrentPhase == TimerPhase.Running || timer.CurrentState.CurrentPhase == TimerPhase.Paused)
            {
                timer.CurrentState.IsGameTimePaused = IsLoading();

                if (GameTime() != null)
                    timer.CurrentState.SetGameTime(GameTime());

                if (Reset())
                    timer.Reset();
                else if (Split())
                    timer.Split();
            }

            // Start logic: for obvious reasons, this should be checked only if the timer has not started yet
            if (timer.CurrentState.CurrentPhase == TimerPhase.NotRunning)
            {
                if (Start())
                {
                    timer.Start();
                }
            }
        }
    }
}