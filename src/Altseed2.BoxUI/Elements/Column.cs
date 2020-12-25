using System;
using System.ComponentModel;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Column : Element
    {
        ColumnDir dir_;

        private Column() { }

        public static Column Create(ColumnDir dir)
        {
            if (dir != ColumnDir.X && dir != ColumnDir.Y)
            {
                throw new InvalidEnumArgumentException(nameof(dir), (int)dir, typeof(ColumnDir));
            }

            var elem = BoxUISystem.RentOrNull<Column>() ?? new Column();
            elem.dir_ = dir;
            return elem;
        }

        protected override void ReturnSelf()
        {
            BoxUISystem.Return(this);
        }

        protected override Vector2F CalcSize(Vector2F size) => size;

        protected override void OnResize(RectF area)
        {
            area = LayoutArea(area);

            var count = Children.Count;
            if (count == 0) return;

            var size = area.Size;
            var offset = Vector2FExt.Zero;

            switch(dir_)
            {
                case ColumnDir.X:
                    size.X /= count;
                    offset = new Vector2F(size.X, 0.0f);
                    break;
                case ColumnDir.Y:
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
