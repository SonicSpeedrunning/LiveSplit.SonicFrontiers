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

        public SonicFrontiersComponent(LiveSplitState state)
        {
            timer = new TimerModel { CurrentState = state };
            Settings = new Settings();

            if (timer.CurrentState.CurrentTimingMethod == TimingMethod.RealTime)
            {
                new Task(() =>
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
                }).Start();
            }
        }

        public override void Dispose()
        {
            this.Settings.Dispose();
            this.watchers = null;
        }

        public override XmlNode GetSettings(XmlDocument document) { return this.Settings.GetSettings(document); }

        public override Control GetSettingsControl(LayoutMode mode) { return this.Settings; }

        public override void SetSettings(XmlNode settings) { this.Settings.SetSettings(settings); }

        public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode)
        {
            this.SplittingLogicUpdate();
        }
    }
}
