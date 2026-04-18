using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using LiveSplit.SonicFrontiers;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.SonicFrontiers;

internal partial class SonicFrontiersComponent : LogicComponent
{
    private Settings Settings { get; set; } = new();

    public SonicFrontiersComponent(LiveSplitState state)
    {
        autosplitterTask = Task.Run(() =>
        {
            try
            {
                AutosplitterLogic(state, cancelToken.Token);
            }
            catch { }
        });

        if (state.CurrentTimingMethod == TimingMethod.RealTime)
            AskGameTime(state);
    }

    public override void Dispose()
    {
        cancelToken.Cancel();
        autosplitterTask?.Wait();
        autosplitterTask?.Dispose();
        Settings?.Dispose();
    }

    public override XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);
    public override Control GetSettingsControl(LayoutMode mode) => Settings;
    public override void SetSettings(XmlNode settings) => Settings.SetSettings(settings);
    public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }

    private void AskGameTime(LiveSplitState state)
    {
        var timingMessage = MessageBox.Show(
            "This autosplitter supports Time without Loads (Game Time).\n" +
            "LiveSplit is currently set to show Real Time (RTA).\n" +
            "Would you like to set the timing method to Game Time?",
            "LiveSplit - Sonic Frontiers",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question
        );

        if (timingMessage == DialogResult.Yes)
            state.CurrentTimingMethod = TimingMethod.GameTime;
    }
}