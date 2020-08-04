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

        internal static void Register(IPoolHandler handler)
        {
            handlers_ ??= new List<IPoolHandler>();
            handlers_.Add(handler);
        }

        public static void Update()
        {
            foreach(var h in handlers_)
            {
                h.Update();
            }
        }

        public static void Termiante()
        {
            foreach(var h in handlers_)
            {
                h.Terminate();
            }

            handlers_ = null;
        }
    }
}
