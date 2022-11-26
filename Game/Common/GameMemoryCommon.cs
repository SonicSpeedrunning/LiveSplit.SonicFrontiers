using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiveSplit.Model;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        // Game process
        private readonly ProcessHook GameProcess;
        private readonly LiveSplitState state;

        // Game offsets
        private readonly Dictionary<string, int> offsets;

        public bool Init()
        {
            // This "init" function checks if the autosplitter has connected to the game
            // (if it has not, there's no point in going further) and starts a Task to
            // get the needed memory addresses for the other methods.
            if (!GameProcess.IsGameHooked)
                return false;

            // The purpose of this task is to limit the update cycle to 1 every 1.5 seconds
            // (instead of the usual one every 16 msec) in order to avoid wasting resources
            if (GameProcess.InitStatus == GameInitStatus.NotStarted)
                Task.Run(() =>
                {
                    GameProcess.InitStatus = GameInitStatus.InProgress;
                    try
                    {
                        GetAddresses();
                        GameProcess.InitStatus = GameInitStatus.Completed;
                    }
                    catch
                    {
                        Task.Delay(1500).Wait();
                        GameProcess.InitStatus = GameInitStatus.NotStarted;
                    }
                    // I'm running this manually because the signature scanner, especially
                    // if it runs several times, can take A LOT of memory, to the point of
                    // filling your RAM with several GB of useless data that doesn't get
                    // collected for some reason.
                    GC.Collect();
                });

            // At this point, if init has not been completed yet, return
            // false to avoid running the rest of the splitting logic.
            if (GameProcess.InitStatus != GameInitStatus.Completed)
                return false;

            return true;
        }
        public void Dispose()
        {
            GameProcess.Dispose();
        }
    }
}
