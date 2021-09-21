using System;
using System.Collections.Generic;

namespace Altseed2.BoxUI
{
    internal interface INodePoolHandler
    {
        void OnUpdate(BoxUIRootNode root);

        void OnRemoved(BoxUIRootNode root);
    }

    [Serializable]
    public sealed class BoxUIRootNode : TransformNode
    {
        private bool isUpdating_;

        private Element element_;
        private readonly List<IBoxUICursor> cursors_;

        [NonSerialized]
        private readonly List<INodePoolHandler> handlers_;

        public IList<IBoxUICursor> Cursors => cursors_;

        /// <summary>
        /// 自身がエンジンから登録解除される前に自動的に<see cref="Terminate()"/>を呼び出す。
        /// </summary>
        public bool IsAutoTerminated { get; private set; }

        public BoxUIRootNode(bool isAutoTerminated = true)
        {
            IsAutoTerminated = isAutoTerminated;
            isUpdating_ = false;

            handlers_ = new List<INodePoolHandler>();
            cursors_ = new List<IBoxUICursor>();
        }

        /// <summary>
        /// <see cref="ElementRoot"/>の登録を解除して、プールに返却します。
        /// </summary>
        /// <remarks>
        /// この<see cref="BoxUIRootNode"/>の更新中（例えばボタンのクリック時など）に呼び出したい場合は、<see cref="BoxUISystem.Post(Action)"/>を利用して、メソッドの呼び出しを遅延してください。
        /// </remarks>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void ClearElement()
        {
            if (isUpdating_)
            {
                throw new InvalidOperationException();
            }

            element_?.Clear();
            element_ = null;
        }

        /// <summary>
        /// <see cref="ElementRoot"/>を登録します。
        /// </summary>
        /// <remarks>
        /// この<see cref="BoxUIRootNode"/>に登録するための新規<see cref="Element"/>のCreateを行う前に、<see cref="ClearElement()"/>メソッドを呼び出してください。
        /// <see cref="BoxUIRootNode"/>の更新中（例えばボタンのクリック時など）に呼び出したい場合は、<see cref="BoxUISystem.Post(Action)"/>を利用して、メソッドの呼び出しを遅延してください。
        /// </remarks>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetElement(ElementRoot elementRoot)
        {
            if (isUpdating_)
            {
                throw new InvalidOperationException("このBoxUIRootNodeの更新中にElementを更新することはできません。");
            }

            if (elementRoot.Root is { })
            {
                throw new InvalidOperationException("追加済みのElementを追加しようとしました。");
            }


            element_?.Clear();
            element_ = elementRoot;
            elementRoot.Added(this);
            elementRoot.CallSetSize();
            FlushQueue();
        }

        /// <summary>
        /// クラスTのプールからオブジェクトを取得します。
        /// 取得できなかった場合はnew()コンストラクタによって新しいインスタンスを作成します。
        /// 同一フレーム内で再利用されたノードの場合、<see cref="BoxUIRootNode"/>の子ノードとして追加されています。
        /// それ以外では自動的に<see cref="BoxUIRootNode">の子ノードとして追加されます。
        /// </summary>
        public T RentOrCreate<T>()
            where T : Node, new()
        {
            var node = NodePool<T>.Rent(this);

            if (node is null)
            {
                node = new T();
                AddChildNode(node);
            }

            return node;
        }

        /// <summary>
        /// クラスTのプールからオブジェクトを取得します。
        /// 取得できなかった場合は引数のinitializeによって新しいインスタンスを作成します。
        /// </summary>
        /// <remarks>
        /// 同一フレーム内で再利用されたノードの場合、<see cref="BoxUIRootNode"/>の子ノードとして追加されています。
        /// それ以外では自動的に<see cref="BoxUIRootNode">の子ノードとして追加されます。
        /// </remarks>
        public T RentOrCreate<T>(Func<T> initialize)
            where T : Node
        {
            var node = NodePool<T>.Rent(this);

            if (node is null)
            {
                if (initialize is null)
                {
                    throw new ArgumentNullException(nameof(initialize));
                }

                node = initialize();
                AddChildNode(node);
            }

            return node;
        }

        /// <summary>
        /// クラスTのプールにオブジェクトを返却します。
        /// </summary>
        /// <remarks>
        /// 同一フレーム内で再利用される場合、<see cref="BoxUIRootNode"/>の子ノードから削除されません。
        /// それ以外の場合は、自動的に<see cref="BoxUIRootNode"/>の子ノードから削除されます。
        /// </remarks>
        public void Return<T>(T node)
            where T : Node => NodePool<T>.Return(this, node);

        protected override void OnUpdate()
        {
            isUpdating_ = true;

            element_?.Update();

            foreach(var h in handlers_)
            {
                h.OnUpdate(this);
            }

            isUpdating_ = false;
        }

        protected override void OnRemoved()
        {
            if (IsAutoTerminated)
            {
                Terminate();
            }
        }

        /// <summary>
        /// 自身に紐ついたオブジェクトの参照を切り離し、プールに返却する。
        /// </summary>
        public void Terminate()
        {
#if DEBUG
            Console.WriteLine($"BoxUIRootNode: Terminate");
#endif

            element_?.Clear();
            element_ = null;
            foreach (var h in handlers_)
            {
                h.OnRemoved(this);
            }
            handlers_.Clear();
            FlushQueue();
        }

        internal void RegisterHandler(INodePoolHandler handler)
        {
#if DEBUG
            Console.WriteLine($"BoxUIRootNode: RegisterHandler({handler.GetType()})");
#endif
            handlers_.Add(handler);
        }
    }
}
