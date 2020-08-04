using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class RectangleElement : Element
    {
        Action<RectangleNode> initializer_;
        RectangleNode node_;

        public RectangleNode Node => node_;

        private RectangleElement() { }

        public static RectangleElement Create(Action<RectangleNode> initializer)
        {
            var elem = RentOrNull<RectangleElement>() ?? new RectangleElement();
            elem.initializer_ = initializer;
            return elem;
        }

        protected override void ReturnToCache()
        {
            Root.Return(node_);
            node_ = null;
            Return(this);
        }

        protected override void OnAdded()
        {
            node_ = Root.RentOrCreate<RectangleNode>();
            initializer_?.Invoke(node_);
            initializer_ = null;
        }

        public override Vector2F CalcSize(Vector2F _) => node_.ContentSize;

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
