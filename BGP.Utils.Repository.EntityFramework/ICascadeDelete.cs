using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGP.Utils.Repository.EntityFramework
{
    public interface ICascadeDelete
    {
        void OnDelete();
    }
}
