using System;
using System.Collections.Generic;
using System.Text;
using Altseed2.BoxUI.Builtin;

namespace Altseed2.BoxUI.Sample
{
    sealed class CounterSample : Node
    {
        private int count_;

        private readonly BoxUIRootNode uiRoot_;
        private Font font_;

        bool updateView;

        public CounterSample()
        {
            uiRoot_ = new BoxUIRootNode();
            AddChildNode(uiRoot_);
            updateView = true;
        }

        protected override void OnAdded()
        {
            font_ = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 70);
            uiRoot_.Cursors.Add(new BoxUIMouseCursor("Mouse"));
        }

        protected override void OnUpdate()
        {
            if (updateView)
            {
                // Don't call SetElement directly from Element
                View(count_);
                updateView = false;
            }
        }

        void Decrement(IBoxUICursor _)
        {
            count_--;
            // Don't call SetElement directly from Element
            updateView = true;
            Console.WriteLine(count_);
        }

        void Increment(IBoxUICursor _)
        {
            count_++;
            // Don't call SetElement directly from Element
            updateView = true;
            Console.WriteLine(count_);
        }


        void View(int count)
        {
            Element CreateButton(Font font, string text, Action<IBoxUICursor> action)
            {
                var background = RectangleElement.Create(color: Params.DefaultColor);

                return MarginElement.Create(new Vector2F(0.05f, 0.05f), Margin.Relative)
                    .With(background)
                    .With(AlignElement.Create(Align.Center, Align.Center)
                        .With(TextElement.Create(color: Params.TextColor, text: text, font: font))
                    )
                    .With(ButtonElement.CreateRectangle()
                        .OnRelease(action)
                        .OnFree(_ => { background.Node.Color = Params.HoverColor; })
                        .OnHold(_ => { background.Node.Color = Params.HoldColor; })
                        .WhileNotCollided(() => { background.Node.Color = Params.DefaultColor; })
                    )
                ;
            }

            // Call ClearElement before Create Element
            uiRoot_.ClearElement();
            uiRoot_.SetElement(WindowElement.Create()
                .With(MarginElement.Create(new Vector2F(0.25f, 0.25f), Margin.RelativeMin)
                    .With(RectangleElement.Create(color:Params.BackgroundColor))
                    .With(MarginElement.Create(new Vector2F(0.05f, 0.05f), Margin.RelativeMin)
                        .With(ColumnElement.Create(Column.Y)
                            .With(AlignElement.Center
                                .With(TextElement.Create(color: Params.TextColor, text: $"{count}", font: font_))
                            )
                            .With(ColumnElement.Create(Column.X)
                                .With(CreateButton(font_, "-", Decrement))
                                .With(CreateButton(font_, "+", Increment))
                            )
                        )
                    )
                )
            );
        }
    }
}
