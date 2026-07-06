using LiveSplit.Model;
using LiveSplit.Options;
using LiveSplit.SonicFrontiers;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.IO;
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
			catch (OperationCanceledException e) {
				File.WriteAllText("LiveSplit.SonicFrontiers.ErrorLog.txt", "Cancelled intentionally\n" + e.StackTrace + "\n" + e.Message);
				Log.Error("Something broke :(\n" + e.StackTrace + "\n" + e.Message);
			}
			catch (Exception e) {
                File.WriteAllText("LiveSplit.SonicFrontiers.ErrorLog.txt", "Something broke :(\n" + e.StackTrace + "\n" + e.Message);
				Log.Error("Something broke :(\n" + e.StackTrace + "\n" + e.Message);
			}
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