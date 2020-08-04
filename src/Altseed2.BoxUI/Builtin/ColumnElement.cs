using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class ColumnElement : Element
    {
        Column dir_;

        private ColumnElement() { }

        public static ColumnElement Create(Column dir)
        {
            FlagsValidater.Validate(dir);

            var elem = RentOrNull<ColumnElement>() ?? new ColumnElement();
            elem.dir_ = dir;
            return elem;
        }

        protected override void ReturnToPool()
        {
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F size) => size;

        protected override void OnResize(RectF area)
        {
            var count = Children.Count;
            if (count == 0) return;

            var size = area.Size;
            var offset = Vector2FExt.Zero;

            switch(dir_)
            {
                case Column.X:
                    size.X /= count;
                    offset = new Vector2F(size.X, 0.0f);
                    break;
                case Column.Y:
                    size.Y /= count;
                    offset = new Vector2F(0.0f, size.Y);
                    break;
            }

            for(int i = 0; i < count; i++)
            {
                Children[i].Resize(new RectF(area.Position + offset * i, size));
            }
        }
    }
}
