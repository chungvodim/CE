using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace BGP.Utils.Test.Dtos
{
    public class UserDto : Repository.Mongo.MongoEntity
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [BsonIgnore]
        public string FullName { get { return FirstName + " " + LastName; } }
        public DateTime DateOfBirth { get; set; }
        public int CompanyID { get; set; }
        public string Phone { get; set; }

        public List<PostDto> Posts { get; set; }
    }
}
