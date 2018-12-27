using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Webisell.Research.Mongo
{
    class MongoWrapper
    {
        string connectionString = "mongodb+srv://devr:00FF00_00@clusterdev0-cvkvj.mongodb.net/test?retryWrites=true";

        public MongoWrapper()
        {
        }
        public enum EFilterType : int
        {
            Single_OR = 1,
            Multiple_OR,
            Multiple_AND,
            Range,
        }
        public class SearchFilter
        {
            public EFilterType FilterType { get; set; }
            public string Name { get; set; }
            public List<string> Matches { get; set; }
            public int Min { get; set; }
            public int Max { get; set; }
        }
        public class SearchViewModel
        {
            public string collectionName = "phones";
            public List<SearchFilter> filters = null;
            public Tuple<string, int> sort = new Tuple<string, int>("price", -1);
            private int _page = 1;
            private int _pageSize = 25;
            public int page { get => _page; set { if (value > 0) _page = value; } }
            public int page_size { get => _pageSize; set { if (value <= 100) _pageSize = value; } }
            public int skip => page_size * (page - 1);
        }
        public async Task<string> Test()
        {
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase("test");
            var collection = database.GetCollection<BsonDocument>("phones");
            var result = await collection.CountDocumentsAsync(new BsonDocument());
            return result.ToString();
        }
        public async Task<string> Find(SearchViewModel search)
        {
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase("test");
            var collection = database.GetCollection<BsonDocument>(search.collectionName);
            // поиск документов, в которых Name="Bill"
            var filter = BuildFilters(search);
            var sort = new BsonDocument(search.sort.Item1, search.sort.Item2);
            var result = await collection.Find(filter).Sort(sort).Skip(search.skip).Limit(search.page_size).ToListAsync();
            return result.ToJson();
        }

        private BsonDocument BuildFilters(SearchViewModel search)
        {
            var filter = new BsonDocument("$and", 
                new BsonArray(search.filters.Select(FilterToBson)));
            return filter;
        }
        private BsonDocument FilterToBson(SearchFilter filter)
        {
            switch (filter.FilterType)
            {
                case EFilterType.Single_OR:
                    return new BsonDocument(filter.Name, filter.Matches[0]);
                case EFilterType.Multiple_OR:
                    return new BsonDocument(filter.Name, new BsonDocument("$in", new BsonArray(filter.Matches)));
                case EFilterType.Multiple_AND:
                    //db.inventory.find( { tags: { $all: ["red", "blank"] } } )
                    return new BsonDocument(filter.Name, new BsonDocument("$all", new BsonArray(filter.Matches)));
                case EFilterType.Range:
                    break;
                default:
                    break;
            }
            throw new Exception("TODO");
        }
    }
}
