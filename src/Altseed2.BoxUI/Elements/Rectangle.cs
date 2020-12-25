using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Rectangle : Element
    {
        ulong cameraGroup_;
        bool horizontalFlip_;
        bool verticalFlip_;
        Color color_;
        int zOrder_;
        Material material_;
        TextureBase texture_;
        RectF src_;

        public event Action<RectangleNode> OnUpdateEvent;

        public RectangleNode Node { get; private set; }

        private Rectangle() { }

        public static Rectangle Create(
            ulong cameraGroup = 0,
            bool horizontalFlip = false,
            bool verticalFlip = false,
            Color? color = null,
            int zOrder = 0,
            Material material = null,
            TextureBase texture = null,
            RectF? src = null
        )
        {
            var elem = BoxUISystem.RentOrNull<Rectangle>() ?? new Rectangle();
            elem.cameraGroup_ = cameraGroup;
            elem.horizontalFlip_ = horizontalFlip;
            elem.verticalFlip_ = verticalFlip;
            elem.color_ = color ?? new Color(255, 255, 255, 255);
            elem.zOrder_ = zOrder;
            elem.material_ = material;
            elem.texture_ = texture;
            elem.src_ = src ?? new RectF(Vector2FExt.Zero, texture?.Size.To2F() ?? Vector2FExt.Zero);
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
            Node.CameraGroup = cameraGroup_;
            Node.HorizontalFlip = horizontalFlip_;
            Node.VerticalFlip = verticalFlip_;
            Node.Color = color_;
            Node.ZOrder = zOrder_;
            Node.Material = material_;
            Node.Texture = texture_;
            Node.Src = src_;

            material_ = null;
        }

        protected override void OnUpdate()
        {
            OnUpdateEvent?.Invoke(Node);
        }

        protected override Vector2F CalcSize(Vector2F _) => Node.ContentSize;

        protected override void OnResize(RectF area)
        {
            area = LayoutArea(area);

            Node.Position = area.Position;
            Node.RectangleSize = area.Size;

            foreach(var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
