using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CE.Repository.Mongo
{
    public class MongoDatabaseFactory
    {
        public static IMongoDatabase Create(string connectionName = "Mongo")
        {
            var url = new MongoUrl(ConfigurationManager.ConnectionStrings[connectionName].ConnectionString);
            var client = new MongoClient(url);
            var db = client.GetDatabase(url.DatabaseName);

            return db;
        }
    }
}
