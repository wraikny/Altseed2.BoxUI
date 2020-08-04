using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class WindowElement : Element
    {
        private Vector2F windowSize_;

        public static WindowElement Create()
        {
            return Rent<WindowElement>();
        }

        protected override void ReturnToCache()
        {
            Return(this);
            windowSize_ = default;
        }

        public override Vector2F CalcSize(Vector2F _) => Engine.WindowSize;

        protected override void OnResize(RectF area) { }

        protected override void OnUpdate()
        {
            var currentSize = Engine.WindowSize;
            if(windowSize_ != currentSize)
            {
                windowSize_ = currentSize;

                foreach (var child in Children)
                {
                    child.Resize(new RectF(Vector2FExt.Zero, windowSize_));
                }
            }
        }
    }
}
