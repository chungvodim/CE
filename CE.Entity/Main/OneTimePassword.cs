using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Entity.Main
{
    [Table("OneTimePasswords")]
    public class OneTimePassword
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OneTimePasswordID { get; set; }
        public int UserID { get; set; }
        public Guid OTPKey { get; set; }
        public Int16 ExpirationMinutes { get; set; }
        public DateTime DateFirstLogin { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedByID { get; set; }
    }
}
