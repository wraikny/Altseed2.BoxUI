using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Altseed2.BoxUI.Builtin
{
    [Serializable]
    public enum Column
    {
        X,
        Y,
    }

    [Serializable]
    public enum Margin
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

    public static class FlagsValidater
    {
        public static void Validate(Column dir)
        {
            if (dir == Column.X || dir == Column.Y) return;
            throw new InvalidEnumArgumentException(nameof(dir), (int)dir, typeof(Column));
        }

        public static void Validate(Margin scale)
        {
            switch(scale)
            {
                case Margin.Fixed: return;
                case Margin.Relative: return;
                case Margin.RelativeMin: return;
                case Margin.RelativeMax: return;
                default:
                    throw new InvalidEnumArgumentException(nameof(scale), (int)scale, typeof(Margin));
            }
        }

        public static void Validate(Align align)
        {
            if (align == Align.Min || align == Align.Center || align == Align.Max) return;
            throw new InvalidEnumArgumentException(nameof(align), (int)align, typeof(Align));
        }
    }
}
