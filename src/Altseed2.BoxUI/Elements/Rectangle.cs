using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Rectangle : Element
    {
        bool horizontalFlip_;
        bool verticalFlip_;
        Color color_;
        int zOrder_;
        Material material_;

        public event Action<RectangleNode> OnUpdateEvent;

        public RectangleNode Node { get; private set; }

        private Rectangle() { }

        public static Rectangle Create(
            bool horizontalFlip = false,
            bool verticalFlip = false,
            Color? color = null,
            int zOrder = 0,
            Material material = null
        )
        {
            var elem = BoxUISystem.RentOrNull<Rectangle>() ?? new Rectangle();
            elem.horizontalFlip_ = horizontalFlip;
            elem.verticalFlip_ = verticalFlip;
            elem.color_ = color ?? new Color(255, 255, 255, 255);
            elem.zOrder_ = zOrder;
            elem.material_ = material;
            return elem;
        }

        protected override void ReturnSelf()
        {
            Root.Return(Node);
            Node = null;
            OnUpdateEvent = null;
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

        protected override void OnUpdate()
        {
            OnUpdateEvent?.Invoke(Node);
        }

        protected override Vector2F CalcSize(Vector2F _) => Node.ContentSize;

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
