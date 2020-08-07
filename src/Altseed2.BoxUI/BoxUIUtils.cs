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
    }
}
