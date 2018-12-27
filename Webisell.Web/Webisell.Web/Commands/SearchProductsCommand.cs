using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webisell.Web.Configuration;
using Webisell.Web.Enums;
using Webisell.Web.ViewModels;

namespace Webisell.Web.Commands
{
    public class SearchProductsCommand : BaseMongoCommand
    {
        public SearchProductsCommand(ISettingsProvider settings) : base(settings)
        {
        }

        public async Task<CommandResult<string>> Run(SearchProductsViewModel search)
        {
            var client = GetMongoClient();
            IMongoDatabase database = client.GetDatabase("test");
            var collection = database.GetCollection<BsonDocument>(search.collectionName);

            var query = collection
                .Find(new BsonDocument("$text", new BsonDocument("$search", search.searchText)))
                .Limit(_settings.CountOfSearchResults);
            var s = query.ToJson();
            var result = await query
                .ToListAsync();
            
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            return WrapResult(result.ToJson(jsonWriterSettings));
        }
    }
}
