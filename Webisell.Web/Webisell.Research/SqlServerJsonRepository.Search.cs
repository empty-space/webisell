using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Webisell.Domain.Entities;
using Webisell.Domain.Enums;
using Webisell.Research.EF;
using System.Configuration;

namespace Webisell.Research
{
    partial class SqlServerJsonRepository
    {
        void SearchProductsExample(int categoryId)
        {
            List<Filter> filters = null;
            using (var context = new WebisellSqlServerDbContext())
            {
                filters = context.Categories
                    .Include(c => c.Filters)
                    .ThenInclude(f => f.FilterValues)
                    .First(c => c.CategoryId == categoryId)
                    .Filters.ToList();
            }
            var multiColumns = new List<string> { "connection_standart_2G", "connection_standart_3G", };
            var listSearchFilters = new List<SearchFilterDto>()
            {
                GetSearchFilter(filters, "Manufacturer"),
                GetSearchFilter(filters, "price", 0, 200),
                GetSearchFilter(filters, "color"),
                GetSearchFilter(filters, "connection_standart", multiColumns)
            };

            var x = GetProducts(categoryId, listSearchFilters, "price", 0, 20);

        }

        private SearchFilterDto GetSearchFilter(List<Filter> filters, string name)
        {
            var filter = filters.First(f => f.Name == name);
            return new SearchFilterDto
            {
                FilterId = filter.FilterId,
                FilterName = filter.Name,
                FilterTypeId = filter.FilterTypeId,
                Values = filter.FilterValues.Select(fv => fv.FilterValueId).ToList()
            };
        }

        private SearchFilterDto GetSearchFilter(List<Filter> filters, string name, int rangeMin, int rangeMax)
        {
            var filter = GetSearchFilter(filters, name);
            filter.RangeMax = rangeMax;
            filter.RangeMin = rangeMin;
            return filter;
        }

        private SearchFilterDto GetSearchFilter(List<Filter> filters, string name, List<string> multiValues)
        {
            var filter = GetSearchFilter(filters, name);
            filter.MultiColumnNames = multiValues;
            return filter;
        }

        private SearchFilterDto GetSearchFilter(List<Filter> filters, string name, List<int> values)
        {
            var filter = GetSearchFilter(filters, name);
            filter.Values = values;
            return filter;
        }

        public List<Product> GetProducts(int categoryId)
        {
            using (var context = new WebisellSqlServerDbContext())
            {
                return context.Products.FromSql($@"
                    SELECT  ProductId,
		                Id as SpecificTableProductId, 
                        {0} as CategoryId,
		                Name,
		                price as Price,
		                Available,
		                Data
                    FROM Products_{0}
                    ", categoryId)
                    .ToList();

            }
        }


        public string GetProductsSearchSql(int categoryId, List<SearchFilterDto> filters, string orderBy, int skip, int take)
        {
            //todo skip take
            var conditions = string.Join("\n AND \n", filters.Select(f => GetWhereFromFilter(f)));
            conditions = conditions != string.Empty ? $" WHERE {conditions}" : null;
            var sql = $@"
select *,
       Id as SpecificTableProductId,
       {categoryId} as CategoryId
from Products_{categoryId}
{conditions}
ORDER BY {orderBy}
";
            return sql;
        }

        public List<Product> GetProducts(int categoryId, List<SearchFilterDto> filters, string orderBy, int skip, int take)
        {
            var sql = GetProductsSearchSql(categoryId, filters, orderBy, skip, take);

            using (var context = new WebisellSqlServerDbContext())
            {
                return context.Products.FromSql(sql).ToList();
            }
        }

        private string GetWhereFromFilter(SearchFilterDto filter)
        {
            string whereClause = null;
            switch ((EFilterType)filter.FilterTypeId)
            {
                case EFilterType.Integer_Range:
                    whereClause = $"{filter.FilterName} BETWEEN {filter.RangeMin} and {filter.RangeMax}";
                    break;
                case EFilterType.MultipleColumns_OR:
                    whereClause = string.Join(" OR ", filter.MultiColumnNames.Select(s => $"({s}=1)"));
                    break;
                case EFilterType.MultipleColumns_AND:
                    whereClause = string.Join(" AND ", filter.MultiColumnNames.Select(s => $"({s}=1)"));
                    break;
                case EFilterType.Integer_OR:
                case EFilterType.FilterValue_OR:
                    whereClause = $"{filter.FilterName} IN ({string.Join(",", filter.Values)})";
                    break;
            }
            return whereClause == null ? whereClause : $"({whereClause})";
        }
    }
}
