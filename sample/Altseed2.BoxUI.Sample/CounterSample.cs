using System;
using System.Collections.Generic;
using System.Text;
using Altseed2.BoxUI.Elements;

namespace Altseed2.BoxUI.Sample
{
    sealed class CounterSample : Node
    {
        const int ZOrderBackground = 0;
        const int ZOrderButton = 1;
        const int ZOrderText = 2;

        State state_;
        private readonly BoxUIRootNode uiRoot_;

        public CounterSample()
        {
            state_ = new State();

            uiRoot_ = new BoxUIRootNode();
            AddChildNode(uiRoot_);

            var cursor = new BoxUIMouseCursor();
            AddChildNode(cursor);

            uiRoot_.Cursors.Add(cursor);
        }

        protected override void OnAdded()
        {
            MakeView(uiRoot_, state_);
        }

        private class State
        {
            public int Count { get; set; }
        }

        static void MakeView(BoxUIRootNode root, State state)
        {
            var font = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 70);

            var textElem = Text.Create(color: Params.TextColor, text: $"{state.Count}", font: font, zOrder: ZOrderText);

            // クリア
            root.ClearElement();

            // Window全体
            root.SetElement(
                Window.Create().SetMargin(LengthScale.RelativeMin, 0.25f).With(
                    // 背景色
                    Rectangle.Create(color:Params.BackgroundColor, zOrder: ZOrderBackground).With(
                        // Y方向分割
                        Column.Create(ColumnDir.Y).SetMargin(LengthScale.RelativeMin, 0.05f).With(
                            // 中心にテキスト
                            textElem.SetAlign(Align.Center),
                            // X方向分割
                            Column.Create(ColumnDir.X).With(
                                // ボタン
                                CounterButton(font, "-", _ => {
                                    state.Count--;
                                    textElem.Node.Text = $"{state.Count}";
                                    Console.WriteLine(state.Count);
                                }),
                                CounterButton(font, "+", _ => {
                                    state.Count++;
                                    textElem.Node.Text = $"{state.Count}";
                                    Console.WriteLine(state.Count);
                                })
                            )
                        )
                    )
                )
            );
        }

        public static Element CounterButton(Font font, string text, Action<IBoxUICursor> action)
        {
            var background = Rectangle.Create(color: Params.DefaultColor, zOrder: ZOrderButton);

            return
                // 背景
                background.SetMargin(LengthScale.RelativeMin, 0.05f).With(
                    // 中心にテキスト
                    Text.Create(color: Params.TextColor, text: text, font: font, zOrder: ZOrderText).SetAlign(Align.Center),
                    // 当たり判定・アクション
                    Button.Create()
                        .OnRelease(action)
                        .OnFree(_ => { background.Node.Color = Params.HoverColor; })
                        .OnHold(_ => { background.Node.Color = Params.HoldColor; })
                        .WhileNotCollided(() => { background.Node.Color = Params.DefaultColor; })
                )
            ;
        }
    }
}
