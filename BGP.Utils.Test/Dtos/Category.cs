namespace BGP.Utils.Test.Dtos
{
    public class CategoryDto : Repository.Mongo.MongoEntity
    {
        public int CategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
