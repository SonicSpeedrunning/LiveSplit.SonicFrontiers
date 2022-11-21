using System;
using System.Linq;
using System.Diagnostics;
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
        public GameInitStatus InitStatus { get; set; }
        public bool IsGameHooked => Game != null && !Game.HasExited;


        public ProcessHook(params string[] exeNames)
        {
            processNames = exeNames;
            InitStatus = GameInitStatus.NotStarted;
            CancelToken = new CancellationTokenSource();
            Task.Run(TryConnect, CancelToken.Token);
        }

        private void TryConnect()
        {
            while (!CancelToken.IsCancellationRequested)
            {
                foreach (var entry in processNames)
                {
                    Game = Process.GetProcessesByName(entry).OrderByDescending(p => p.StartTime).FirstOrDefault(p => !p.HasExited);
                    if (Game != null)
                    {
                        Game.Exited += CallBackTryConnect;
                        return;
                    }
                }
                Task.Delay(1500, CancelToken.Token).Wait();
            }
        }

        private void CallBackTryConnect(object sender, EventArgs e)
        {
            Game.Exited -= CallBackTryConnect;
            InitStatus = GameInitStatus.NotStarted;
            Game?.Dispose();
            Game = null;
            Task.Run(() => TryConnect(), CancelToken.Token);
        }

        public void Dispose()
        {
            CancelToken.Cancel();
            CancelToken.Dispose();
            Game?.Dispose();
        }
    }

    enum GameInitStatus
    {
        NotStarted,
        InProgress,
        Completed
    }
}
