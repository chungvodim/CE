using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BGP.Utils.Repository.Mongo
{
    /// <summary>
    /// mongo entity
    /// </summary>
    [BsonIgnoreExtraElements(Inherited = true)]
    public class MongoEntity : IMongoEntity
    {
        public MongoEntity()
        {
            //Id = ObjectId.GenerateNewId().ToString();
        }

        /// <summary>
        /// id in string format
        /// </summary>
        [BsonElement(Order = 0)]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        /// <summary>
        /// id in objectId format
        /// </summary>
        public ObjectId ObjectId
        {
            get
            {
                //Incase, this is required if db record is null
                if (Id == null)
                    return default(ObjectId);
                    //Id = ObjectId.GenerateNewId().ToString();
                return ObjectId.Parse(Id);
            }
        }

        [BsonIgnore]
        public DateTime CreatedDate
        {
            get
            {
                return ObjectId.CreationTime;
            }
        }
    }
}