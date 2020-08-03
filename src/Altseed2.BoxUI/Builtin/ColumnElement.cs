using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class ColumnElement : Element
    {
        public static ColumnElement Create()
        {
            var elem = Rent<ColumnElement>();
            return elem;
        }

        protected override void ReturnToCache()
        {
            Return<ColumnElement>(this);
        }

        protected override void OnResize(RectF area)
        {

        }
    }
}
