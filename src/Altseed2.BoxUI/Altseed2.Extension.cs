using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    internal static class Vector2FExt
    {
        internal static Vector2F Zero => new Vector2F(0.0f, 0.0f);

        internal static Vector2F One => new Vector2F(1.0f, 1.0f);

        internal static float Min(this Vector2F v) => MathF.Min(v.X, v.Y);

        internal static float Max(this Vector2F v) => MathF.Max(v.X, v.Y);
    }

    internal static class ColorExt
    {
        internal static Color White => new Color(255, 255, 255);
    }

    internal static class NodeExt
    {
        internal static bool FindAncestorStatus(this Node node, RegisterStatus status)
        {
            Node current = node;
            while (current != null)
            {
                if (current.Status == status) return true;
                current = current.Parent;
            }
            return false;
        }
    }
}
