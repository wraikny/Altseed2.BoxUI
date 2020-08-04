﻿using System;
using System.Collections.Generic;
using System.Text;
using Altseed2.BoxUI.Builtin;

namespace Altseed2.BoxUI.Test
{
    class Counter : Node
    {
        private int count_;

        private readonly BoxUIRootNode uiRoot_;
        private Font font_;

        bool updateView;

        public Counter()
        {
            uiRoot_ = new BoxUIRootNode();
            AddChildNode(uiRoot_);
            updateView = true;
        }

        protected override void OnAdded()
        {
            font_ = Font.LoadDynamicFont("TestData/Font/mplus-1m-regular.ttf", 20);
            uiRoot_.Cursors.Add(new BoxUIMouseCursor("Mouse"));
        }

        protected override void OnUpdate()
        {
            if (updateView)
            {
                // Don't call SetElement directly from Element
                View();
                updateView = false;
            }
        }

        private static readonly Color backgroundColor = new Color(180, 180, 220);
        private static readonly Color defaultColor = new Color(200, 200, 200);
        private static readonly Color hoverColor = new Color(180, 180, 180);
        private static readonly Color holdColor = new Color(150, 150, 150);
        private static readonly Color textColor = new Color();

        void View()
        {
            // Call ClearElement before Create Element
            uiRoot_.ClearElement();
            uiRoot_.SetElement(WindowElement.Create()
                .With(MerginElement.Create(new Vector2F(0.25f, 0.25f), UIScale.Relative)
                    .With(RectangleElement.Create(rect => {
                        //rect.ZOrder = 0;
                        rect.Color = backgroundColor;
                    }))
                    .With(ColumnElement.Create(UIDir.Y)
                        .With(AlignElement.Center()
                            .With(TextElement.Create(t => {
                                //t.ZOrder = 2;
                                t.Font = font_;
                                t.Text = $"{count_}";
                                t.Color = textColor;
                            }))
                        )
                        .With(ColumnElement.Create(UIDir.X)
                            .With(CreateButton("-", _ => {
                                count_--;
                                // Don't call SetElement directly from Element
                                updateView = true;
                                Console.WriteLine(count_);
                            }))
                            .With(CreateButton("+", _ => {
                                count_++;
                                // Don't call SetElement directly from Element
                                updateView = true;
                                Console.WriteLine(count_);
                            }))
                        )
                    )
                )
            );
        }

        private Element CreateButton(string text, Action<IBoxUICursor> action)
        {
            var background = RectangleElement.Create(r =>
            {
                //r.ZOrder = 1;
                r.Color = defaultColor;
            });

            return MerginElement.Create(new Vector2F(0.05f, 0.05f), UIScale.Relative)
                .With(background)
                .With(AlignElement.Create(Align.Center, Align.Center)
                    .With(TextElement.Create(t =>
                    {
                        //t.ZOrder = 2;
                        t.Font = font_;
                        t.Text = text;
                        t.Color = textColor;
                    }))
                )
                .With(ButtonElement.CreateRectangle()
                    .OnRelease(action)
                    .OnFree(_ => { background.Node.Color = hoverColor; })
                    .OnHold(_ => { background.Node.Color = holdColor; })
                    .WhileNotCollided(() => { background.Node.Color = defaultColor; })
                )
            ;
        }
    }
}
