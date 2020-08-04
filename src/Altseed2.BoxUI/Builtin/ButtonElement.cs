using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altseed2.BoxUI.Builtin
{
    public sealed class ButtonElement : Element
    {
        Action<IBoxUICursor> onFree_;
        Action<IBoxUICursor> onPush_;
        Action<IBoxUICursor> onHold_;
        Action<IBoxUICursor> onRelease_;
        Action whileNotCollided_;
        Collider collider_;

        private ButtonElement() { }

        public static ButtonElement CreateRectangle()
        {
            var elem = RentOrNull<ButtonElement>() ?? new ButtonElement();
            elem.collider_ = RentOrNull<RectangleCollider>() ?? RectangleCollider.Create();
            return elem;
        }

        public static ButtonElement CreateCircle()
        {
            var elem = RentOrNull<ButtonElement>() ?? new ButtonElement();
            elem.collider_ = RentOrNull<CircleCollider>() ?? CircleCollider.Create();
            return elem;
        }

        protected override void ReturnToPool()
        {
            switch (collider_)
            {
                case RectangleCollider rect:
                    Return(rect);
                    break;
                case CircleCollider circle:
                    Return(circle);
                    break;
                default:
                    break;
            }
            collider_ = null;
            onFree_ = null;
            onPush_ = null;
            onHold_ = null;
            onRelease_ = null;
            whileNotCollided_ = null;
            Return(this);
        }

        public override Vector2F CalcSize(Vector2F size) => size;

        protected override void OnResize(RectF area)
        {
            switch(collider_)
            {
                case RectangleCollider rect:
                    rect.Position = area.Position;
                    rect.Size = area.Size;
                    break;
                case CircleCollider circle:
                    circle.Position = area.Position + area.Size * 0.5f;
                    circle.Radius = MathF.Min(area.Size.X, area.Size.Y) * 0.5f;
                    break;
                default:
                    break;
            }
        }

        protected override void OnUpdate()
        {
            bool isCollidedAny = false;

            for(int i  = 0; i < Root.Cursors.Count; i++)
            {
                var cursor = Root.Cursors[i];
                if (cursor is null) continue;

                if (cursor.Collider?.GetIsCollidedWith(collider_) ?? false)
                {
                    isCollidedAny = true;

                    switch (cursor.ButtonState)
                    {
                        case ButtonState.Free:
                            onFree_?.Invoke(cursor);
                            break;
                        case ButtonState.Push:
                            onPush_?.Invoke(cursor);
                            break;
                        case ButtonState.Hold:
                            onHold_?.Invoke(cursor);
                            break;
                        case ButtonState.Release:
                            onRelease_?.Invoke(cursor);
                            break;
                        default:
                            break;
                    };
                }
            }

            if (!isCollidedAny)
            {
                whileNotCollided_?.Invoke();
            }
        }

        public ButtonElement OnFree(Action<IBoxUICursor> action)
        {
            if (action != null)
            {
                onFree_ += action;
            }
            return this;
        }

        public ButtonElement OnPush(Action<IBoxUICursor> action)
        {
            if (action != null)
            {
                onPush_ += action;
            }
            return this;
        }

        public ButtonElement OnHold(Action<IBoxUICursor> action)
        {
            if (action != null)
            {
                onHold_ += action;
            }
            return this;
        }
        public ButtonElement OnRelease(Action<IBoxUICursor> action)
        {
            if (action != null)
            {
                onRelease_ += action;
            }
            return this;
        }

        public ButtonElement WhileNotCollided(Action action)
        {
            if (action != null)
            {
                whileNotCollided_ += action;
            }
            return this;
        }
    }
}
