using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Dto
{
    public class UserLogDto
    {
        public int UserLogID { get; set; }
        public string Description { get; set; }
        public string ChangeLog { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedByID { get; set; }

        public UserLogDto()
        {
            CreatedDate = DateTime.UtcNow;
        }
    }
}
