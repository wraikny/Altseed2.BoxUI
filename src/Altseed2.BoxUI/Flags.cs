using System;

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
}
