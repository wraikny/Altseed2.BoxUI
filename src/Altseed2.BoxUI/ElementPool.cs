﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altseed2.BoxUI
{
    internal static class ElementPool<T>
        where T : Element, new()
    {
        private static bool registered_ = false;
        private static Stack<T> pool_ = null;

        public static T Rent()
        {
            if (pool_ is null) return new T();

            if(pool_.TryPop(out T result))
            {
                return result;
            }
            else
            {
                return new T();
            }
        }

        public static void Return(T element)
        {
            pool_ ??= new Stack<T>();

            pool_.Push(element);
            
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