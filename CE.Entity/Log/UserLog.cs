using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CE.Entity.Log
{
    [Table("UserLogs")]
    public class UserLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserLogID { get; set; }
        //public int CompanyID { get; set; }
        [StringLength(200)]
        public string Description { get; set; }
        public string ChangeLog { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByID { get; set; }
    }
}
