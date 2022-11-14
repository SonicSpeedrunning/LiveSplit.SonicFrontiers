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
        private Watchers watchers;


        public SonicFrontiersComponent(LiveSplitState state)
        {
            timer = new TimerModel { CurrentState = state };
            Settings = new Settings();

            string[] processNames = new string[] { "SonicFrontiers" };
            watchers = new Watchers(processNames);

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

        public override void Dispose()
        {
            Settings.Dispose();
            watchers.Dispose();
        }

        public override XmlNode GetSettings(XmlDocument document) { return this.Settings.GetSettings(document); }

        public override Control GetSettingsControl(LayoutMode mode) { return this.Settings; }

        public override void SetSettings(XmlNode settings) { this.Settings.SetSettings(settings); }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            if (!watchers.IsGameHooked)
                return;

            watchers.Update();
            LogicUpdate();
            timer.CurrentState.IsGameTimePaused = IsLoading();

            switch (timer.CurrentState.CurrentPhase)
            {
                case TimerPhase.NotRunning:
                    if (Start()) timer.Start();
                    break;
                case TimerPhase.Running:
                    if (Reset()) timer.Reset();
                    else if (Split()) timer.Split();
                    break;
                case TimerPhase.Paused:
                    if (Reset()) timer.Reset();
                    break;
            }
        }
    }
}
