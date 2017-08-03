using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BGP.Utils.Repository.Mongo
{
    /// <summary>
    /// mongo entity interface
    /// </summary>
    public interface IMongoEntity
    {

        /// <summary>
        /// id in string format
        /// </summary>
        [BsonId]
        string Id { get; set; }

        /// <summary>
        /// id in objectId format
        /// </summary>
        [BsonIgnore]
        ObjectId ObjectId { get; }
    }
}