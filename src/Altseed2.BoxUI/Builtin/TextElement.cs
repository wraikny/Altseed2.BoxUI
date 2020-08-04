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

        private TextElement() { }

        public static TextElement Create(Action<TextNode> initializer)
        {
            var elem = RentOrNull<TextElement>() ?? new TextElement();
            elem.initializer_ = initializer;
            return elem;
        }

        protected override void ReturnToPool()
        {
            Root.Return(node_);
            node_ = null;
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F _) => node_.ContentSize;

        protected override void OnAdded()
        {
            node_ = Root.RentOrCreate<TextNode>();
            initializer_?.Invoke(node_);
            initializer_ = null;
        }

        protected override void OnResize(RectF area)
        {
            node_.Position = area.Position;
        }
    }
}
