using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    internal static class NodePool<T>
        where T : Node
    {
        private static bool registered_ = false;

        private static Queue<T> sharedPool_;
        private static Queue<T> sharedRemovedPool_;

        private static Dictionary<BoxUIRootNode, Queue<T>> returnedPool_;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="result"></param>
        /// <returns>取得したノードを子ノードとして追加する必要があるかどうか</returns>
        /// <exception cref="System.ArgumentNullException">引数として与えられたBoxUIRootNodeがnull</exception>
        internal static T Rent(BoxUIRootNode root)
        {
            if (root is null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            if (returnedPool_ is { } && returnedPool_.TryGetValue(root, out var queue))
            {
                if (queue.TryDequeue(out T res))
                {
                    return res;
                }
            }
            else if (sharedPool_ is { } && sharedPool_.TryDequeue(out T res))
            {
                root.AddChildNode(res);
                return res;
            }

            return null;
        }

        internal static void Return(BoxUIRootNode root, T node)
        {
            if (root is null || node is null) return;

            returnedPool_ ??= new Dictionary<BoxUIRootNode, Queue<T>>();

            if (!returnedPool_.TryGetValue(root, out var queue))
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

        internal static void Register(T node)
        {
            sharedPool_ ??= new Queue<T>();
            sharedPool_.Enqueue(node);
        }

        private sealed class NodePoolHandler : INodePoolHandler
        {
            void ReturnToShared(BoxUIRootNode root)
            {
                if (returnedPool_ is null) return;

                if(!returnedPool_.TryGetValue(root, out var pool))
                {
                    return;
                }

                if (pool.Count == 0) return;

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
                ReturnToShared(root);
                returnedPool_?.Remove(root);
            }
        }


        private sealed class PoolHandler : IPoolHandler
        {
            void IPoolHandler.Update()
            {
                if (sharedRemovedPool_ is null || sharedRemovedPool_.Count == 0) return;

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
                returnedPool_ = null;
            }
        }
    }
}
