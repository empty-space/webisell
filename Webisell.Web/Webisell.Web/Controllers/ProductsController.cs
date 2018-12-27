using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Webisell.Web.Commands;
using Webisell.Web.ViewModels;

namespace Webisell.Web.Controllers
{
    public class ProductsController : Controller
    {   
        public async Task<IActionResult> FilterPage(FilterPageViewModel viewModel, [FromServices]GetFiltersViewModelCommand command)
        {
            var vm = await command.Run(viewModel);

            return View("FilterPage",vm.Result);
        }

        [HttpPost]
        public async Task<IActionResult> FilterProducts([FromForm]FilterProductsViewModel vm, [FromServices]GetFilteredProductsCommand command)
        {
            var result = await command.Run(vm);
            if (result.Type == ECommandResultType.Error)
            {
                ModelState.AddModelError("Error_message", result.ErrorMessage);
                //todo ? return error or no
            }
            return new JsonResult(result.Result);
        }
        [HttpPost]
        public async Task<IActionResult> FilterProductsMeta([FromForm]FilterProductsViewModel vm, [FromServices]GetFilteredProductsMetaResultCommand command)
        {
            var result = await command.Run(vm);
            if (result.Type == ECommandResultType.Error)
            {
                ModelState.AddModelError("Error_message", result.ErrorMessage);
                //todo ? return error or no
            }
            return new JsonResult(result.Result);
        }

        [HttpPost]
        public async Task<IActionResult> SearchProducts([FromBody]SearchProductsViewModel vm, [FromServices]SearchProductsCommand command)
        {
            var result = await command.Run(vm);
            if (result.Type == ECommandResultType.Error)
            {
                ModelState.AddModelError("Error_message", result.ErrorMessage);
                //todo ? return error or no
            }
            return Content(result.Result, "application/json");
        }

        public async Task<IActionResult> Details(GetProductDetailsViewModel vm, [FromServices]GetDetailsViewModelCommand command)
        {
            var result = await command.Run(vm);
            if (result.Type == ECommandResultType.Error)
            {
                ModelState.AddModelError("Error_message", result.ErrorMessage);
                //todo ? return error or no
            }
            return View(result.Result);
        }

#if DEBUG
        public IActionResult ExampleFilter()
        {
            var result = new FilterProductsViewModel()
            {
                Page = 1,
                PageSize = 10,
                Sorting = new Sort { Field = "price", Direction = -1 },
                CollectionName = "phones",
                Filters = new List<SingleFilterViewModel>
                {
                    new SingleFilterViewModel
                    {
                        FilterType = Enums.EFilterType.Multiple_OR,
                        Name="manufacturer",
                        Values = new List<string>{"Apple","Blackberry"}
                    }
                }
            };

            return Json(result);
        }

        public IActionResult ExampleSearch()
        {
            var result = new SearchProductsViewModel()
            {
                collectionName = "phones",
                searchText = "Samsung S9"
            };
            return Json(result);
        }
#endif

        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Edit(int id)
        {
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
