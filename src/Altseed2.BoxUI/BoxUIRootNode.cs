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

    public sealed class BoxUIRootNode : TransformNode
    {
        private Element element_;
        private readonly List<IBoxUICursor> cursors_;
        private readonly List<INodePoolHandler> handlers_;

        public IList<IBoxUICursor> Cursors => cursors_;

        public BoxUIRootNode()
        {
            handlers_ = new List<INodePoolHandler>();
            cursors_ = new List<IBoxUICursor>();
        }

        public void ClearElement()
        {
            element_?.Clear();
            element_ = null;
        }

        public void SetElement(Element element)
        {
            element_?.Clear();
            element_ = element;
            element.Root = this;
            element.Added();
            element.RequireResize();
        }

        public void RegisterPool<T>(int count)
            where T : Node, new()
        {
            if (count < 1) return;

            for(int i = 0; i < count; i++)
            {
                NodePool<T>.Return(this, new T());
            }
        }

        /// <summary>
        /// クラスT : Nodeのプールからオブジェクトを取得します。
        /// 取得できなかった場合はnew()コンストラクタによって新しいインスタンスを作成します。
        /// このメソッドによって取得されたノードは自動的にBoxUIRootNodeの子ノードとして追加されます。
        /// </summary>
        public T RentOrCreate<T>()
            where T : Node, new() => NodePool<T>.Rent(this);

        /// <summary>
        /// T型のプールからT型のNodeを取得します。
        /// 取得できなかった場合はnew()コンストラクタによって新しいインスタンスを作成します。
        /// このメソッドによって取得されたノードは自動的にBoxUIRootNodeの子ノードから削除されます。
        /// </summary>
        public void Return<T>(T node)
            where T : Node, new() => NodePool<T>.Return(this, node);

        protected override void OnUpdate()
        {
            foreach (var cursor in cursors_)
            {
                cursor.Update();
            }

            element_?.Update();

            foreach(var h in handlers_)
            {
                h.OnUpdate(this);
            }
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
