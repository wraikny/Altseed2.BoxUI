using System;
using System.Collections.Generic;
using System.Numerics;
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

        internal bool ResizeRequired { get; set; }

        public void RequireResize()
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

        public abstract Vector2F CalcSize(Vector2F size);
        protected abstract void OnResize(RectF area);
        protected abstract void ReturnSelf();

        protected virtual void OnAdded() { }
        protected virtual void OnUpdate() { }

        public void Resize(RectF area)
        {
            previousParentArea = area;
            ResizeRequired = false;
            OnResize(area);
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
    }
}
