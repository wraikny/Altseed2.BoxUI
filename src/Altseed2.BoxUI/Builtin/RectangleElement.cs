using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    [Serializable]
    public sealed class RectangleElement : Element
    {
        bool horizontalFlip_;
        bool verticalFlip_;
        Color color_;
        int zOrder_;
        Material material_;

        public RectangleNode Node { get; private set; }

        private RectangleElement() { }

        public static RectangleElement Create(
            bool horizontalFlip = false,
            bool verticalFlip = false,
            Color? color = null,
            int zOrder = 0,
            Material material = null
        )
        {
            var elem = BoxUISystem.RentOrNull<RectangleElement>() ?? new RectangleElement();
            elem.horizontalFlip_ = horizontalFlip;
            elem.verticalFlip_ = verticalFlip;
            elem.color_ = color ?? new Color(255, 255, 255, 255);
            elem.zOrder_ = zOrder;
            elem.material_ = material;
            return elem;
        }

        protected override void ReturnToPool()
        {
            Root.Return(Node);
            Node = null;
            BoxUISystem.Return(this);
        }

        protected override void OnAdded()
        {
            Node = Root.RentOrCreate<RectangleNode>();
            Node.HorizontalFlip = horizontalFlip_;
            Node.VerticalFlip = verticalFlip_;
            Node.Color = color_;
            Node.ZOrder = zOrder_;
            Node.Material = material_;

            material_ = null;
        }

        public override Vector2F CalcSize(Vector2F _) => Node.ContentSize;

        protected override void OnResize(RectF area)
        {
            Node.Position = area.Position;
            Node.RectangleSize = area.Size;

            foreach(var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
