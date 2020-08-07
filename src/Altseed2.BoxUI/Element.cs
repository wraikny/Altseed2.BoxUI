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

        private BoxUIRootNode root_;
        public BoxUIRootNode Root
        {
            get => root_;
            internal set
            {
                if (root_ == value) return;
                root_ = value;
                foreach(var c in children_)
                {
                    c.Root = value;
                }
            }
        }

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

        public Vector2F GetSize(Vector2F size)
        {
            // margin
            return CalcSize(LayoutArea(new RectF(Vector2FExt.Zero, size)).Size);
        }

        public void Resize(RectF area)
        {
            previousParentArea = area;
            ResizeRequired = false;

            // margin
            OnResize(LayoutArea(area));
        }

        /// <summary>
        /// MarginとAlignを適用した矩形領域を求める。
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        protected RectF LayoutArea(RectF area)
        {
            static float calcAlign(Align align, float pos, float areaSize, float cSize, float marginMin, float marginMax)
            {
                return align switch
                {
                    Align.Min => pos,
                    Align.Center => pos + (areaSize - cSize - marginMin - marginMax) * 0.5f,
                    Align.Max => pos + areaSize - cSize - marginMax,
                    _ => 0.0f,
                };
            }

            var (marginMin, marginMax) = BoxUIUtils.CalcMargin(this, area.Size);
            var marginedArea = new RectF(area.Position + marginMin, area.Size - marginMin - marginMax);

            var cSize = CalcSize(marginedArea.Size);

            var x = calcAlign(AlignX, marginedArea.Position.X, marginedArea.Size.X, cSize.X, marginMin.X, marginMax.X);
            var y = calcAlign(AlignY, marginedArea.Position.Y, marginedArea.Size.Y, cSize.Y, marginMin.Y, marginMax.Y);

            return new RectF(x, y, marginedArea.Width, marginedArea.Height);
        }

        internal void Added()
        {
            OnAdded();
            foreach(var c in children_)
            {
                c.Added();
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
            if(root_ != null)
            {
                child.Root = root_;
                child.Added();
                if(previousParentArea is RectF area)
                {
                    child.Resize(area);
                }
            }
        }

        internal void Clear()
        {
            foreach (var c in Children)
            {
                c.Clear();
            }
            children_.Clear();
            ReturnSelf();
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
