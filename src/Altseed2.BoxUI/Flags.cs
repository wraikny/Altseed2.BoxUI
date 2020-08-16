using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Altseed2.BoxUI
{
    [Serializable]
    public enum ColumnDir
    {
        X,
        Y,
    }

    [Serializable]
    public enum LengthScale
    {
        Fixed,
        Relative,
        RelativeMin,
        RelativeMax,
    }

    [Serializable]
    public enum Align
    {
        Min,
        Center,
        Max,
    }

    [Serializable]
    public enum Shape
    {
        Rectangle,
        Circle
    }

    [Serializable]
    public enum Aspect
    {
        Keep,
        Fixed,
        Responsive,
    }

    public static class FlagsValidater
    {
        public static void Validate(ColumnDir dir)
        {
            if (dir == ColumnDir.X || dir == ColumnDir.Y) return;
            throw new InvalidEnumArgumentException(nameof(dir), (int)dir, typeof(ColumnDir));
        }

        public static void Validate(LengthScale length)
        {
            switch(length)
            {
                case LengthScale.Fixed: return;
                case LengthScale.Relative: return;
                case LengthScale.RelativeMin: return;
                case LengthScale.RelativeMax: return;
                default:
                    throw new InvalidEnumArgumentException(nameof(length), (int)length, typeof(LengthScale));
            }
        }

        public static void Validate(Align align)
        {
            if (align == Align.Min || align == Align.Center || align == Align.Max) return;
            throw new InvalidEnumArgumentException(nameof(align), (int)align, typeof(Align));
        }

        public static void Validate(Shape shape)
        {
            switch(shape)
            {
                case Shape.Rectangle: return;
                case Shape.Circle: return;
                default:
                    throw new InvalidEnumArgumentException(nameof(shape), (int)shape, typeof(Shape));
            }
        }

        public static void Validate(Aspect aspect)
        {
            switch (aspect)
            {
                case Aspect.Keep: return;
                case Aspect.Fixed: return;
                case Aspect.Responsive: return;
                default:
                    throw new InvalidEnumArgumentException(nameof(aspect), (int)aspect, typeof(Aspect));
            }
        }
    }
}
