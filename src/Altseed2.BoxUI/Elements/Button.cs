using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.ComponentModel;

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

        public bool IsActive { get; set; }

        private Button() { }

        public static Button Create(Shape shape = Shape.Rectangle)
        {
            var elem = BoxUISystem.RentOrNull<Button>() ?? new Button();
            elem.IsActive = true;
            switch (shape)
            {
                case Shape.Rectangle:
                    elem.collider_ = BoxUISystem.RentOrNull<RectangleCollider>() ?? new RectangleCollider();
                    break;
                case Shape.Circle:
                    elem.collider_ = BoxUISystem.RentOrNull<CircleCollider>() ?? new CircleCollider();
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(shape), (int)shape, typeof(Shape));

            }
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
            IsActive = true;
            collider_ = null;
            onFree_ = null;
            onPush_ = null;
            onHold_ = null;
            onRelease_ = null;
            whileNotCollided_ = null;
            BoxUISystem.Return(this);
        }

        protected override Vector2F CalcSize(Vector2F size) => size;

        protected override void OnResize(RectF area)
        {
            foreach(var c in Children)
            {
                c.Resize(area);
            }
        }

        protected override void OnUpdate()
        {
            if (!IsActive) return;

            if(Root.Cursors.Count > 0 && Root.InheritedTransform != previousTransform_ && PreviousParentArea is RectF area)
            {
                var (position, size, angle, center) = BoxUIUtils.TransformArea(area, Root.InheritedTransform);

                switch (collider_)
                {
                    case RectangleCollider rect:
                        rect.Position = position;
                        rect.Size = size;
                        rect.Rotation = angle;
                        break;
                    case CircleCollider circle:
                        circle.Position = center;
                        circle.Radius = size.Min() * 0.5f;
                        break;
                    default:
                        break;
                }

                previousTransform_ = Root.InheritedTransform;
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
