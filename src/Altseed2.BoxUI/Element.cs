using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Altseed2.BoxUI
{
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

        private bool resizeRequired_;

        protected void RequireResize()
        {
            resizeRequired_ = true;
        }


        public abstract Vector2F CalcSize(Vector2F size);
        protected abstract void OnResize(RectF area);
        protected abstract void ReturnToCache();

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
            if(resizeRequired_ && lastArea_ is RectF area)
            {
                Resize(area);
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

        public Element With(Element child)
        {
            AddChild(child);
            return this;
        }

        /// <summary>
        /// クラスTのプールからブジェクトを取得します。
        /// 取得できなかった場合はnullが返ります。
        /// </summary>
        public static T RentOrNull<T>()
            where T : class => BoxUIPool<T>.Rent();

        /// <summary>
        /// T型のプールにT型のオブジェクトを返却します。
        /// </summary>
        public static void Return<T>(T element)
            where T : class => BoxUIPool<T>.Return(element);

        internal void Clear()
        {
            foreach (var c in Children)
            {
                c.Clear();
            }
            children_.Clear();

            ReturnToCache();
            Root = null;
        }
    }
}
