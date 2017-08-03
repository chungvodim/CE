namespace BGP.Utils.Test.Dtos
{
    public class CommentDto : Repository.Mongo.MongoEntity
    {
        public int CommentID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int PostID { get; set; }
        public int PostedByUserID { get; set; }

        public virtual PostDto Post { get; set; }
        public virtual UserDto PostedByUser { get; set; }
    }
}
