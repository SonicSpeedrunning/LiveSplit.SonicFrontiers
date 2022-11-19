using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.SonicFrontiers
{
    class ProcessHook
    {
        // Internal stuff
        private readonly string[] processNames;
        private readonly CancellationTokenSource CancelToken;

        // Properties we probably want to expose
        public Process Game { get; protected set; }
        public bool GotAddresses { get; set; }
        public bool IsGameHooked => Game != null && !Game.HasExited;


        public ProcessHook(params string[] exeNames)
        {
            processNames = exeNames;
            GotAddresses = false;
            CancelToken = new CancellationTokenSource();
            Task.Run(() => TryConnect(), CancelToken.Token);
        }

        private async Task TryConnect()
        {
            while (!CancelToken.IsCancellationRequested)
            {
                foreach (var entry in processNames)
                {
                    Game = Process.GetProcessesByName(entry).OrderByDescending(x => x.StartTime).FirstOrDefault(x => !x.HasExited);
                    if (Game != null)
                    {
                        Game.Exited += CallBackTryConnect;
                        return;
                    }
                }
                await Task.Delay(1500, CancelToken.Token);
            }
        }

        private void CallBackTryConnect(object sender, EventArgs e)
        {
            Game.Exited -= CallBackTryConnect;
            GotAddresses = false;
            Game = null;
            Task.Run(() => TryConnect(), CancelToken.Token);
        }

        public void Dispose()
        {
            CancelToken.Cancel();
            CancelToken.Dispose();
        }
    }
}
