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
}
