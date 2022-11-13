using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace LiveSplit.SonicFrontiers
{
    partial class Watchers
    {
        // Common variable that are likely to be used in every game
        private ManagementEventWatcher[] _watcher;
        private Process game;
        private bool GotAddresses = false;
        public bool IsGameHooked => game != null && !game.HasExited;


        public Watchers()
        {
            AddManagementWatchers();
            TryConnect();
        }

        private void CallBackAddManagementWatchers(object sender, EventArgs e)
        {
            game.Exited -= CallBackAddManagementWatchers;
            GotAddresses = false;
            game = null;
            AddManagementWatchers();
        }

        private void AddManagementWatchers()
        {
            _watcher = new ManagementEventWatcher[processNames.Length];

            for (int i = 0; i < _watcher.Length; i++)
            {
                _watcher[i] = new ManagementEventWatcher(new WqlEventQuery($"SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process' AND TargetInstance.Name = '{processNames[i]}.exe'"));
                _watcher[i].EventArrived += CallBackTryConnect;
                _watcher[i].Start();
            }
        }

        private void CallBackTryConnect(object sender, EventArgs e)
        {
            for (int i = 0; i < _watcher.Length; i++)
            {
                _watcher[i].Stop();
                _watcher[i].EventArrived -= CallBackTryConnect;
                _watcher[i].Dispose();
            }
            _watcher = null;
            TryConnect();
        }

        private void TryConnect()
        {
            foreach (var entry in processNames)
            {
                game = Process.GetProcessesByName(entry).OrderByDescending(x => x.StartTime).FirstOrDefault(x => !x.HasExited);
                if (game != null)
                {
                    game.Exited += CallBackAddManagementWatchers;

                    if (_watcher != null)
                    {
                        for (int i = 0; i < _watcher.Length; i++)
                        {
                            _watcher[i].Stop();
                            _watcher[i].Dispose();
                        }
                        _watcher = null;
                    }
                    return;
                }
            }
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
    }
}
