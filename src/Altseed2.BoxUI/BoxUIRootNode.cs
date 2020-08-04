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

    public sealed class BoxUIRootNode : Node
    {
        private Element element_;
        private readonly List<INodePoolHandler> handlers_;

        public BoxUIRootNode()
        {
            handlers_ = new List<INodePoolHandler>();
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
        }

        public void AddCache<T>(T node)
            where T : Node => NodePool<T>.Return(this, node);

        public T Rent<T>()
            where T : Node
        {
            var elem = NodePool<T>.Rent(this);
            return elem;
        }

        public void Return<T>(T node)
            where T : Node
        {
            NodePool<T>.Return(this, node);
        }

        protected override void OnUpdate()
        {
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
