using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altseed2.BoxUI
{
    internal static class ElementCacher<T>
        where T : Element
    {
        private static bool registered_ = false;
        private static Stack<T> cache_ = null;
        private static Stack<T> returnedCache_ = null;

        public static T Rent()
        {
            if (cache_ is null) return null;

            if(cache_.TryPop(out T result))
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
            returnedCache_ ??= new Stack<T>();

            returnedCache_.Push(element);
            
            if(!registered_)
            {
                registered_ = true;
                BoxUISystem.Register(new Handler());
            }
        }

        private sealed class Handler : ICacheHandler
        {
            void ICacheHandler.Update()
            {
                if (returnedCache_ != null && returnedCache_.Count != 0)
                {
                    cache_ ??= new Stack<T>();

                    foreach (var item in returnedCache_)
                    {
                        cache_.Push(item);
                    }

                    returnedCache_.Clear();
                }
            }

            void ICacheHandler.Terminate()
            {
                registered_ = false;
                cache_.Clear();
                cache_ = null;
                returnedCache_.Clear();
                returnedCache_ = null;
            }
        }
    }
}
