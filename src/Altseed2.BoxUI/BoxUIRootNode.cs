using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    public sealed class BoxUIRootNode : Node
    {
        private Element element_;

        public void Clear()
        {
            element_.Clear();
            element_ = null;
        }

        public void UpdateElement(Element element)
        {
            element_?.Clear();
            element_ = element;
            element.Root = this;
            element.Added();
        }

        public void AddCache<T>(T node)
            where T : Node => NodeCacher<T>.Return(this, node);

        public T Rent<T>()
            where T : Node
        {
            var elem = NodeCacher<T>.Rent(this);
            AddChildNode(elem);
            return elem;
        }

        public void Return<T>(T node)
            where T : Node
        {
            RemoveChildNode(node);
            NodeCacher<T>.Return(this, node);
        }

        protected override void OnUpdate()
        {
            element_?.Update();
        }
    }
}
