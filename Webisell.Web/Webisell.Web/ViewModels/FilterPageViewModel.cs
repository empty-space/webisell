using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webisell.Web.Enums;

namespace Webisell.Web.ViewModels
{
    public class FilterPageViewModel
    {
        public string Category { get; set; }
        //Example
        public List<FilterViewModel> Filters { get; set; } = new List<FilterViewModel>();
        //    = new List<FilterViewModel>
        //{
        //    new FilterViewModel
        //    {
        //        FilterType = EFilterType.Range,
        //        Name = "price",
        //        Min = 1,
        //        Max = 100500
        //    },
        //    new FilterViewModel
        //    {
        //        FilterType = EFilterType.Multiple_OR,
        //        Name = "manufacturer",
        //        Values = new List<string>{
        //            "Apple",
        //            "Nokia",
        //            "Samsung",
        //            "Sony"
        //        }
        //    },
        //    new FilterViewModel
        //    {
        //        FilterType = EFilterType.Ranges,
        //        Name = "camera_resolution",
        //        Values = new List<string>{
        //            "5-12",
        //            "12-20",
        //            "20+"
        //        }
        //    }
        //};
    }

    public class FilterViewModel
    {
        public EFilterType FilterType { get; set; }
        public string Name { get; set; }
        public List<string> Values { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
    }
}
