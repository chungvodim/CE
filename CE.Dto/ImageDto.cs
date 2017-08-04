using CE.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Dto
{
    public class ImageDto
    {
        [Key]
        public int ImageID { get; set; }
        public int ObjectID { get; set; }
        public string URL { get; set; }
        public DateTime DateCreated { get; set; }
        public ImageType ObjectType { get; set; }
        public byte ImageIndex { get; set; }
        public bool IsDeleted { get; set; }
        public string S3ID { get; set; }

        public int? ListingID { get; set; }
    }
}
