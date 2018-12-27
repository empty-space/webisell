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
    public class GetFiltersViewModelCommand : BaseMongoCommand
    {
        public GetFiltersViewModelCommand(ISettingsProvider settings) : base(settings)
        {
        }

        public async Task<CommandResult<FilterPageViewModel>> Run(FilterPageViewModel vm)
        {
            var result = new FilterPageViewModel
            {
                Category = vm.Category
            };
            result.Filters.Add(
                    new FilterViewModel
                    {
                        FilterType = EFilterType.Range,
                        Name = "price",
                        Min = 1,
                        Max = 100500
                    });

            switch (vm.Category)
            {
                case "phone":
                    FillPhoneFilters(result);
                    break;
                case "books":
                    FillBookFilters(result);
                    break;
            }

            return WrapResult(result);
        }
        private void FillBookFilters(FilterPageViewModel result)
        {
            result.Filters.AddRange(new List<FilterViewModel>
                {
                    new FilterViewModel
                    {
                        FilterType = EFilterType.Multiple_OR,
                        Name = "manufacturer",
                        Values = new List<string>{"Piter","Stenly","Abababalamaga","Garage"}
                    },
                    new FilterViewModel
                    {
                        FilterType = EFilterType.Multiple_OR,
                        Name = "genre",
                        Values = new List<string>{"Action and adventure", "Alternate history", "Anthology", "Chick lit", "Children's literature", "Comic book", "Coming-of-age", "Crime", "Drama", "Fairytale", "Fantasy", "Graphic novel", "Historical fiction", "Horror", "Mystery", "Paranormal romance", "Picture book", "Poetry", "Political thriller", "Romance", "Satire", "Science fiction", "Short story", "Suspense", "Thriller", "Young adult"
                        }
                    }
                });
        }
        private void FillPhoneFilters(FilterPageViewModel result)
        {
            result.Filters.AddRange(new List<FilterViewModel>
                {
                    new FilterViewModel
                    {
                        FilterType = EFilterType.Multiple_OR,
                        Name = "manufacturer",
                        Values = new List<string>{
                           "Samsung","Blackberry","Apple","Htc"
                        }
                    },
                    //new FilterViewModel
                    //{
                    //    FilterType = EFilterType.Ranges,
                    //    Name = "camera_resolution",
                    //    Values = new List<string>{
                    //        "5-12",
                    //        "12-20",
                    //        "20+"
                    //    }
                    //},
                    new FilterViewModel
                    {
                        FilterType = EFilterType.Multiple_OR,
                        Name = "color",
                        Values = new List<string>{ "red", "blue", "black", "white", "green", "yellow", "pink", "silver" }
                    },
                    new FilterViewModel
                    {
                        FilterType = EFilterType.Multiple_OR,
                        Name = "internal_storage_size",
                        Values = new List<string>{
                            "32",
                            "64",
                            "128",
                            "256" }
                    },
                    new FilterViewModel
                    {
                        FilterType = EFilterType.Multiple_AND,
                        Name = "connection_standart",
                        Values = new List<string>{ "2G", "3G", "4G", "CDMA" }
                    },
                    new FilterViewModel
                    {
                        FilterType = EFilterType.Multiple_OR,
                        Name = "operating_system",
                        Values = new List<string>{ "Android","IOS","BlackberryOS"}
                    },
                });
        }
    }
}
