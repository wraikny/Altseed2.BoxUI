using Altseed2.BoxUI.Elements;
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
            private static int IdNext = 0;
            public int Id { get; private set; }
            public int Count { get; private set; }

            public State(int count)
            {
                Id = IdNext;
                Count = count;
                IdNext++;
            }

            public void Incr() => Count++;

            public void Decr() => Count--;
        }

        private static readonly Vector2F WindowSize = new Vector2F(200.0f, 300.0f);
        readonly Stack<BoxUIRootNode> rootPool_;
        private readonly IBoxUICursor cursor_;

        public WindowsSample()
        {
            rootPool_ = new Stack<BoxUIRootNode>();
            var cursor = new BoxUIMouseCursor("Mouse");
            AddChildNode(cursor);
            cursor_ = cursor;
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
                root = new BoxUIRootNode(isAutoTerminated:false);
                root.Cursors.Add(cursor_);
            }

            root.Position = position;
            AddChildNode(root);

            root.ClearElement();
            root.SetElement(MakeView(new State(0), () => {
                RemoveChildNode(root);
                root.Terminate();
                rootPool_.Push(root);
            }));
        }

        private ElementRoot MakeView(State state, Action closeWindow)
        {
            Vector2F zero = new Vector2F(0.0f, 0.0f);

            static Text makeText(Font font, string text, int zOrder)
            {
                return Text.Create(
                    font: font,
                    text: text,
                    color: Params.TextColor,
                    zOrder: zOrder + 2
                );
            }

            static Element makeButton(Font font, int zOrder, string text, Action<IBoxUICursor> action)
            {
                var background = Rectangle.Create(color: Params.DefaultColor, zOrder: zOrder + 1);

                return background.SetMargin(LengthScale.RelativeMin, 0.05f).With(
                    makeText(font, text, zOrder).SetAlign(Align.Center),
                    Button.Create()
                        .OnPush(action)
                        .WhileNotCollided(() => { background.Node.Color = Params.DefaultColor; })
                        .OnFree(_ => { background.Node.Color = Params.HoverColor; })
                        .OnHold(_ => { background.Node.Color = Params.HoldColor; })
                );
            }

            var font = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 40);

            var zOrderOffset = state.Id << 3;

            var textElem = makeText(font, $"{state.Id}: {state.Count}", zOrderOffset);

            // 固定サイズ
            return FixedArea.Create(new RectF(zero, WindowSize)).With(
                // 背景色
                Rectangle.Create(color: new Color(50, 50, 100), zOrder: zOrderOffset + 0),
                // 縦方向分割
                Column.Create(ColumnDir.Y).SetMargin(LengthScale.RelativeMin, 0.05f).With(
                    // テキスト
                    textElem.SetMargin(LengthScale.RelativeMin, 0.05f),
                    // デクリメントボタン
                    makeButton(font, zOrderOffset, "-", _ =>
                    {
                        state.Decr();
                        textElem.Node.Text = $"{state.Count}";
                    }),
                    // インクリメントボタン
                    makeButton(font, zOrderOffset, "+", _ =>
                    {
                        state.Incr();
                        textElem.Node.Text = $"{state.Count}";
                    }),
                    // 閉じるボタン
                    makeButton(font, zOrderOffset, "close", _ =>
                    {
                        Console.WriteLine($"close({state.Id})");
                        BoxUISystem.Post(closeWindow);
                    })
                )
            );
        }
    }
}
