using System;
using Altseed2;
using Altseed2.BoxUI;
using Altseed2.BoxUI.Elements;

namespace CounterExample
{
    sealed class View
    {
        static class ZOrders
        {
            public const int Background = 0;
            public const int Button = 1;
            public const int Text = 2;
        }

        static class Colors
        {
            public static readonly Color Background = new Color(200, 200, 200);
            public static readonly Color Text = new Color(20, 20, 20);
            public static readonly Color ButtonDefault = new Color(150, 150, 150);
            public static readonly Color ButtonHover = new Color(120, 120, 120);
            public static readonly Color ButtonHold = new Color(100, 100, 100);
        }

        readonly Font _font;

        public View()
        {
            _font = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 70); ;
        }

        public ElementRoot MakeView(State state, Action<Msg> dispatch)
        {
            return Window.Create().SetMargin(LengthScale.RelativeMin, 0.25f).With(
                Rectangle.Create(color: Colors.Background, zOrder: ZOrders.Background),
                // Y方向分割
                Column.Create(ColumnDir.Y).SetMargin(LengthScale.RelativeMin, 0.05f).With(
                    // 中心にテキスト
                    MakeText(text:$"{state.Count}").SetAlign(Align.Center),
                    // X方向分割
                    Column.Create(ColumnDir.X).With(
                        MakeButton("+", _ => dispatch(State.Incr)),
                        MakeButton("-", _ => dispatch(State.Decr))
                    )
                )
            );
        }

        Text MakeText(string text)
        {
            return Text.Create(text: text, font: _font, color: Colors.Text, zOrder: ZOrders.Text);
        }

        Element MakeButton(string text, Action<IBoxUICursor> action)
        {
            // ボタンの背景
            var background = Rectangle.Create(zOrder: ZOrders.Button);

            return
                background.SetMargin(LengthScale.RelativeMin, 0.05f).With(
                    // 中心にテキスト
                    MakeText(text).SetAlign(Align.Center),
                    // 当たり判定・アクション
                    Button.Create()
                        .OnRelease(action)
                        .OnFree(_ => background.Node.Color = Colors.ButtonHover)
                        .OnHold(_ => background.Node.Color = Colors.ButtonHold)
                        .WhileNotCollided(() => background.Node.Color = Colors.ButtonDefault)
                )
            ;
        }
    }
}
