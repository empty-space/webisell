//using MongoDB.Bson;
//using MongoDB.Driver;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Webisell.Persistent.Mongo
//{
//    class MongoWrapper
//    {
//        public MongoWrapper()
//        {
//        }
//        public enum EFilterType : int
//        {
//            Single_OR = 1,
//            Multiple_OR,
//            Multiple_AND,            
//            Range,
//        }
//        public class SearchFilter
//        {
//            public EFilterType FilterType { get; set; }
//            public string Name { get; set; }
//            public string Match { get; set; }
//            public List<string> Matches { get; set; }
//            public int Min { get; set; }
//            public int Max { get; set; }
//        }
//        public class SearchViewModel
//        {
//            public string collectionName = "phones";
//            public List<SearchFilter> filters = null;
//            public Tuple<string, int> sort = new Tuple<string, int>("price", -1);
//            private int _page= 1;
//            private int _pageSize = 25;
//            public int page { get => _page; set { if (value > 0) _page = value; } } 
//            public int page_size { get => _pageSize; set { if (value <= 100) _pageSize = value; } } 
//            public int skip => page_size*(page-1);
//        }
//        public string Find(SearchViewModel search)
//        {
//            string connectionString = "mongodb://localhost:27017";
//            MongoClient client = new MongoClient(connectionString);
//            IMongoDatabase database = client.GetDatabase("test");
//            var collection = database.GetCollection<BsonDocument>(search.collectionName);
//            // поиск документов, в которых Name="Bill"
//            var filter = new BsonDocument("$and", new BsonArray{
//                new BsonDocument("price",new BsonDocument("$gt", 31)),
//                new BsonDocument("Name", "Bill")
//            });
//            var sort = new BsonDocument(search.sort.Item1, search.sort.Item2);
//            return collection.Find(filter).Sort(sort).Skip(search.skip).Limit(search.page_size).ToJson();
//        }

//        private BsonDocument BuildFilters(SearchViewModel search) {
//            var filters = new BsonArray{
//                new BsonDocument("price",new BsonDocument("$gt", 31)),
//                new BsonDocument("Name", "Bill")
//            };
//            var filter = new BsonDocument("$and", filters );
//            return filter;
//        }
//        private BsonDocument FilterToBson(SearchFilter filter) {
//            switch (filter.FilterType)
//            {
//                case EFilterType.Single_OR:
//                    return new BsonDocument(filter.Name, filter.Match);
//                case EFilterType.Multiple_OR:
//                    return new BsonDocument(filter.Name,new BsonDocument("$in", new BsonArray(filter.Matches)));
//                case EFilterType.Multiple_AND:
//                    //db.inventory.find( { tags: { $all: ["red", "blank"] } } )
//                    return new BsonDocument(filter.Name,new BsonDocument("$all", new BsonArray(filter.Matches)));
//                case EFilterType.Range:
//                    break;
//                default:
//                    break;
//            }
//            throw new Exception("TODO");
//        }
//    }
//}
