using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class ButtonElement : Element
    {
        Collider collider_;

        private ButtonElement() { }

        public static ButtonElement CreateRectangle()
        {
            var elem = RentOrNull<ButtonElement>() ?? new ButtonElement();
            elem.collider_ = RentOrNull<RectangleCollider>() ?? RectangleCollider.Create();
            return elem;
        }

        public static ButtonElement CreateCircle()
        {
            var elem = RentOrNull<ButtonElement>() ?? new ButtonElement();
            elem.collider_ = RentOrNull<CircleCollider>() ?? CircleCollider.Create();
            return elem;
        }

        protected override void ReturnToCache()
        {
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F size) => size;

        protected override void OnResize(RectF area)
        {

        }
    }
}
