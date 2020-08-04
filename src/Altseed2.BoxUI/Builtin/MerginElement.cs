using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class MerginElement : Element
    {
        private Vector2F mergin_;
        private UIScale uiScale_;

        public static MerginElement Create(Vector2F mergin, UIScale uiScale = default)
        {
            var elem = Rent<MerginElement>();
            elem.mergin_ = mergin;
            elem.uiScale_ = uiScale;
            return elem;
        }

        protected override void ReturnToCache()
        {
            Return(this);
        }

        protected override void OnResize(RectF area)
        {
            var mergin = uiScale_ switch
            {
                UIScale.Fixed => mergin_,
                UIScale.Relative => mergin_ * area.Size,
                _ => Vector2FExt.Zero,
            };

            var merginedArea = new RectF(area.Position + mergin, area.Size - mergin * 2.0f);

            foreach (var c in Children)
            {
                c.Resize(merginedArea);
            }
        }
    }
}
