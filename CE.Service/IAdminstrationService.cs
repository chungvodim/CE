using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE.Enum;

namespace CE.Service
{
    public interface IAdminstrationService
    {
        UserRole[] Roles { get; set; }
        int UserID { get; set; }
    }
}
