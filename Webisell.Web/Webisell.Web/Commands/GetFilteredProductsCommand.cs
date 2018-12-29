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
    public class GetFilteredProductsCommand : BaseMongoCommand
    {
        public GetFilteredProductsCommand(ISettingsProvider settings) : base(settings)
        {
        }

        public async Task<CommandResult<FilteredProductsViewModel>> Run(FilterProductsViewModel search)
        {
            var sort = new BsonDocument(search.Sorting.Field, search.Sorting.Direction);
            var filteredItems = await (GetFilteredTask(search)
                .Sort(sort)
                .Skip(search.skip)
                .Limit(search.PageSize)
                .ToListAsync());
            
            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var result = new FilteredProductsViewModel
            {
                FilteredResults = filteredItems.ToJson(jsonWriterSettings),
                Count = await GetFilteredTask(search).CountDocumentsAsync()
            };
            return WrapResult(result);
        }


        protected IFindFluent<BsonDocument, BsonDocument> GetFilteredTask(FilterProductsViewModel search)
        {
            var collection = GetCollection(search.CollectionName);
            var filter = BuildFilters(search);

            return collection.Find(filter);
        }

        private BsonDocument BuildFilters(FilterProductsViewModel search)
        {
            BsonDocument filter = new BsonDocument();
            if (search.Filters != null)
            {
                var andArray = new BsonArray();
                andArray.Add(new BsonDocument("category", search.Category));
                andArray.AddRange( search.Filters?.Where(f => f.IsFilled).Select(FilterToBson));
                filter = new BsonDocument("$and", andArray);
            }
            return filter;
        }
        private BsonDocument FilterToBson(SingleFilterViewModel filter)
        {
            switch (filter.FilterType)
            {
                case EFilterType.Range:
                    return new BsonDocument(filter.Name, new BsonDocument {
                        {"$gte", filter.Min },
                        {"$lte", filter.Max }
                    });
                case EFilterType.Ranges:
                    throw new Exception("TODO");
                case EFilterType.Multiple_OR:
                    return new BsonDocument(filter.Name, new BsonDocument("$in", new BsonArray(filter.Values)));
                case EFilterType.Multiple_AND:
                default:
                    //db.inventory.find( { tags: { $all: ["red", "blank"] } } )
                    return new BsonDocument(filter.Name, new BsonDocument("$all", new BsonArray(filter.Values)));
            }
        }

    }
}