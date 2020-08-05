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

        private RectF? lastArea_;
        public RectF? LastArea => lastArea_;

        internal bool ResizeRequired { get; set; }

        public void RequireResize()
        {
            ResizeRequired = true;
        }

        internal virtual void UpdateSizeWhenRequired()
        {
            if(lastArea_ is RectF area)
            {
                Resize(area);
            }
        }

        public abstract Vector2F CalcSize(Vector2F size);
        protected abstract void OnResize(RectF area);
        protected abstract void ReturnToPool();

        protected virtual void OnAdded() { }
        protected virtual void OnUpdate() { }

        public void Resize(RectF area)
        {
            lastArea_ = area;
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
                UpdateSizeWhenRequired();
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
                if(lastArea_ is RectF area)
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

            ReturnToPool();
            Root = null;
        }
    }

    [Serializable]
    public abstract class ElementRoot : Element
    {
        internal override sealed void UpdateSizeWhenRequired() => CallSetSize();
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
