using System.Collections.Generic;

namespace Altseed2.BoxUI
{
    internal static class BoxUIPool<T>
        where T : class
    {
        private static bool registered_ = false;
        private static Queue<T> pool_ = null;

        public static T Rent()
        {
            if (pool_ is null) return null;

            if(pool_.TryDequeue(out T result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }

        public static void Return(T element)
        {
            if (element is null) return;

            pool_ ??= new Queue<T>();

            pool_.Enqueue(element);
            
            if(!registered_)
            {
                registered_ = true;
                BoxUISystem.Register(new Handler());
            }
        }

        private sealed class Handler : IPoolHandler
        {
            void IPoolHandler.Update() { }

            void IPoolHandler.Terminate()
            {
                registered_ = false;
                pool_.Clear();
                pool_ = null;
            }
        }
    }
}
