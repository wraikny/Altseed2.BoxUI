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
            public static Color Background => new Color(200, 200, 200);
            public static Color Text => new Color(20, 20, 20);
            public static Color ButtonDefault => new Color(150, 150, 150);
            public static Color ButtonHover => new Color(120, 120, 120);
            public static Color ButtonHold => new Color(100, 100, 100);
        }

        readonly Font _font;

        public View()
        {
            _font = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 70); ;
        }

        // 最終的な見た目を作る
        public ElementRoot MakeView(State state, Action<IMsg> dispatch)
        {
            // Y方向分割
            var content = Column.Create(ColumnDir.Y)
                .SetMargin(LengthScale.RelativeMin, 0.05f)
                .With(
                    // 中心にテキスト
                    MakeText(text:$"{state.Count}").SetAlign(Align.Center),
                    // X方向分割
                    Column.Create(ColumnDir.X).With(
                        MakeButton("+", _ => dispatch(State.Incr)),
                        MakeButton("-", _ => dispatch(State.Decr))
                    )
                );

            return Window.Create()
                .SetMargin(LengthScale.RelativeMin, 0.25f)
                .With(
                    Rectangle.Create(color: Colors.Background, zOrder: ZOrders.Background),
                    content
                );
        }

        // テキスト作成する
        Text MakeText(string text)
        {
            return Text.Create(text: text, font: _font, color: Colors.Text, zOrder: ZOrders.Text);
        }

        // ボタンを作成する
        Element MakeButton(string text, Action<IBoxUICursor> action)
        {
            // ボタンの背景Element
            var background = Rectangle.Create(zOrder: ZOrders.Button);

            // 当たり判定とイベントの定義
            var button = Button.Create()
                .OnRelease(action)
                .OnFree(_ => background.Node.Color = Colors.ButtonHover)
                .OnHold(_ => background.Node.Color = Colors.ButtonHold)
                .WhileNotCollided(() => background.Node.Color = Colors.ButtonDefault);

            return background
                .SetMargin(LengthScale.RelativeMin, 0.05f)
                .With(
                    MakeText(text).SetAlign(Align.Center),
                    button
                );
        }
    }
}
