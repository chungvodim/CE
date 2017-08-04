using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CE.Enum;

namespace CE.Entity.Main
{
    [Table("Images")]
    public class Image
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }
        public int ObjectID { get; set; }
        [StringLength(200)]
        public string S3ID { get; set; }
        [StringLength(1000)]
        public string URL { get; set; }
        public DateTime DateCreated { get; set; }
        public ImageType ObjectType { get; set; }
        public byte ImageIndex { get; set; }
        public bool IsDeleted { get; set; }
    }
}
