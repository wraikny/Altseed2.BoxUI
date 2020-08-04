using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class SpriteElement : Element
    {
        bool keepAspect_;
        Action<SpriteNode> initializer_;
        SpriteNode node_;

        public SpriteNode Node => node_;

        private SpriteElement() { }

        public static SpriteElement Create(bool keepAspect = true, Action<SpriteNode> initializer = null)
        {
            var elem = RentOrNull<SpriteElement>() ?? new SpriteElement();
            elem.keepAspect_ = keepAspect;
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
            node_ = Root.RentOrCreate<SpriteNode>();
            initializer_?.Invoke(node_);
            initializer_ = null;
        }

        public override Vector2F CalcSize(Vector2F size)
        {
            if (node_.Texture is null) return Vector2FExt.Zero;

            var srcSize = node_.ContentSize;
            var scale = size / srcSize;
            if (keepAspect_)
            {
                return srcSize * Vector2FExt.One * MathF.Min(scale.X, scale.Y);
            }

            return size;
        }

        protected override void OnResize(RectF area)
        {
            var pos = area.Position;
            var size = CalcSize(area.Size);

            area = new RectF(pos, size);

            node_.Position = pos;
            node_.Scale = size / node_.ContentSize;

            foreach (var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
