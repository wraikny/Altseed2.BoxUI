using System;
using System.Collections.Generic;
using System.Text;
using Altseed2.BoxUI.Builtin;

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

            var textElem = TextElement.Create(color: Params.TextColor, text: $"{state.Count}", font: font);

            // クリア
            root.ClearElement();

            // Window全体
            root.SetElement(WindowElement.Create()
                // マージン
                .With(MarginElement.Create(new Vector2F(0.25f, 0.25f), Margin.RelativeMin)
                    // 背景色
                    .With(RectangleElement.Create(color:Params.BackgroundColor))
                    // マージン
                    .With(MarginElement.Create(new Vector2F(0.05f, 0.05f), Margin.RelativeMin)
                        // Y方向分割
                        .With(ColumnElement.Create(Column.Y)
                            // 中心にテキスト
                            .With(AlignElement.Center.With(textElem))
                            // X方向分割
                            .With(ColumnElement.Create(Column.X)
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
                )
            );
        }

        public static Element CounterButton(Font font, string text, Action<IBoxUICursor> action)
        {
            var background = RectangleElement.Create(color: Params.DefaultColor);

            // マージン
            return MarginElement.Create(new Vector2F(0.05f, 0.05f), Margin.Relative)
                // 背景色
                .With(background)
                // 中心にテキスト
                .With(AlignElement.Create(Align.Center, Align.Center)
                    .With(TextElement.Create(color: Params.TextColor, text: text, font: font))
                )
                // 当たり判定・アクション
                .With(ButtonElement.CreateRectangle()
                    .OnRelease(action)
                    .OnFree(_ => { background.Node.Color = Params.HoverColor; })
                    .OnHold(_ => { background.Node.Color = Params.HoldColor; })
                    .WhileNotCollided(() => { background.Node.Color = Params.DefaultColor; })
                )
            ;
        }
    }
}
