using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class KeepAspectElement : Element
    {
        private Vector2F aspect_;

        public static KeepAspectElement Create(Vector2F aspect)
        {
            var elem = Rent<KeepAspectElement>();
            elem.aspect_ = aspect;
            return elem;
        }

        protected override void ReturnToCache()
        {
            Return(this);
        }

        protected override void OnResize(RectF area)
        {
            if(aspect_.X == 0.0f && aspect_.Y == 0.0f)
            {
                area = new RectF(area.Position, Vector2FExt.Zero);
            }
            else
            {
                var aspect = area.Size.X > area.Size.Y ? aspect_ / aspect_.Y : aspect_ / aspect_.X;
                var size = area.Size * aspect;
                var position = area.Position + (area.Size - size) * 0.5f;
                area = new RectF(position, size);
            }

            foreach (var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
