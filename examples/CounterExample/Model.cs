using System;

namespace CounterExample
{
    interface IMsg
    {
        void Update(State state);
    }

    sealed class State
    {
        public int Count { get; private set; }

        public static readonly IMsg Incr = new Msg(s => s.Count += 1);
        public static readonly IMsg Decr = new Msg(s => s.Count -= 1);

        class Msg : IMsg
        {
            private Action<State> _action;
            public Msg(Action<State> action)
            {
                _action = action;
            }

            void IMsg.Update(State state) => _action(state);
        }
    }
}
