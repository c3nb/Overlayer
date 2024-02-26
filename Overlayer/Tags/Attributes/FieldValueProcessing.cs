using System;

namespace Overlayer.Tags.Attributes
{
    [Flags]
    public enum FieldValueProcessing
    {
        None = 0,
        RoundNumber = 1 << 0,
        TrimString = 1 << 1,
    }
}
