using System.Collections.Generic;

namespace BGP.Utils.Test.Entities
{
    [Repository.Mongo.CollectionName("products")]
    public class Post : Repository.Mongo.MongoEntity
    {
        public int PostID { get; set; }
        public int CategoryID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int PostedByUserId { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Category Category { get; set; }
        public virtual User PostedByUser { get; set; }
    }
}
