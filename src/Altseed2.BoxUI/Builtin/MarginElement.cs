using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    [Serializable]
    public sealed class MarginElement : Element
    {
        private Vector2F margin_;
        private Margin uiScale_;

        private MarginElement() { }

        public static MarginElement Create(Vector2F margin, Margin uiScale = default)
        {
            FlagsValidater.Validate(uiScale);

            var elem = BoxUISystem.RentOrNull<MarginElement>() ?? new MarginElement();
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
                Margin.Fixed => margin_,
                Margin.Relative => margin_ * size,
                Margin.RelativeMin => (margin_ * size).Min() * Vector2FExt.One,
                Margin.RelativeMax => (margin_ * size).Max() * Vector2FExt.One,
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