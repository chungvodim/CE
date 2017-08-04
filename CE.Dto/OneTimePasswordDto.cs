using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Dto
{
    public class OneTimePasswordDto
    {
        public int OneTimePasswordID { get; set; }
        public int UserID { get; set; }
        public Guid OTPKey { get; set; }
        public Int16 ExpirationMinutes { get; set; }
        public DateTime DateFirstLogin { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByID { get; set; }
    }
}
