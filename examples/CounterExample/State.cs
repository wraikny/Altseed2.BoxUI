using System;

namespace CounterExample
{
    sealed class Msg
    {
        private Action<State> _action;
        public Msg(Action<State> action)
        {
            _action = action;
        }

        public void Invoke(State state)
        {
            _action(state);
        }
    }

    sealed class State
    {
        public int Count { get; private set; }

        public static readonly Msg Incr = new Msg(s => s.Count += 1);
        public static readonly Msg Decr = new Msg(s => s.Count -= 1);
    }
}
