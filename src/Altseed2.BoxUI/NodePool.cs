using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    internal static class NodePool<T>
        where T : Node, new()
    {
        private static bool registered_ = false;

        private static Queue<T> sharedPool_;
        private static Queue<T> sharedRemovedPool_;

        private static Dictionary<BoxUIRootNode, Queue<T>> returnedPool_;

        public static T Rent(BoxUIRootNode root)
        {
            if (root is null) return null;

            if(returnedPool_ != null && returnedPool_.TryGetValue(root, out var queue))
            {
                if(queue.TryDequeue(out T res))
                {
                    return res;
                }
            }
            else if(sharedPool_ != null && sharedPool_.TryDequeue(out T res))
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

            returnedPool_ ??= new Dictionary<BoxUIRootNode, Queue<T>>();

            if(!returnedPool_.TryGetValue(root, out var queue))
            {
                queue = new Queue<T>();
                returnedPool_[root] = queue;
                root.RegisterHandler(new NodePoolHandler());
            }

            queue.Enqueue(node);

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

                sharedRemovedPool_ ??= new Queue<T>();

                foreach (var item in pool)
                {
                    root.RemoveChildNode(item);
                    sharedRemovedPool_.Enqueue(item);
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

                sharedPool_ ??= new Queue<T>();

                foreach (var item in sharedRemovedPool_)
                {
                    sharedPool_.Enqueue(item);
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
