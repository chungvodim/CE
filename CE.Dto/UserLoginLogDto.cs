using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Dto
{
    public class UserLoginLogDto
    {
        public int UserLoginLogID { get; set; }
        public string Login { get; set; }
        public int? UserID { get; set; }
        public DateTime LogDate { get; set; }
        public int IsSuccess { get; set; }
        public string RemoteIP { get; set; }
    }
}
