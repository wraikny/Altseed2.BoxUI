﻿using System;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Window : ElementRoot
    {
        private Vector2F windowSize_;

        private Window() { }

        public static Window Create()
        {
            return BoxUISystem.RentOrNull<Window>() ?? new Window();
        }

        protected override void ReturnSelf()
        {
            windowSize_ = default;
            BoxUISystem.Return(this);
        }

        protected override Vector2F CalcSize(Vector2F _) => Engine.WindowSize;

        protected override void OnUpdate()
        {
            var currentSize = Engine.WindowSize;
            if(windowSize_ != currentSize)
            {
                windowSize_ = currentSize;
                RequireResize();
            }
        }

        protected override void SetSize()
        {
            var area = LayoutArea(new RectF(Vector2FExt.Zero, Engine.WindowSize));
            foreach (var child in Children)
            {
                child.Resize(area);
            }
        }
    }
}
