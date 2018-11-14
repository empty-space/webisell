using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webisell.Web.ViewModels
{
    public class FilterPageViewModel
    {
        //Example
        public List<FilterViewModel> Filters = new List<FilterViewModel>
        {
            new FilterViewModel
            {
                Name = "Brand",
                AllowedValues = new List<string>{
                    "Apple",
                    "Nokia",
                    "Samsung",
                    "Sony"
                }
            },
            new FilterViewModel
            {
                Name = "Camera",
                AllowedValues = new List<string>{
                    "2-8",
                    "8-12",
                    "12+"
                }
            }
        };
    }

    public class FilterViewModel
    {
        public String Name { get; set; }
        public List<String> AllowedValues;
    }
}
