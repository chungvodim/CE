using BGP.Utils.Common;
using System;
using System.ComponentModel;

namespace CE.Enum
{
    [Serializable]
    public enum UserRole : byte
    {
        [Description("SuperAdmin")]
        SuperAdmin = 1,
        [Description("Vendor")]
        Vendor = 2,
        [Description("Client")]
        Client = 3
    }

    [Serializable]
    public enum ImageType : byte
    {
        Null = 0,
        User = 1,
        Company = 2,
        Listing = 3,
        Overlay = 4,
    }

    [Serializable]
    public enum UserStatus : byte
    {
        [IgnoredEnum]
        All = 0,
        Active = 1,
        Inactive = 2
    }
}
