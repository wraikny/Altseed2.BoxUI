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
        public RectF? PreviousParentArea => previousParentArea;

        public (LengthScale, float) MarginLeft { get; set; }
        public (LengthScale, float) MarginRight { get; set; }
        public (LengthScale, float) MarginTop { get; set; }
        public (LengthScale, float) MarginBottom { get; set; }

        public Align AlignX { get; set; }
        public Align AlignY { get; set; }

        internal bool ResizeRequired { get; set; }

        protected void RequireResize()
        {
            ResizeRequired = true;
        }

        internal virtual void ResizeWhenRequired()
        {
            if(previousParentArea is RectF area)
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
            ResizeRequired = false;

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
            Root = root;
            OnAdded();
            foreach(var c in children_)
            {
                c.Added(root);
            }
        }

        internal void Update()
        {
            OnUpdate();
            if(ResizeRequired)
            {
                ResizeWhenRequired();
            }
            
            foreach(var c in children_)
            {
                c.Update();
            }
        }

        public void AddChild(Element child)
        {
            children_.Add(child);
            if(Root != null)
            {
                child.Added(Root);
                if(previousParentArea is RectF area)
                {
                    child.Resize(area);
                }
            }
        }

        internal void Clear()
        {
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
