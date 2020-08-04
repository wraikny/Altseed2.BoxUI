﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Altseed2.BoxUI.Builtin
{
    public enum Column
    {
        X,
        Y,
    }

    public enum Mergin
    {
        Fixed,
        Relative,
        RelativeMin,
        RelativeMax,
    }

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

        public static void Validate(Mergin scale)
        {
            switch(scale)
            {
                case Mergin.Fixed: return;
                case Mergin.Relative: return;
                case Mergin.RelativeMin: return;
                case Mergin.RelativeMax: return;
                default:
                    throw new InvalidEnumArgumentException(nameof(scale), (int)scale, typeof(Mergin));
            }
        }

        public static void Validate(Align align)
        {
            if (align == Align.Min || align == Align.Center || align == Align.Max) return;
            throw new InvalidEnumArgumentException(nameof(align), (int)align, typeof(Align));
        }
    }
}
