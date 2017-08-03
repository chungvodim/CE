using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Entity.Log
{
    [Table("UserLoginLogs")]
    public class UserLoginLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserLoginLogID { get; set; }
        [Required, StringLength(100)]
        public string Login { get; set; }
        public int? UserID { get; set; }
        public DateTime LogDate { get; set; }
        public int IsSuccess { get; set; }
        [StringLength(39)]
        public string RemoteIP { get; set; }
    }
}
