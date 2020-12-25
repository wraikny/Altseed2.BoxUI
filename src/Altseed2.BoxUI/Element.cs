using System;
using System.Collections.Generic;
using System.Text;

namespace Altseed2.BoxUI
{
    [Serializable]
    public abstract class Element
    {
        private readonly List<Element> children_ = new List<Element>();
        public IReadOnlyList<Element> Children => children_;

        public BoxUIRootNode Root { get; private set; }

        private RectF? previousParentArea;
        private (LengthScale, float) marginLeft;
        private (LengthScale, float) marginRight;
        private (LengthScale, float) marginTop;
        private (LengthScale, float) marginBottom;
        private Align alignX;
        private Align alignY;

        public RectF? PreviousParentArea => previousParentArea;

        public (LengthScale, float) MarginLeft
        {
            get => marginLeft;
            set
            {
                if (marginLeft == value) return;
                RequireResize();
                marginLeft = value;
            }
        }

        public (LengthScale, float) MarginRight
        {
            get => marginRight;
            set
            {
                if (marginRight == value) return;
                RequireResize();
                marginRight = value;
            }
        }
        public (LengthScale, float) MarginTop
        {
            get => marginTop;
            set
            {
                if (marginTop == value) return;
                RequireResize();
                marginTop = value;
            }
        }
        public (LengthScale, float) MarginBottom
        {
            get => marginBottom;
            set
            {
                if (marginBottom == value) return;
                RequireResize();
                marginBottom = value;
            }
        }

        public Align AlignX
        {
            get => alignX;
            set
            {
                if (alignX == value) return;
                RequireResize();
                alignX = value;
            }
        }
        public Align AlignY
        {
            get => alignY;
            set
            {
                if (alignY == value) return;
                RequireResize();
                alignY = value;
            }
        }

        internal bool ResizeRequired { get; set; }

        public void RequireResize()
        {
            ResizeRequired = true;
        }

        internal virtual void ResizeWhenRequired()
        {
            if (previousParentArea is RectF area)
            {
                Resize(area);
            }
        }

        protected abstract Vector2F CalcSize(Vector2F size);
        protected abstract void OnResize(RectF area);
        protected abstract void ReturnSelf();

        protected virtual void OnAdded() { }
        protected virtual void OnUpdate() { }

        /// <summary>
        /// CalcSizeにMarginを適用した大きさを取得する。
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public Vector2F GetSize(Vector2F size)
        {
            var (marginMin, marginMax) = BoxUIUtils.CalcMargin(this, size);
            return CalcSize(size - marginMin - marginMax);
        }

        public void Resize(RectF area)
        {
            previousParentArea = area;
            ResizeRequired = true;

            OnResize(LayoutArea(area));
        }

        /// <summary>
        /// AlignとMarginを適用した矩形領域を求める。
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        protected RectF LayoutArea(RectF area)
        {
            static float alignToFloat(Align align)
            {
                return align switch
                {
                    Align.Min => 0.0f,
                    Align.Center => 0.5f,
                    Align.Max => 1.0f,
                    _ => 0.0f,
                };
            }

            var alignOffset = (area.Size - CalcSize(area.Size)) * new Vector2F(alignToFloat(AlignX), alignToFloat(AlignY));

            var (marginMin, marginMax) = BoxUIUtils.CalcMargin(this, area.Size);
            var marginedArea = new RectF(area.Position + marginMin, area.Size - marginMin - marginMax);

            return new RectF(marginedArea.Position + alignOffset, marginedArea.Size);
        }

        internal void Added(BoxUIRootNode root)
        {
            if (Root is { })
            {
                throw new InvalidOperationException("すでに追加済みのElementです。");
            }

            Root = root;
            OnAdded();
            foreach (var c in children_)
            {
                c.Added(root);
            }
        }

