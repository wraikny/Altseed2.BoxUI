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
        Vector2F contentSize_;

        public static SpriteElement Create(bool keepAspect = true, Action<SpriteNode> initializer = null)
        {
            var elem = Rent<SpriteElement>();
            elem.keepAspect_ = keepAspect;
            elem.initializer_ = initializer;
            return elem;
        }

        protected override void ReturnToCache()
        {
            Root.Return(node_);
            Return(this);
            node_ = null;
            contentSize_ = default;
        }

        protected override void OnAdded()
        {
            node_ = Root.Rent<SpriteNode>();
            initializer_?.Invoke(node_);
            initializer_ = null;
            contentSize_ = node_.ContentSize;
        }

        public override Vector2F CalcSize(Vector2F _) => contentSize_;

        protected override void OnResize(RectF area)
        {
            var pos = area.Position;

            if(node_.Texture != null)
            {
                var srcSize = node_.ContentSize;
                var scale = area.Size / srcSize;
                if(keepAspect_)
                {
                    node_.Scale = Vector2FExt.One * MathF.Min(scale.X, scale.Y);
                    var size = srcSize * node_.Scale;
                    pos += (area.Size + size) * 0.5f;

                    area = new RectF(node_.Position, size);

                    contentSize_ = size;
                }

                node_.Scale = scale;
            }

            node_.Position = pos;

            foreach (var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
