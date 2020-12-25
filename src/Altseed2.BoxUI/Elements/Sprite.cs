using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Sprite : Element
    {
        ulong cameraGroup_;
        Aspect aspect_;
        bool horizontalFlip_;
        bool verticalFlip_;
        Color color_;
        int zOrder_;
        Material material_;
        TextureBase texture_;
        RectF src_;

        public event Action<SpriteNode> OnUpdateEvent;

        public SpriteNode Node { get; private set; }

        private Sprite() { }

        public static Sprite Create(
            ulong cameraGroup = 0,
            Aspect aspect = Aspect.Keep,
            bool horizontalFlip = false,
            bool verticalFlip = false,
            Color? color = null,
            int zOrder = 0,
            Material material = null,
            TextureBase texture = null,
            RectF? src = null
        )
        {
            var elem = BoxUISystem.RentOrNull<Sprite>() ?? new Sprite();
            elem.cameraGroup_ = cameraGroup;
            elem.aspect_ = aspect;
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
            Node = Root.RentOrCreate<SpriteNode>();
            Node.CameraGroup = cameraGroup_;
            Node.HorizontalFlip = horizontalFlip_;
            Node.VerticalFlip = verticalFlip_;
            Node.Color = color_;
            Node.ZOrder = zOrder_;
            Node.Material = material_;
            Node.Texture = texture_;
            Node.Src = src_;

            material_ = null;
            texture_ = null;
        }

        protected override void OnUpdate()
        {
            OnUpdateEvent?.Invoke(Node);
        }

        protected override Vector2F CalcSize(Vector2F size)
        {
            if (Node.Texture is null) return Vector2FExt.Zero;


            switch (aspect_)
            {
                case Aspect.Keep:
                    {
                        var srcSize = Node.ContentSize;
                        var scale = size / srcSize;
                        return srcSize * Vector2FExt.One * MathF.Min(scale.X, scale.Y);
                    }
                case Aspect.Fixed:
                    return Node.ContentSize;
                case Aspect.Responsive:
                    return size;
            }
            return size;
        }

        protected override void OnResize(RectF area)
        {
            var pos = area.Position;
            var size = CalcSize(area.Size);

            area = new RectF(pos, size);

            Node.Position = pos;
            Node.Scale = size / Node.ContentSize;

            foreach (var c in Children)
            {
                c.Resize(area);
            }
        }
    }
}
