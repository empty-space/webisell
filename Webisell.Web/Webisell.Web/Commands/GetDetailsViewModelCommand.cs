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
    public class GetDetailsViewModelCommand : BaseMongoCommand
    {
        public GetDetailsViewModelCommand(ISettingsProvider settings) : base(settings)
        {
        }

        public async Task<CommandResult<ProductDetailsViewModel>> Run(GetProductDetailsViewModel vm)
        {
            var collection = GetCollection(vm.CollectionName ?? "products");
            var x = await collection.Find(new BsonDocument { { "product_id", Int64.Parse(vm.Id) } }).FirstOrDefaultAsync();
            var result = new ProductDetailsViewModel
            {
                Id = vm.Id,
                Name = x["product_name"].ToString()
            };
            
            return WrapResult(result);
        }

    }
}
