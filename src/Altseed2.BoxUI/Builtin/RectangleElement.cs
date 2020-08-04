using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class RectangleElement<T> : Element
        where T : RectangleNode, new()
    {
        Action<T> initializer_;
        T node_;

        public static RectangleElement<T> Create(Action<T> initializer)
        {
            var elem = Rent<RectangleElement<T>>();
            elem.initializer_ = initializer;
            return elem;
        }

        protected override void ReturnToCache()
        {
            Root.Return(node_);
            Return(this);
            node_ = null;
        }

        protected override void OnAdded()
        {
            node_ = Root.Rent<T>() ?? new T();
            initializer_?.Invoke(node_);
            initializer_ = null;
        }

        protected override void OnResize(RectF area)
        {
            node_.Position = area.Position;
            node_.RectangleSize = area.Size;

            foreach(var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
