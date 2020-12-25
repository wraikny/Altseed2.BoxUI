using System;
using System.Collections.Generic;

namespace Altseed2.BoxUI
{
    internal interface IPoolHandler
    {
        void Update();
        void Terminate();
    }

    public static class BoxUISystem
    {
        private static readonly List<IPoolHandler> handlers_ = new List<IPoolHandler>();

        private static readonly Queue<Action> posts_ = new Queue<Action>();

        internal static void Register(IPoolHandler handler)
        {
            handlers_.Add(handler);
        }

        /// <summary>
        /// クラスTのプールからブジェクトを取得します。
        /// 取得できなかった場合はnullが返ります。
        /// </summary>
        public static T RentOrNull<T>()
            where T : class => BoxUIPool<T>.Rent();

        /// <summary>
        /// クラスTのプールにオブジェクトを返却します。
        /// </summary>
        public static void Return<T>(T element)
            where T : class => BoxUIPool<T>.Return(element);

        /// <summary>
        /// 更新処理を行います。
        /// Altseed2.Engine.Updateの前に必ず実行してください。
        /// </summary>
        public static void Update()
        {
            while (posts_.TryDequeue(out var action))
            {
                action();
            }

            foreach (var h in handlers_)
            {
                h.Update();
            }
        }

        /// <summary>
        /// 終了処理を行います。
        /// Altseed2.Engine.Terminateの前に必ず実行してください。
        /// </summary>
        public static void Terminate()
        {
            while (posts_.TryDequeue(out var action))
            {
                action();
            }

            foreach(var h in handlers_)
            {
#if DEBUG
                Console.WriteLine($"Terminate: {h.GetType()}");
#endif
                h.Terminate();
            }
        }

        /// <summary>
        /// BoxUISystem.Updateで実行される処理を登録します。
        /// 特定の処理を遅延させる際に利用します。
        /// </summary>
        /// <param name="action"></param>
        public static void Post(Action action)
        {
            if (action is null) return;
            posts_.Enqueue(action);
        }

        /// <summary>
        /// ノードを作成してオブジェクトプールに登録します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void PoolNode<T>(T node)
            where T : Node => NodePool<T>.Register(node);
    }
}
