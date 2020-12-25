using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Text : Element
    {
        ulong cameraGroup_;
        Aspect aspect_;
        bool horizontalFlip_;
        bool verticalFlip_;
        Color color_;
        int zOrder_;
        Material materialGlyph_;
        Material materialImage_;
        string text_;
        Font font_;

        public event Action<TextNode> OnUpdateEvent;

        public TextNode Node { get; private set; }

        private Text() { }

        public static Text Create(
            ulong cameraGroup = 0,
            Aspect aspect = Aspect.Fixed,
            bool horizontalFlip = false,
            bool verticalFlip = false,
            Color? color = null,
            int zOrder = 0,
            Material materialGlyph = null,
            Material materialImage = null,
            string text = null,
            Font font = null
        )
        {
            var elem = BoxUISystem.RentOrNull<Text>() ?? new Text();
            elem.cameraGroup_ = cameraGroup;
            elem.aspect_ = aspect;
            elem.horizontalFlip_ = horizontalFlip;
            elem.verticalFlip_ = verticalFlip;
            elem.color_ = color ?? new Color(255, 255, 255, 255);
            elem.zOrder_ = zOrder;
            elem.materialGlyph_ = materialGlyph;
            elem.materialImage_ = materialImage;
            elem.text_ = text;
            elem.font_ = font;
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
            Node = Root.RentOrCreate<TextNode>();
            Node.CameraGroup = cameraGroup_;
            Node.HorizontalFlip = horizontalFlip_;
            Node.VerticalFlip = verticalFlip_;
            Node.Color = color_;
            Node.ZOrder = zOrder_;
            Node.MaterialGlyph = materialGlyph_;
            Node.MaterialImage = materialImage_;
            Node.Text = text_;
            Node.Font = font_;

            materialGlyph_ = null;
            materialImage_ = null;
            text_ = null;
            font_ = null;
        }

        protected override void OnUpdate()
        {
            OnUpdateEvent?.Invoke(Node);
        }

        protected override Vector2F CalcSize(Vector2F size)
        {
            if (Node.Text is null || Node.Font is null) return Vector2FExt.Zero;


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
