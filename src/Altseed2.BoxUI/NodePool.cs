using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    internal static class NodePool<T>
        where T : Node, new()
    {
        private static bool registered_ = false;

        private static Stack<T> sharedPool_;
        private static Stack<T> sharedRemovedPool_;

        private static Dictionary<BoxUIRootNode, Stack<T>> returnedPool_;

        public static T Rent(BoxUIRootNode root)
        {
            if (root is null) return;

            if(returnedPool_ != null && returnedPool_.TryGetValue(root, out var stack))
            {
                if(stack.TryPop(out T res))
                {
                    return res;
                }
            }
            else if(sharedPool_ != null && sharedPool_.TryPop(out T res))
            {
                root.AddChildNode(res);
                return res;
            }

            var node = new T();
            root.AddChildNode(node);

            return node;
        }

        internal static void Return(BoxUIRootNode root, T node)
        {
            if (root is null || node is null) return;

            returnedPool_ ??= new Dictionary<BoxUIRootNode, Stack<T>>();

            if(!returnedPool_.TryGetValue(root, out var stack))
            {
                stack = new Stack<T>();
                returnedPool_[root] = stack;
                root.RegisterHandler(new NodePoolHandler());
            }

            stack.Push(node);

            if (!registered_)
            {
                registered_ = true;
                BoxUISystem.Register(new PoolHandler());
            }
        }

        private sealed class NodePoolHandler : INodePoolHandler
        {
            void ReturnToShared(BoxUIRootNode root)
            {
                var pool = returnedPool_[root];

                sharedRemovedPool_ ??= new Stack<T>();

                foreach (var item in pool)
                {
                    root.RemoveChildNode(item);
                    sharedRemovedPool_.Push(item);
                }

                pool.Clear();
            }

            void INodePoolHandler.OnUpdate(BoxUIRootNode root)
            {
                ReturnToShared(root);
            }

            void INodePoolHandler.OnRemoved(BoxUIRootNode root)
            {
                returnedPool_?.Remove(root);
                ReturnToShared(root);
            }
        }


        private sealed class PoolHandler : IPoolHandler
        {
            void IPoolHandler.Update()
            {
                if (sharedRemovedPool_ is null) return;

                sharedPool_ ??= new Stack<T>();

                foreach (var item in sharedRemovedPool_)
                {
                    sharedPool_.Push(item);
                }

                sharedRemovedPool_.Clear();
            }

            void IPoolHandler.Terminate()
            {
                registered_ = false;
                sharedPool_ = null;
                sharedRemovedPool_ = null;
                sharedPool_ = null;
            }
        }
    }
}
