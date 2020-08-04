﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class TextElement : Element
    {
        bool horizontalFlip_;
        bool verticalFlip_;
        Color color_;
        int zOrder_;
        Material materialGlyph_;
        Material materialImage_;
        string text_;
        Font font_;

        public TextNode Node { get; private set; }

        private TextElement() { }

        public static TextElement Create(
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
            var elem = RentOrNull<TextElement>() ?? new TextElement();
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

        protected override void ReturnToPool()
        {
            Root.Return(Node);
            Node = null;
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F _) => Node.ContentSize;

        protected override void OnAdded()
        {
            Node = Root.RentOrCreate<TextNode>();
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

        protected override void OnResize(RectF area)
        {
            Node.Position = area.Position;
        }
    }
}
