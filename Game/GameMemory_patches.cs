using System;
using LiveSplit.ComponentUtil;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        public event EventHandler WFocusChange;

        // Enable or disable the WFocus patch, depending on your settings
        public void ScreenFocus(bool enabled)
        {
            if (GameProcess.InitStatus == GameInitStatus.NotStarted) return;
            if (enabled) GameProcess.Game.WriteValue<byte>(addresses["baseFocus"], 0xEB);
            if (!enabled) GameProcess.Game.WriteValue<byte>(addresses["baseFocus"], 0x74);
        }

        /// <summary>
        /// Check if the previous split occurred under a certain threshold, provided as milliseconds. This check if performed regardless of the split being triggered by the autosplitter or by manual intervention.
        /// </summary>
        /// <param name="milliseconds"></param>
        /// <returns>True if the previous split occurred over the provided threshold value. False otherwise (or if LiveSplit did not split yet).</returns>
        public bool IsLastSplitBelowValue(double milliseconds)
        {
            if (state.CurrentSplitIndex == 0)
                return false;

            var this_splittime = state.CurrentSplit.SplitTime.RealTime.Value;
            var last_splittime = state.Run[state.CurrentSplitIndex - 1].SplitTime.RealTime.Value;

            return this_splittime - last_splittime <= TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}
