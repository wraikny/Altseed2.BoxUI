using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class FixedSize : ElementRoot
    {
        private Vector2F size_;

        public Vector2F Size
        {
            get => size_;
            set
            {
                if (size_ == value) return;
                size_ = value;
                RequireResize();
            }
        }

        private FixedSize() { }

        public static FixedSize Create(Vector2F size)
        {
            var elem = BoxUISystem.RentOrNull<FixedSize>() ?? new FixedSize();
            elem.size_ = size;
            return elem;
        }

        protected override void ReturnSelf()
        {
            size_ = default;
            BoxUISystem.Return(this);
        }

        protected override Vector2F CalcSize(Vector2F _) => size_;

        protected override void SetSize()
        {
            var area = LayoutArea(new RectF(0.0f, 0.0f, size_.X, size_.Y));

            foreach (var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
