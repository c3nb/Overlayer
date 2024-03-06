using System;

namespace Overlayer.Tags.Attributes
{
    [Flags]
    public enum ValueProcessing
    {
        None = 0,
        RoundNumber = 1 << 0,
        TrimString = 1 << 1,
        AccessMember = 1 << 2,
    }
}
