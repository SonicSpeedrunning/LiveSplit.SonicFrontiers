using System;

namespace LiveSplit.SonicFrontiers
{
    public class FakeMemoryWatcher<T>
    {
        protected readonly Func<T> func = null;

        public T Current { get; set; } = default;
        public T Old { get; protected set; } = default;
        public bool Changed => !Old.Equals(Current);

        public FakeMemoryWatcher() { }

        public FakeMemoryWatcher(Func<T> func)
        {
            this.func = func;
        }

        public void Update()
        {
            Old = Current;

            if (func != null)
                Current = func.Invoke();
        }

        public void Update(T newValue)
        {
            Old = Current;
            Current = newValue;
        }
    }
}
