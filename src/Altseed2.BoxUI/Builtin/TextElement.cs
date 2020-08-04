using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class TextElement : Element
    {
        TextNode node_;
        Action<TextNode> initializer_;

        public TextNode Node => node_;

        public static TextElement Create(Action<TextNode> initializer)
        {
            var elem = Rent<TextElement>();
            elem.initializer_ = initializer;
            return elem;
        }

        protected override void ReturnToCache()
        {
            Root.Return(node_);
            node_ = null;
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F _) => node_.ContentSize;

        protected override void OnAdded()
        {
            node_ = Root.Rent<TextNode>();
            initializer_?.Invoke(node_);
            initializer_ = null;
        }

        protected override void OnResize(RectF area)
        {
            node_.Position = area.Position;
        }
    }
}
