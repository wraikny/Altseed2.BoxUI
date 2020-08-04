using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class AlignElement : Element
    {
        Align xAlign_;
        Align yAlign_;

        private AlignElement() { }

        public static AlignElement Create(Align xAlign, Align yAlign)
        {
            FlagsValidater.Validate(xAlign);
            FlagsValidater.Validate(yAlign);

            var elem = RentOrNull<AlignElement>() ?? new AlignElement();
            elem.xAlign_ = xAlign;
            elem.yAlign_ = yAlign;
            return elem;
        }

        public static AlignElement Center => Create(Align.Center, Align.Center);

        protected override void ReturnToPool()
        {
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F size) => size;

        protected override void OnResize(RectF area)
        {
            foreach(var c in Children)
            {
                static float calcAlign(Align align, float pos, float size, float cSize)
                {
                    return align switch
                    {
                        Align.Min => pos,
                        Align.Center => pos + (size - cSize) * 0.5f,
                        Align.Max => pos + size - cSize,
                        _ => 0.0f,
                    };
                }

                var cSize = c.CalcSize(area.Size);

                var x = calcAlign(xAlign_, area.Position.X, area.Size.X, cSize.X);
                var y = calcAlign(yAlign_, area.Position.Y, area.Size.Y, cSize.Y);

                c.Resize(new RectF(new Vector2F(x, y), area.Size));
            }
        }
    }
}
