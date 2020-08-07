using System;

namespace Altseed2.BoxUI
{
    public static class BoxUIUtils
    {
        public static Vector2F GetInheritedPosition(this TransformNode node)
        {
            var v = node.InheritedTransform.Transform3D(new Vector3F(node.Position.X, node.Position.Y, 0.0f));
            return new Vector2F(v.X, v.Y);
        }

        public static (RectF, float) TransformArea(RectF area, Matrix44F transform)
        {
            var pos3 = new Vector3F(area.Position.X, area.Position.Y, 0.0f);
            var a = new Vector3F(area.Position.X + area.Size.X, area.Position.Y, 0.0f);
            var b = new Vector3F(area.Position.X, area.Position.Y + area.Size.Y, 0.0f);

            var pos3t = transform.Transform3D(pos3);
            var at = transform.Transform3D(a);
            var bt = transform.Transform3D(b);

            var aDiff = at - pos3t;

            var position = new Vector2F(pos3t.X, pos3t.Y);
            var size = new Vector2F(aDiff.Length, (bt - pos3t).Length);
            var angle = MathF.Atan2(aDiff.Y, aDiff.X);

            return (new RectF(position, size), angle);
        }

        static float CalcLengthScale(Vector2F areaSize, float size, (LengthScale, float) x)
        {
            return x switch
            {
                (LengthScale.Fixed, float v) => v,
                (LengthScale.Relative, float v) => v * size,
                (LengthScale.RelativeMin, float v) => v * areaSize.Min(),
                (LengthScale.RelativeMax, float v) => v * areaSize.Max(),
                _ => 0.0f,
            };
        }

        public static (Vector2F marginMin, Vector2F marginMax) CalcMargin(Element elem, Vector2F areaSize)
        {

            var marginLeft = CalcLengthScale(areaSize, areaSize.X, elem.MarginLeft);
            var marginRight = CalcLengthScale(areaSize, areaSize.X, elem.MarginRight);
            var marginTop = CalcLengthScale(areaSize, areaSize.Y, elem.MarginTop);
            var marginBottom = CalcLengthScale(areaSize, areaSize.Y, elem.MarginBottom);

            return (new Vector2F(marginLeft, marginTop), new Vector2F(marginRight, marginBottom));
        }
    }
}
