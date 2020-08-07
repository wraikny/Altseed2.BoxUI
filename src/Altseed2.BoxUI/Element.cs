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
            return CalcSize(CalcMargin(size).Size);
        }

        public void Resize(RectF area)
        {
            previousParentArea = area;
            ResizeRequired = false;

            OnResize(CalcMargin(area));
        }

        protected RectF CalcMargin(Vector2F areaSize)
        {
            float margin1(float size, (LengthScale, float) x)
            {
                return x switch
                {
                    (LengthScale.Fixed, float v) => v,
                    (LengthScale.Relative, float v) => v * size,
                    (LengthScale.RelativeMin, float v) => v * areaSize.Min(),
                    (LengthScale.RelativeMax, float v) => v * areaSize.Max(),
                    _ => 0.0f,
                };
            }

            var marginLeft = margin1(areaSize.X, MarginLeft);
            var marginRight = margin1(areaSize.X, MarginRight);
            var marginTop = margin1(areaSize.Y, MarginTop);
            var marginBottom = margin1(areaSize.Y, MarginBottom);

            return new RectF(
                marginLeft,
                marginTop,
                areaSize.X - marginLeft - marginRight,
                areaSize.Y - marginTop - marginBottom
            );
        }

        protected RectF CalcMargin(RectF area)
        {
            var marginedArea = CalcMargin(area.Size);
            return new RectF(area.Position + marginedArea.Position, marginedArea.Size);
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
            this.SetMargin(LengthScale.Fixed, 0.0f);
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
    }
}
