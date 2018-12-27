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
    public class GetFilteredProductsMetaResultCommand : GetFilteredProductsCommand
    {
        public GetFilteredProductsMetaResultCommand(ISettingsProvider settings) : base(settings)
        {
        }

        public async Task<CommandResult<FilteredProductsMetaResult>> Run(FilterProductsViewModel search)
        {
            var result = new FilteredProductsMetaResult
            {
                Count = await GetFilteredTask(search).CountDocumentsAsync()
            };

            var jsonWriterSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            return WrapResult(result);
        }
    }
}
