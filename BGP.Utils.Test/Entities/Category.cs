namespace BGP.Utils.Test.Entities
{
    [Repository.Mongo.CollectionName("categories")]
    public class Category : Repository.Mongo.MongoEntity
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
