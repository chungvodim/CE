using BGP.Utils.Common;
using System;

namespace CE.Enum
{
    [Serializable]
    public enum UserStatus : byte
    {
        [IgnoredEnum]
        All = 0,
        Active = 1,
        Inactive = 2
    }
}
