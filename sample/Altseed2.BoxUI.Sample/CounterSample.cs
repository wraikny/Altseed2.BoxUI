using System;
using System.Collections.Generic;
using System.Text;
using Altseed2.BoxUI.Elements;

namespace Altseed2.BoxUI.Sample
{
    sealed class CounterSample : Node
    {
        private readonly BoxUIRootNode uiRoot_;

        public CounterSample()
        {
            var cursor = new BoxUIMouseCursor("Mouse");
            AddChildNode(cursor);

            uiRoot_ = new BoxUIRootNode();
            AddChildNode(uiRoot_);
            uiRoot_.Cursors.Add(cursor);
        }

        protected override void OnAdded()
        {
            MakeView(uiRoot_, new State());
        }

        private class State
        {
            public int Count { get; set; }
        }

        static void MakeView(BoxUIRootNode root, State state)
        {
            var font = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 70);

            var textElem = Text.Create(color: Params.TextColor, text: $"{state.Count}", font: font);

            // クリア
            root.ClearElement();

            // Window全体
            root.SetElement(Window.Create().SetMargin(LengthScale.RelativeMin, 0.25f)
                // 背景色
                .With(Rectangle.Create(color:Params.BackgroundColor)
                    // Y方向分割
                    .With(Column.Create(ColumnDir.Y).SetMargin(LengthScale.RelativeMin, 0.05f)
                        // 中心にテキスト
                        .With(textElem.SetAlign(Align.Center))
                        // X方向分割
                        .With(Column.Create(ColumnDir.X)
                            // ボタン
                            .With(CounterButton(font, "-", _ => {
                                state.Count--;
                                textElem.Node.Text = $"{state.Count}";
                                Console.WriteLine(state.Count);
                            }))
                            .With(CounterButton(font, "+", _ => {
                                state.Count++;
                                textElem.Node.Text = $"{state.Count}";
                                Console.WriteLine(state.Count);
                            }))
                        )
                    )
                )
            );
        }

        public static Element CounterButton(Font font, string text, Action<IBoxUICursor> action)
        {
            var background = Rectangle.Create(color: Params.DefaultColor);

            // マージン
            return
                // 背景色
                background.SetMargin(LengthScale.RelativeMin, 0.05f)
                // 中心にテキスト
                .With(Text.Create(color: Params.TextColor, text: text, font: font).SetAlign(Align.Center))
                // 当たり判定・アクション
                .With(Button.Create()
                    .OnRelease(action)
                    .OnFree(_ => { background.Node.Color = Params.HoverColor; })
                    .OnHold(_ => { background.Node.Color = Params.HoldColor; })
                    .WhileNotCollided(() => { background.Node.Color = Params.DefaultColor; })
                )
            ;
        }
    }
}