        internal void Update()
        {
            OnUpdate();
            if (ResizeRequired)
            {
                ResizeWhenRequired();
            }

            foreach (var c in children_)
            {
                c.Update();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="child"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddChild(Element child)
        {
            if (child.Root is { })
            {
                throw new InvalidOperationException("追加済みのElementを追加しようとしました。");
            }

            children_.Add(child);
            if (Root is { })
            {
                child.Added(Root);
                if (previousParentArea is RectF area)
                {
                    child.Resize(area);
                }
            }
        }

        internal void Clear()
        {
            if (Root is null)
            {
                throw new InvalidOperationException("このElementはClear済みです。");
            }
            ReturnSelf();
            foreach (var c in Children)
            {
                c.Clear();
            }
            children_.Clear();
            previousParentArea = null;
            ResizeRequired = false;
            this.SetMargin(default, default(float)).SetAlign(default);
            Root = null;
        }
    }

    [Serializable]
    public abstract class ElementRoot : Element
    {
        internal override sealed void ResizeWhenRequired() => CallSetSize();
        protected override sealed void OnResize(RectF area) { }
        abstract protected void SetSize();

        internal void CallSetSize()
        {
            SetSize();
            ResizeRequired = false;
        }

    }

    public static class ElementExt
    {
        public static T With<T>(this T elem, Element child)
            where T : Element
        {
            elem.AddChild(child);
            return elem;
        }

        public static T With<T>(this T elem, Element child1, Element child2)
            where T : Element
        {
            elem.AddChild(child1);
            elem.AddChild(child2);
            return elem;
        }

        public static T With<T>(this T elem, Element child1, Element child2, Element child3)
            where T : Element
        {
            elem.AddChild(child1);
            elem.AddChild(child2);
            elem.AddChild(child3);
            return elem;
        }

        public static T With<T>(this T elem, Element child1, Element child2, Element child3, Element child4)
            where T : Element
        {
            elem.AddChild(child1);
            elem.AddChild(child2);
            elem.AddChild(child3);
            elem.AddChild(child4);
            return elem;
        }

        public static T With<T>(this T elem, Element child1, Element child2, Element child3, Element child4, Element child5)
            where T : Element
        {
            elem.AddChild(child1);
            elem.AddChild(child2);
            elem.AddChild(child3);
            elem.AddChild(child4);
            elem.AddChild(child5);
            return elem;
        }

        public static T With<T>(this T elem, Element child1, Element child2, Element child3, Element child4, Element child5, Element child6)
            where T : Element
        {
            elem.AddChild(child1);
            elem.AddChild(child2);
            elem.AddChild(child3);
            elem.AddChild(child4);
            elem.AddChild(child5);
            elem.AddChild(child6);
            return elem;
        }

        public static T With<T>(this T elem, Element child1, Element child2, Element child3, Element child4, Element child5, Element child6, Element child7)
            where T : Element
        {
            elem.AddChild(child1);
            elem.AddChild(child2);
            elem.AddChild(child3);
            elem.AddChild(child4);
            elem.AddChild(child5);
            elem.AddChild(child6);
            elem.AddChild(child7);
            return elem;
        }

        public static T With<T>(this T elem, Element child1, Element child2, Element child3, Element child4, Element child5, Element child6, Element child7, Element child8)
            where T : Element
        {
            elem.AddChild(child1);
            elem.AddChild(child2);
            elem.AddChild(child3);
            elem.AddChild(child4);
            elem.AddChild(child5);
            elem.AddChild(child6);
            elem.AddChild(child7);
            elem.AddChild(child8);
            return elem;
        }


        public static T SetMargin<T>(this T elem, LengthScale scale, float left, float right, float top, float bottom)
            where T : Element
        {
            elem.MarginLeft = (scale, left);
            elem.MarginRight = (scale, right);
            elem.MarginTop = (scale, top);
            elem.MarginBottom = (scale, bottom);
            return elem;
        }

        public static T SetMargin<T>(this T elem, LengthScale scale, Vector2F margin)
            where T : Element
        {
            elem.MarginLeft = (scale, margin.X);
            elem.MarginRight = (scale, margin.X);
            elem.MarginTop = (scale, margin.Y);
            elem.MarginBottom = (scale, margin.Y);
            return elem;
        }

        public static T SetMargin<T>(this T elem, LengthScale scale, float margin)
            where T : Element
        {
            elem.MarginLeft = (scale, margin);
            elem.MarginRight = (scale, margin);
            elem.MarginTop = (scale, margin);
            elem.MarginBottom = (scale, margin);
            return elem;
        }

        public static T SetAlign<T>(this T elem, Align alignX, Align alignY)
            where T : Element
        {
            elem.AlignX = alignX;
            elem.AlignY = alignY;
            return elem;
        }

        public static T SetAlign<T>(this T elem, Align align)
            where T : Element
        {
            elem.AlignX = align;
            elem.AlignY = align;
            return elem;
        }
    }
}
