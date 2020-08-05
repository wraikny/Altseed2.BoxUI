﻿using System;
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

        private bool resizeRequired_ = false;

        public void RequireResize()
        {
            resizeRequired_ = true;
        }


        public abstract Vector2F CalcSize(Vector2F size);
        protected abstract void OnResize(RectF area);
        protected abstract void ReturnToPool();

        protected virtual void OnAdded() { }
        protected virtual void OnUpdate() { }

        public void Resize(RectF area)
        {
            lastArea_ = area;
            resizeRequired_ = false;
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
            if(resizeRequired_)
            {
                if(this is ElementRoot)
                {

                }
                else if (lastArea_ is RectF area)
                {
                    Resize(area);
                }
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
        protected override sealed void OnResize(RectF area) { }
        abstract protected void SetSize();

        internal void CallSetSize() => SetSize();
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
