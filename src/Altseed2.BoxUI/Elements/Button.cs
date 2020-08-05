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

        Matrix44F lastTransform_;

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

        protected override void ReturnToPool()
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

        private void Resize(RectF area)
        {
            var transform = Root.InheritedTransform;

            var pos3 = new Vector3F(area.Position.X, area.Position.Y, 0.0f);
            var a = new Vector3F(area.Position.X + area.Size.X, area.Position.Y, 0.0f);
            var b = new Vector3F(area.Position.X, area.Position.Y + area.Size.Y, 0.0f);

            var pos3t = transform.Transform3D(pos3);
            var at = transform.Transform3D(a);
            var bt = transform.Transform3D(b);

            var aDiff = at - pos3t;

            var position = new Vector2F(pos3t.X, pos3t.Y);
            var size = new Vector2F(aDiff.Length, (bt - pos3t).Length);
            var angle = MathF.Atan2(aDiff.Y, aDiff.X);

            switch (collider_)
            {
                case RectangleCollider rect:
                    rect.Position = position;
                    rect.Size = size;
                    rect.Rotation = angle;
                    break;
                case CircleCollider circle:
                    circle.Position = position + size * 0.5f;
                    circle.Radius = MathF.Min(size.X, size.Y) * 0.5f;
                    break;
                default:
                    break;
            }
        }

        protected override void OnResize(RectF area)
        {
            Resize(area);

            foreach(var c in Children)
            {
                c.Resize(area);
            }
        }

        protected override void OnUpdate()
        {
            var transform = Root.InheritedTransform;
            
            if(Root.Cursors.Count > 0 && transform != lastTransform_ && LastArea is RectF area)
            {
                Resize(area);

                lastTransform_ = transform;
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
