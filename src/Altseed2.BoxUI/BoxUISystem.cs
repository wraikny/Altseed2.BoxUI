using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    internal interface IPoolHandler
    {
        void Update();
        void Terminate();
    }

    public static class BoxUISystem
    {
        private static List<IPoolHandler> handlers_;

        private static Queue<Action> posts_;

        internal static void Register(IPoolHandler handler)
        {
            handlers_ ??= new List<IPoolHandler>();

            handlers_.Add(handler);
        }

        public static void Update()
        {
            if (posts_ != null)
            {
                while (posts_.TryDequeue(out var action))
                {
                    action();
                }
            }

            if (handlers_ != null)
            {
                foreach(var h in handlers_)
                {
                    h.Update();
                }
            }
        }

        public static void Termiante()
        {
            if (posts_ != null)
            {
                while(posts_.TryDequeue(out var action))
                {
                    action();
                }
            }

            if (handlers_ != null)
            {
                foreach(var h in handlers_)
                {
                    h.Terminate();
                }

                handlers_ = null;
            }
        }

        public static void Post(Action action)
        {
            if (action is null) return;
            posts_ ??= new Queue<Action>();
            posts_.Enqueue(action);
        }
    }
}
