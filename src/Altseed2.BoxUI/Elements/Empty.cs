﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    public sealed class Empty : Element
    {
        private Empty() { }

        public static Empty Create()
        {
            return BoxUISystem.RentOrNull<Empty>() ?? new Empty();
        }

        protected override void ReturnSelf()
        {
            BoxUISystem.Return(this);
        }

        protected override Vector2F CalcSize(Vector2F size) => size;

        protected override void OnResize(RectF area)
        {
            foreach (var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
