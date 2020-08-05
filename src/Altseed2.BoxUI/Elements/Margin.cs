using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Margin : Element
    {
        private Vector2F margin_;
        private MarginScale uiScale_;

        private Margin() { }

        public static Margin Create(Vector2F margin, MarginScale uiScale = default)
        {
            FlagsValidater.Validate(uiScale);

            var elem = BoxUISystem.RentOrNull<Margin>() ?? new Margin();
            elem.margin_ = margin;
            elem.uiScale_ = uiScale;
            return elem;
        }

        protected override void ReturnToPool()
        {
            BoxUISystem.Return(this);
        }

        public override Vector2F CalcSize(Vector2F size)
        {
            var margin = uiScale_ switch
            {
                MarginScale.Fixed => margin_,
                MarginScale.Relative => margin_ * size,
                MarginScale.RelativeMin => (margin_ * size).Min() * Vector2FExt.One,
                MarginScale.RelativeMax => (margin_ * size).Max() * Vector2FExt.One,
                _ => Vector2FExt.Zero,
            };

            return size - margin * 2.0f;
        }

        protected override void OnResize(RectF area)
        {
            var size = CalcSize(area.Size);

            var mArea = new RectF(area.Position + (area.Size - size) * 0.5f, size);

            foreach (var c in Children)
            {
                c.Resize(mArea);
            }
        }
    }
}