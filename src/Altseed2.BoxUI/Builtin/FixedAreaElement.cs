﻿using System;
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

        public static FixedAreaElement Create(RectF area)
        {
            var elem = Rent<FixedAreaElement>() ?? new FixedAreaElement();
            elem.area_ = area;
            return elem;
        }

        protected override void ReturnToCache()
        {
            Return(this);
            area_ = default;
        }

        protected override void OnResize(RectF _)
        {
            foreach(var c in Children)
            {
                c.Resize(area_);
            }
        }
    }
}
