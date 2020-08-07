using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public sealed class Button : Element
    {
        Action<IBoxUICursor> onFree_;
        Action<IBoxUICursor> onPush_;
        Action<IBoxUICursor> onHold_;
        Action<IBoxUICursor> onRelease_;
        Action whileNotCollided_;
        Collider collider_;

        Matrix44F previousTransform_;

        private Button() { }

        public static Button CreateRectangle()
        {
            var elem = BoxUISystem.RentOrNull<Button>() ?? new Button();
            elem.collider_ = BoxUISystem.RentOrNull<RectangleCollider>() ?? RectangleCollider.Create();
            return elem;
        }

        public static Button CreateCircle()
        {
            var elem = BoxUISystem.RentOrNull<Button>() ?? new Button();
            elem.collider_ = BoxUISystem.RentOrNull<CircleCollider>() ?? CircleCollider.Create();
            return elem;
        }

        protected override void ReturnSelf()
        {
            switch (collider_)
            {
                case RectangleCollider rect:
                    BoxUISystem.Return(rect);
                    break;
                case CircleCollider circle:
                    BoxUISystem.Return(circle);
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
            BoxUISystem.Return(this);
        }

        public override Vector2F CalcSize(Vector2F size) => size;

        private void UpdateTransform(RectF area)
        {
            var (absoluteArea, angle) = BoxUIUtils.TransformArea(area, Root.InheritedTransform);

            switch (collider_)
            {
                case RectangleCollider rect:
                    rect.Position = absoluteArea.Position;
                    rect.Size = absoluteArea.Size;
                    rect.Rotation = angle;
                    break;
                case CircleCollider circle:
                    circle.Position = absoluteArea.Position + absoluteArea.Size * 0.5f;
                    circle.Radius = MathF.Min(absoluteArea.Size.X, absoluteArea.Size.Y) * 0.5f;
                    break;
                default:
                    break;
            }

            previousTransform_ = Root.InheritedTransform;
        }

        protected override void OnResize(RectF area)
        {
            UpdateTransform(area);

            foreach(var c in Children)
            {
                c.Resize(area);
            }
        }

        protected override void OnUpdate()
        {
            if(Root.Cursors.Count > 0 && Root.InheritedTransform != previousTransform_ && PreviousParentArea is RectF area)
            {
                UpdateTransform(area);
            }

            bool isCollidedAny = false;

            for(int i  = 0; i < Root.Cursors.Count; i++)
            {
                var cursor = Root.Cursors[i];
                if (cursor is null || !cursor.IsActive) continue;

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

        public Button OnFree(Action<IBoxUICursor> action)
        {
            if (action != null)
            {
                onFree_ += action;
            }
            return this;
        }

        public Button OnPush(Action<IBoxUICursor> action)
        {
            if (action != null)
            {
                onPush_ += action;
            }
            return this;
        }

        public Button OnHold(Action<IBoxUICursor> action)
        {
            if (action != null)
            {
                onHold_ += action;
            }
            return this;
        }
        public Button OnRelease(Action<IBoxUICursor> action)
        {
            if (action != null)
            {
                onRelease_ += action;
            }
            return this;
        }

        public Button WhileNotCollided(Action action)
        {
            if (action != null)
            {
                whileNotCollided_ += action;
            }
            return this;
        }
    }
}
