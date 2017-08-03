using System.Collections.Generic;

namespace BGP.Utils.Test.Dtos
{
    public class PostDto : Repository.Mongo.MongoEntity
    {
        public int PostID { get; set; }
        public int CategoryID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int PostedByUserId { get; set; }

        public List<CommentDto> Comments { get; set; }
        public CategoryDto Category { get; set; }
        public UserDto PostedByUser { get; set; }
    }
}
