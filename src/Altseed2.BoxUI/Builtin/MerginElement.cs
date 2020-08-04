using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class MerginElement : Element
    {
        private Vector2F mergin_;
        private UIScale uiScale_;

        private MerginElement() { }

        public static MerginElement Create(Vector2F mergin, UIScale uiScale = default)
        {
            FlagsValidater.Validate(uiScale);

            var elem = RentOrNull<MerginElement>() ?? new MerginElement();
            elem.mergin_ = mergin;
            elem.uiScale_ = uiScale;
            return elem;
        }

        protected override void ReturnToPool()
        {
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F size)
        {
            var mergin = uiScale_ switch
            {
                UIScale.Fixed => mergin_,
                UIScale.Relative => mergin_ * size,
                _ => Vector2FExt.Zero,
            };

            return size - mergin * 2.0f;
        }

        protected override void OnResize(RectF area)
        {
            var size = CalcSize(area.Size);

            var merginedArea = new RectF(area.Position + (area.Size - size) * 0.5f, size);

            foreach (var c in Children)
            {
                c.Resize(merginedArea);
            }
        }
    }
}