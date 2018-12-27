//using MongoDB.Bson;
//using MongoDB.Bson.IO;
//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Webisell.Web.ViewModels;

//namespace Webisell.Web.DAL
//{
//    public class MongoWrapper
//    {
//        string connectionString = "mongodb+srv://devr:00FF00_00@clusterdev0-cvkvj.mongodb.net/test?retryWrites=true";

//        public MongoWrapper()
//        {
//        }
        
//        public async Task<string> Test()
//        {
//            MongoClient client = new MongoClient(connectionString);
//            IMongoDatabase database = client.GetDatabase("test");
//            var collection = database.GetCollection<BsonDocument>("phones");
//            var result = await collection.CountDocumentsAsync(new BsonDocument());
//            return result.ToString();
//        }
//        public async Task<string> OldFind(SearchViewModel search)
//        {
//            MongoClient client = new MongoClient(connectionString);
//            IMongoDatabase database = client.GetDatabase("test");
//            var collection = database.GetCollection<BsonDocument>(search.collectionName);
//            // поиск документов, в которых Name="Bill"
//            var filter = BuildFilters(search);
//            var sort = new BsonDocument(search.sort.Item1, search.sort.Item2);
//            var result = await collection.Find(filter).Sort(sort).Skip(search.skip).Limit(search.page_size).ToListAsync();
//            result.ForEach(d => { d["_id"] = BsonNull.Value; });
//            return result.ToJson();
//        }
//        public async Task<string> Find(SearchViewModel search)
//        {
//            MongoClient client = new MongoClient(connectionString);
//            IMongoDatabase database = client.GetDatabase("test");
//            var collection = database.GetCollection<BsonDocument>(search.collectionName);
//            // поиск документов, в которых Name="Bill"
//            var filter = BuildFilters(search);
//            var sort = new BsonDocument(search.sort.Item1, search.sort.Item2);
//            var result = await collection.Find(filter).Sort(sort).Skip(search.skip).Limit(search.page_size).ToListAsync();
//            //result.ForEach(d => { d["_id"] = BsonNull.Value; });
//            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };
//            return result.ToJson(jsonWriterSettings);
//        }

//        private BsonDocument BuildFilters(SearchViewModel search)
//        {
//            BsonDocument filter = new BsonDocument();
//            if(search.filters!=null)
//                filter = new BsonDocument("$and",
//                    new BsonArray(search.filters?.Select(FilterToBson)));
//            return filter;
//        }
//        private BsonDocument FilterToBson(SearchFilter filter)
//        {
//            switch (filter.FilterType)
//            {
//                case EFilterType.Single_OR:
//                    return new BsonDocument(filter.Name, filter.Matches[0]);
//                case EFilterType.Multiple_OR:
//                    return new BsonDocument(filter.Name, new BsonDocument("$in", new BsonArray(filter.Matches)));
//                case EFilterType.Multiple_AND:
//                    //db.inventory.find( { tags: { $all: ["red", "blank"] } } )
//                    return new BsonDocument(filter.Name, new BsonDocument("$all", new BsonArray(filter.Matches)));
//                case EFilterType.Range:
//                    break;
//                case EFilterType.Ranges:
//                    break;
//                default:
//                    break;
//            }
//            throw new Exception("TODO");
//        }
//    }
//}
