using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class WindowElement : Element, IAbsoluteSizeElement
    {
        private Vector2F windowSize_;

        private WindowElement() { }

        public static WindowElement Create()
        {
            return RentOrNull<WindowElement>() ?? new WindowElement();
        }

        protected override void ReturnToPool()
        {
            windowSize_ = default;
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F _) => Engine.WindowSize;

        protected override void OnResize(RectF area) { }

        protected override void OnUpdate()
        {
            var currentSize = Engine.WindowSize;
            if(windowSize_ != currentSize)
            {
                windowSize_ = currentSize;
                RequireResize();
            }
        }

        void IAbsoluteSizeElement.Resize()
        {
            foreach (var child in Children)
            {
                child.Resize(new RectF(Vector2FExt.Zero, Engine.WindowSize));
            }
        }
    }
}
