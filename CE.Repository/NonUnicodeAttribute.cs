using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Repository
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NonUnicodeAttribute : Attribute
    {
    }
}
