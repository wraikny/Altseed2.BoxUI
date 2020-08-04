using Altseed2.BoxUI.Builtin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Altseed2.BoxUI.Sample
{
    sealed class WindowsSample : Node
    {
        private class State
        {
            private static int id = 0;
            public int Id { get; private set; }
            public int Count { get; set; }

            public State()
            {
                Id = id;
                id++;
            }
        }

        private static readonly Vector2F WindowSize = new Vector2F(200.0f, 300.0f);
        readonly Stack<BoxUIRootNode> rootPool_;
        private Font font_;
        private IBoxUICursor cursor_;

        public WindowsSample()
        {
            rootPool_ = new Stack<BoxUIRootNode>();
            var cursor = new BoxUIMouseCursor("Mouse");
            AddChildNode(cursor);
            cursor_ = cursor;
        }

        protected override void OnAdded()
        {
            font_ = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 40);
        }

        protected override void OnUpdate()
        {
            if(Engine.Mouse.GetMouseButtonState(MouseButton.ButtonRight) == ButtonState.Push)
            {
                CreateWindow(Engine.Mouse.Position);
            }
        }

        private void CreateWindow(Vector2F position)
        {
            if(!rootPool_.TryPop(out var root))
            {
                root = new BoxUIRootNode();
                root.Cursors.Add(cursor_);
            }

            root.Position = position;
            AddChildNode(root);

            MakeView(root, new State());
        }

        private void RemoveWindow(BoxUIRootNode root)
        {
            RemoveChildNode(root);
            rootPool_.Push(root);
        }

        private void MakeView(BoxUIRootNode root, State state)
        {
            Vector2F zero = new Vector2F(0.0f, 0.0f);

            void incr(IBoxUICursor _)
            {
                state.Count++;
                Console.WriteLine($"incr({state.Id}): {state.Count}");
                // Elementの更新は遅延させる
                BoxUISystem.Post(() => MakeView(root, state));
            }

            void decr(IBoxUICursor _)
            {
                state.Count--;
                Console.WriteLine($"decr({state.Id}): {state.Count}");
                // Elementの更新は遅延させる
                BoxUISystem.Post(() => { MakeView(root, state); });
            }

            void close(IBoxUICursor _)
            {
                Console.WriteLine($"close({state.Id})");
                RemoveWindow(root);
            }

            static Element makeText(Font font, string text, int zOrder)
            {
                return
                    TextElement.Create(
                        font: font,
                        text: text,
                        color: Params.TextColor,
                        zOrder: zOrder + 2
                    );
            }

            static MerginElement makeMergin()
            {
                return MerginElement
                    .Create(new Vector2F(0.05f, 0.05f), Builtin.Mergin.RelativeMin);
            }

            static Element makeButton(Font font, int zOrder, string text, Action<IBoxUICursor> action)
            {
                var background = RectangleElement.Create(color: Params.DefaultColor, zOrder: zOrder + 1);

                return makeMergin()
                    .With(background)
                    .With(AlignElement.Center.With(makeText(font, text, zOrder)))
                    .With(ButtonElement.CreateRectangle()
                        .OnPush(action)
                        .WhileNotCollided(() => { background.Node.Color = Params.DefaultColor; })
                        .OnFree(_ => { background.Node.Color = Params.HoverColor; })
                        .OnHold(_ => { background.Node.Color = Params.HoldColor; })
                    );
            }

            var zOrderOffset = state.Id << 3;

            root.ClearElement();
            root.SetElement(FixedAreaElement.Create(new RectF(zero, WindowSize))
                .With(RectangleElement.Create(color: new Color(50, 50, 100), zOrder: zOrderOffset + 0))
                .With(makeMergin()
                    .With(ColumnElement.Create(Column.Y)
                        .With(makeMergin().With(makeText(font_, $"{state.Id}: {state.Count}", zOrderOffset)))
                        .With(makeButton(font_, zOrderOffset, "-", decr))
                        .With(makeButton(font_, zOrderOffset, "+", incr))
                        .With(makeButton(font_, zOrderOffset, "close", close))
                    )
                )
            );
        }
    }
}
