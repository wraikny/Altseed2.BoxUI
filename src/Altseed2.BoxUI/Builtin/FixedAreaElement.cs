using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class FixedAreaElement : Element
    {
        private RectF area_;
        public RectF Area
        {
            get => area_;
            set
            {
                if (area_ == value) return;
                area_ = value;
                RequireResize();
            }
        }

        private FixedAreaElement() { }

        public static FixedAreaElement Create(RectF area)
        {
            var elem = RentOrNull<FixedAreaElement>() ?? new FixedAreaElement();
            elem.area_ = area;
            return elem;
        }

        protected override void ReturnToCache()
        {
            area_ = default;
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F _) => area_.Size;

        protected override void OnResize(RectF _)
        {
            foreach(var c in Children)
            {
                c.Resize(area_);
            }
        }
    }
}
