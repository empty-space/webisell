using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webisell.Web.Enums;

namespace Webisell.Web.ViewModels
{

    public class SingleFilterViewModel
    {
        public EFilterType FilterType { get; set; }
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public int? Min { get; set; }
        public int? Max { get; set; }

        public bool IsFilled
        {
            get
            {
                return Values != null && Values.Count > 0
                    || FilterType == EFilterType.Range && Min != null && Max != null;
            }
        }
    }
    public class Sort
    {
        public string Field { get; set; }
        public int Direction { get; set; }
    }
    public class FilterProductsViewModel
    {
        static int MaxItemsPerPage = 100;//todo

        private int _page;
        private int _pageSize;

        public int Page { get => _page; set { if (value > 0) _page = value; } } //todo else
        public int PageSize { get => _pageSize; set { if (value <= MaxItemsPerPage) _pageSize = value; } } //todo else

        public string CollectionName { get; set; }
        public string Category { get; set; }
        public List<SingleFilterViewModel> Filters { get; set; } = new List<SingleFilterViewModel>();
        public Sort Sorting { get; set; }

        [JsonIgnore]
        public int skip => PageSize * (Page - 1);
    }
}
