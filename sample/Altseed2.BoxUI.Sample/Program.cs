using Altseed2.BoxUI.Elements;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Altseed2.BoxUI.Sample
{
    static class Params
    {
        public static readonly Color BackgroundColor = new Color(180, 180, 220);
        public static readonly Color DefaultColor = new Color(200, 200, 200);
        public static readonly Color HoverColor = new Color(180, 180, 180);
        public static readonly Color HoldColor = new Color(150, 150, 150);
        public static readonly Color TextColor = new Color(0, 0, 0);
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var config = new Configuration();
            config.ConsoleLoggingEnabled = true;
            config.IsResizable = true;
            if (!Engine.Initialize("Altseed2.BoxUI.Test", 800, 600, config))
            {
                return;
            }

            var samples = new List<Node> {
                new CounterSample(),
                new WindowsSample(),
            };

            var cursor = new BoxUIMouseCursor("MenuMouse");
            Engine.AddNode(cursor);

            var menuRoot = new BoxUIRootNode();
            menuRoot.Cursors.Add(cursor);
            menuRoot.SetElement(MakeView(samples));

            Engine.AddNode(menuRoot);

            while(Engine.DoEvents())
            {
                if(Engine.Keyboard.GetKeyState(Key.Num0) == ButtonState.Release)
                {
                    foreach(var n in Engine.GetNodes())
                    {
                        PrintNodeTree(n);
                    }
                }

                // 必ず更新処理をする。
                BoxUISystem.Update();
                Engine.Update();
            }

            // 必ず終了処理をする
            BoxUISystem.Termiante();
            Engine.Terminate();
        }

        static ElementRoot MakeView(IList<Node> samples)
        {
            var column = Column.Create(ColumnDir.Y);
            column.MarginBottom = (LengthScale.Relative, 1.0f - samples.Count / 8.0f);

            Node current = null;
            foreach(var sample in samples)
            {
                column.AddChild(MakeButton(sample.GetType().Name, _ => {
                    if(current != sample)
                    {
                        if(current != null)
                        {
                            Engine.RemoveNode(current);
                        }
                        current = sample;
                        Engine.AddNode(current);
                    }
                }));
            }

            return
                Window.Create().SetMargin(LengthScale.Relative, 0.0f, 0.7f, 0.0f, 0.0f)
                    .With(Rectangle.Create(color:new Color(230, 230, 100), zOrder:-10))
                    .With(column)
            ;
        }

        static Element MakeButton(string text, Action<IBoxUICursor> action)
        {
            var font = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 30);
            var background = Rectangle.Create(color: Params.DefaultColor, zOrder:-9);

            return
                // 背景
                background.SetMargin(LengthScale.RelativeMin, 0.1f)
                // 中心にテキスト
                .With(Text.Create(color: Params.TextColor, text: text, font: font, zOrder: -8).SetAlign(Align.Center))
                // 当たり判定・アクション
                .With(Button.Create()
                    .OnRelease(action)
                    .OnFree(_ => { background.Node.Color = Params.HoverColor; })
                    .OnHold(_ => { background.Node.Color = Params.HoldColor; })
                    .WhileNotCollided(() => { background.Node.Color = Params.DefaultColor; })
                )
            ;
        }

        static void PrintNodeTree(Node node)
        {
            static void printNode(Node n, string s)
            {
                Console.WriteLine($"{s}{n.GetType().Name}");
                s = s + "\t";
                foreach(var child in n.Children)
                {
                    printNode(child, s);
                }
            }

            printNode(node, "");
        }
    }
}
