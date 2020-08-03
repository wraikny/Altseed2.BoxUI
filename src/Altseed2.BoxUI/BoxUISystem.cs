using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    internal interface ICacheHandler
    {
        void Update();
        void Terminate();
    }

    public static class BoxUISystem
    {
        private static List<ICacheHandler> handlers_;

        internal static void Register(ICacheHandler handler)
        {
            handlers_ ??= new List<ICacheHandler>();
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
