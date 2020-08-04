using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Altseed2.BoxUI.Builtin
{
    public enum UIDir
    {
        X,
        Y,
    }

    public enum UIScale
    {
        Fixed,
        Relative,
    }

    public enum Align
    {
        Min,
        Center,
        Max,
    }

    public static class FlagsValidater
    {
        public static void Validate(UIDir dir)
        {
            if (dir == UIDir.X || dir == UIDir.Y) return;
            throw new InvalidEnumArgumentException(nameof(dir), (int)dir, typeof(UIDir));
        }

        public static void Validate(UIScale scale)
        {
            if (scale == UIScale.Fixed || scale == UIScale.Relative) return;
            throw new InvalidEnumArgumentException(nameof(scale), (int)scale, typeof(UIScale));
        }

        public static void Validate(Align align)
        {
            if (align == Align.Min || align == Align.Center || align == Align.Max) return;
            throw new InvalidEnumArgumentException(nameof(align), (int)align, typeof(Align));
        }
    }
}
