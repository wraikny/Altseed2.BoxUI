using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    public interface IBoxUICursor
    {
        string Id { get; }
        Vector2F Position { get; }
        ButtonState ButtonState { get; }
        Collider Collider { get; }
        bool IsActive { get; set;  }
    }

    public sealed class BoxUIMouseCursor : Node, IBoxUICursor
    {
        private readonly string id_;
        private readonly MouseButton button_;
        private readonly CircleCollider collider_;
        private ButtonState buttonState_;

        public BoxUIMouseCursor(string id, MouseButton button = MouseButton.ButtonLeft, float radius = 5.0f)
        {
            id_ = id;
            button_ = button;
            collider_ = CircleCollider.Create();
            collider_.Radius = radius;
        }

        protected override void OnUpdate()
        {
            if(IsActive)
            {
                collider_.Position = Engine.Mouse.Position;
                buttonState_ = Engine.Mouse.GetMouseButtonState(button_);
            }
        }

        string IBoxUICursor.Id => id_;
        Vector2F IBoxUICursor.Position => collider_.Position;
        ButtonState IBoxUICursor.ButtonState => buttonState_;
        Collider IBoxUICursor.Collider => collider_;
        public bool IsActive { get; set; } = true;
    }
}
