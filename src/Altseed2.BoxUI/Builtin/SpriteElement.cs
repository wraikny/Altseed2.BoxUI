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
        }

        protected override void OnAdded()
        {
            node_ = Root.Rent<SpriteNode>();
            initializer_?.Invoke(node_);
            initializer_ = null;
        }

        protected override void OnResize(RectF area)
        {
            node_.Position = area.Position;

            if(node_.Texture != null)
            {
                var texSize = node_.Texture.Size;
                node_.Scale = area.Size / texSize;
                if(keepAspect_)
                {
                    node_.Scale = Vector2FExt.One * MathF.Min(node_.Scale.X, node_.Scale.Y);
                    var size = texSize * node_.Scale;
                    node_.Position += (area.Size + size) * 0.5f;

                    area = new RectF(node_.Position, size);
                }
            }

            foreach (var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
