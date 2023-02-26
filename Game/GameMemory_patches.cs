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
            if (GameProcess.InitStatus == GameInitStatus.NotStarted)
                return;

            if (enabled)
                GameProcess.Game.WriteValue<byte>(addresses["baseFocus"], 0xEB);
            else
                GameProcess.Game.WriteValue<byte>(addresses["baseFocus"], 0x74);
        }
    }
}
