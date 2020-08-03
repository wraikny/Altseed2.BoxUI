using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    internal static class NodeCacher<T>
        where T : Node
    {
        private static bool registered_ = false;
        private static Dictionary<WeakReference<BoxUIRootNode>, Stack<T>> availableCache_;
        private static Dictionary<WeakReference<BoxUIRootNode>, Stack<T>> returnedCache_;

        public static T Rent(BoxUIRootNode root)
        {
            if (availableCache_ is null) return null;
            if (availableCache_.TryGetValue(new WeakReference<BoxUIRootNode>(root), out var cache))
            {
                if (cache.TryPop(out T result))
                {
                    return result;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        internal static void Return(BoxUIRootNode root, T node)
        {
            var weak = new WeakReference<BoxUIRootNode>(root);

            returnedCache_ ??= new Dictionary<WeakReference<BoxUIRootNode>, Stack<T>>();

            if(returnedCache_.TryGetValue(weak, out var cache))
            {
                cache.Push(node);
            }
            else
            {
                var stack = new Stack<T>();
                stack.Push(node);
                returnedCache_[weak] = stack;
                root.AddChildNode(new HandlerNode(root));
            }

            if (!registered_)
            {
                registered_ = true;
                BoxUISystem.Register(new Handler());
            }
        }

        private class HandlerNode : Node
        {
            private readonly WeakReference<BoxUIRootNode> root_;

            public HandlerNode(BoxUIRootNode root)
            {
                root_ = new WeakReference<BoxUIRootNode>(root);
            }

            protected override void OnUpdate()
            {
                var returnedCache = returnedCache_[root_];

                availableCache_ ??= new Dictionary<WeakReference<BoxUIRootNode>, Stack<T>>();

                if (!availableCache_.TryGetValue(root_, out Stack<T> cache))
                {
                    cache = new Stack<T>();
                    availableCache_[root_] = cache;
                }

                foreach (var item in returnedCache)
                {
                    cache.Push(item);
                }

                returnedCache.Clear();
            }

            protected override void OnRemoved()
            {
                returnedCache_.Remove(root_);
                availableCache_?.Remove(root_);
                Parent.RemoveChildNode(this);
            }
        }


        private sealed class Handler : ICacheHandler
        {
            void ICacheHandler.Update() { }

            void ICacheHandler.Terminate()
            {
                registered_ = false;
                availableCache_.Clear();
                availableCache_ = null;
                returnedCache_.Clear();
                returnedCache_ = null;
            }
        }
    }
}
