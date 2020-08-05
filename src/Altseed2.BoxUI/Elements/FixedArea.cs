﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class FixedArea : ElementRoot
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

        private FixedArea() { }

        public static FixedArea Create(RectF area)
        {
            var elem = BoxUISystem.RentOrNull<FixedArea>() ?? new FixedArea();
            elem.area_ = area;
            return elem;
        }

        protected override void ReturnToPool()
        {
            area_ = default;
            BoxUISystem.Return(this);
        }

        public override Vector2F CalcSize(Vector2F _) => area_.Size;

        protected override void SetSize()
        {
            foreach (var c in Children)
            {
                c.Resize(area_);
            }
        }
    }
}
