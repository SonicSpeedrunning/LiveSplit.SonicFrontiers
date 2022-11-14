using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        private Process game;
        private readonly string[] processNames;
        private bool GotAddresses = false;
        public bool IsGameHooked => game != null && !game.HasExited;
        private CancellationTokenSource CancelToken = new CancellationTokenSource();


        public Watchers(string[] n)
        {
            this.processNames = n;
            Task.Run(TryConnect, CancelToken.Token);
        }

        async Task TryConnect()
        {
            while (true)
            {
                foreach (var entry in processNames)
                {
                    game = Process.GetProcessesByName(entry).OrderByDescending(x => x.StartTime).FirstOrDefault(x => !x.HasExited);
                    if (game != null)
                    {
                        game.Exited += CallBackTryConnect;
                        return;
                    }
                }
                await Task.Delay(1500, CancelToken.Token);
            }
        }

        private void CallBackTryConnect(object sender, EventArgs e)
        {
            game.Exited -= CallBackTryConnect;
            GotAddresses = false;
            game = null;
            Task.Run(TryConnect, CancelToken.Token);
        }

        private void CheckPtr(IntPtr o)
        {
            if (o == IntPtr.Zero)
                throw new NullReferenceException("Sigscanning failed!");
        }

        public class FakeMemoryWatcher<T>
        {
            public T Current { get; set; }
            public T Old { get; set; }
            public bool Changed => !this.Old.Equals(this.Current);
            public FakeMemoryWatcher(T old, T current)
            {
                this.Old = old;
                this.Current = current;
            }
        }

        public void Dispose()
        {
            CancelToken.Cancel();
        }
    }
}
