using System;
using System.Collections.Generic;
using System.Text;

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
        private readonly List<INodePoolHandler> handlers_;

        public IList<IBoxUICursor> Cursors => cursors_;

        public BoxUIRootNode()
        {
            isUpdating_ = false;

            handlers_ = new List<INodePoolHandler>();
            cursors_ = new List<IBoxUICursor>();
        }

        /// <summary>
        /// BoxUIRootNodeの更新中（例えばボタンのクリック時など）に呼び出したい場合は、BoXUISystem.Postを利用して、メソッドの呼び出しを遅延してください。
        /// </summary>
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
        /// BoxUIRootNodeの更新中（例えばボタンのクリック時など）に呼び出したい場合は、BoXUISystem.Postを利用して、メソッドの呼び出しを遅延してください。
        /// </summary>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void SetElement<T>(T absoluteSizeElement)
            where T : Element, IAbsoluteSizeElement
        {
            if (isUpdating_)
            {
                throw new InvalidOperationException();
            }

            element_?.Clear();
            element_ = absoluteSizeElement;
            absoluteSizeElement.Root = this;
            absoluteSizeElement.Added();
            absoluteSizeElement.Resize();
        }

        /// <summary>
        /// クラスTのプールからオブジェクトを取得します。
        /// 取得できなかった場合はnew()コンストラクタによって新しいインスタンスを作成します。
        /// 同一フレーム内で再利用されたノードの場合、BoxUIRootNodeの子ノードとして追加されています。
        /// それ以外では自動的にBoxUIRootNodeの子ノードとして追加されます。
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
        /// 同一フレーム内で再利用されたノードの場合、BoxUIRootNodeの子ノードとして追加されています。
        /// それ以外では自動的にBoxUIRootNodeの子ノードとして追加されます。
        /// </summary>
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
        /// 同一フレーム内で再利用される場合、BoxUIRootNodeの子ノードから削除されません。
        /// それ以外の場合は、自動的にBoxUIRootNodeの子ノードから削除されます。
        /// </summary>
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
            foreach (var h in handlers_)
            {
                h.OnRemoved(this);
            }
        }

        internal void RegisterHandler(INodePoolHandler handler)
        {
            handlers_.Add(handler);
        }
    }
}
