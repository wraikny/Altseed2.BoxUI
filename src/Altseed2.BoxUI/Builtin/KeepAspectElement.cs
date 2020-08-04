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

        public override Vector2F CalcSize(Vector2F size)
        {
            if (aspect_.X == 0.0f && aspect_.Y == 0.0f)
            {
                return Vector2FExt.Zero;
            }
            else
            {
                var aspect = size.X > size.Y ? aspect_ / aspect_.Y : aspect_ / aspect_.X;
                return size * aspect;
            }
        }

        protected override void OnResize(RectF area)
        {
            var size = CalcSize(area.Size);
            var position = area.Position + (area.Size - size) * 0.5f;
            area = new RectF(position, size);

            foreach (var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
