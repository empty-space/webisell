using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webisell.Web.Configuration;

namespace Webisell.Web.Commands
{
    public class BaseMongoCommand:BaseCommand
    {
        protected ISettingsProvider _settings;
        //#if DEBUG
        //        public BaseMongoCommand() : this("mongodb+srv://devr:00FF00_00@clusterdev0-cvkvj.mongodb.net/test?retryWrites=true")
        //        {

        //        }
        //#endif
        public BaseMongoCommand(ISettingsProvider settings)
        {
            _settings = settings;
        }
        protected MongoClient GetMongoClient() => new MongoClient(_settings.MongoConnectionString);
        
        protected IMongoCollection<BsonDocument> GetCollection(string name)
        {
            var client = GetMongoClient();
            IMongoDatabase database = client.GetDatabase("test");
            return database.GetCollection<BsonDocument>(name);
        }
    }
}
