﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Altseed2.BoxUI.Elements
{
    [Serializable]
    public enum ColumnDir
    {
        X,
        Y,
    }

    [Serializable]
    public enum MarginScale
    {
        Fixed,
        Relative,
        RelativeMin,
        RelativeMax,
    }

    [Serializable]
    public enum AlignPos
    {
        Min,
        Center,
        Max,
    }

    public static class FlagsValidater
    {
        public static void Validate(ColumnDir dir)
        {
            if (dir == ColumnDir.X || dir == ColumnDir.Y) return;
            throw new InvalidEnumArgumentException(nameof(dir), (int)dir, typeof(ColumnDir));
        }

        public static void Validate(MarginScale scale)
        {
            switch(scale)
            {
                case MarginScale.Fixed: return;
                case MarginScale.Relative: return;
                case MarginScale.RelativeMin: return;
                case MarginScale.RelativeMax: return;
                default:
                    throw new InvalidEnumArgumentException(nameof(scale), (int)scale, typeof(MarginScale));
            }
        }

        public static void Validate(AlignPos align)
        {
            if (align == AlignPos.Min || align == AlignPos.Center || align == AlignPos.Max) return;
            throw new InvalidEnumArgumentException(nameof(align), (int)align, typeof(AlignPos));
        }
    }
}