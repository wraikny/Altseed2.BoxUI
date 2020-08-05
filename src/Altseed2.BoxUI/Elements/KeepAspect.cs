using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class KeepAspect : Element
    {
        private Vector2F aspect_;

        private KeepAspect() { }

        public static KeepAspect Create(Vector2F aspect)
        {
            var elem = BoxUISystem.RentOrNull<KeepAspect>() ?? new KeepAspect();
            elem.aspect_ = aspect;
            return elem;
        }

        protected override void ReturnToPool()
        {
            BoxUISystem.Return(this);
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
