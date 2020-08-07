using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Align : Element
    {
        AlignPos xAlign_;
        AlignPos yAlign_;

        private Align() { }

        public static Align Create(AlignPos xAlign, AlignPos yAlign)
        {
            FlagsValidater.Validate(xAlign);
            FlagsValidater.Validate(yAlign);

            var elem = BoxUISystem.RentOrNull<Align>() ?? new Align();
            elem.xAlign_ = xAlign;
            elem.yAlign_ = yAlign;
            return elem;
        }

        public static Align CreateCenter() => Create(AlignPos.Center, AlignPos.Center);

        protected override void ReturnSelf()
        {
            BoxUISystem.Return(this);
        }

        public override Vector2F CalcSize(Vector2F size) => size;

        protected override void OnResize(RectF area)
        {
            foreach(var c in Children)
            {
                static float calcAlign(AlignPos align, float pos, float size, float cSize)
                {
                    return align switch
                    {
                        AlignPos.Min => pos,
                        AlignPos.Center => pos + (size - cSize) * 0.5f,
                        AlignPos.Max => pos + size - cSize,
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
