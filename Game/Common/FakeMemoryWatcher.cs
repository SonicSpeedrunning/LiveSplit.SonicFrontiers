namespace LiveSplit.SonicFrontiers
{
    public class FakeMemoryWatcher<T>
    {
        public T Current { get; set; }
        public T Old { get; protected set; }
        public bool Changed => !Old.Equals(Current);

        public FakeMemoryWatcher(T old, T current)
        {
            Old = old;
            Current = current;
        }

        public FakeMemoryWatcher()
        {
            Old = default;
            Current = default;
        }

        public FakeMemoryWatcher(T val)
        {
            Old = val;
            Current = val;
        }

        public void Update()
        {
            Old = Current;
        }
    }
}
