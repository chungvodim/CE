namespace BGP.Utils.Test.Entities
{
    [Repository.Mongo.CollectionName("shippers")]
    public class Comment : Repository.Mongo.MongoEntity
    {
        public int CommentID { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public int PostID { get; set; }
        public int PostedByUserID { get; set; }

        public virtual Post Post { get; set; }
        public virtual User PostedByUser { get; set; }
    }
}
