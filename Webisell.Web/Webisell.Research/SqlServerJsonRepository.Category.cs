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
        //todo
        //public int AddCategoryFilters(List<Filter> filters)
        //{
        //    //Alter table Product_{categoryId}
        //    //Regenerate Procedure Insert_Procudct_{categoryId}
        //    //NOTE:Prodedure Update_product will be skipped during this work
        //}

        public int CreateCategoryAndCorrespondingTable(Category categoryWithFilters)
        {
            using (var transactionHelper = new TransactionHelper(ConnectionString))
            {
                transactionHelper.RunInTransaction(context => { context.Categories.Add(categoryWithFilters); });
                transactionHelper.RunInTransaction(GetCreateCategoryTableSql(categoryWithFilters));
                transactionHelper.RunInTransaction(GetCreateProductInsertProcedureSql(categoryWithFilters));
                transactionHelper.Commit();
            }
            return categoryWithFilters.CategoryId;
        }


        private string GetCreateCategoryTableSql(Category category)
        {
            var categoryId = category.CategoryId;
            var columnsFromFilters =
                string.Join("\n",
            category.Filters
        .Where(f => !f.IsSystem)
        .Select(f => GetColumnsSql(f)));

            var sql = $@"
CREATE TABLE dbo.Products_{categoryId}(
    Id int IDENTITY(1,1) PRIMARY KEY, 
    ProductId int FOREIGN KEY  REFERENCES Products(ProductId), 
    Name nvarchar(250) NOT NULL,
    Available bit DEFAULT 1,
    Rating int DEFAULT 0,
    --for Product detail page and comparison page
    Data nvarchar(max) NULL --json
    --Filters:,
    {columnsFromFilters}
)";
            return sql;
        }

        private string GetColumnsSql(Filter filter)
        {
            switch ((EFilterType)filter.FilterTypeId)
            {
                case EFilterType.Integer_Range:
                case EFilterType.Integer_OR:
                    return $",{filter.Name} AS CONVERT(int, JSON_VALUE(Data, '$.{filter.JsonFieldName}')) PERSISTED";
                case EFilterType.FilterValue_OR:
                    return $",{filter.Name} int FOREIGN KEY  REFERENCES FilterValues(FilterValueId)";
                case EFilterType.MultipleColumns_AND:
                case EFilterType.MultipleColumns_OR:
                    var multiValueColumns = "";
                    //every FilterVaue becomes Column
                    //EXAMPLE: connection_standart_2g AS CASE WHEN   (JSON_QUERY(Data, '$.connection_standart') like '%2g%') THEN 1 ELSE 0 END  Persisted                
                    multiValueColumns = string.Join("",
                    filter.FilterValues.Select(fv =>
        $"\n,{filter.Name}_{fv.Value} AS " +
        $"CASE WHEN (JSON_QUERY(Data, '$.{filter.JsonFieldName}') like '%{fv.Value}%') THEN 1 ELSE 0 END  PERSISTED"));

                    return multiValueColumns;
                default:
                    return string.Empty;
            }
        }

        private string GetCreateProductInsertProcedureSql(Category categoryWithFilters)
        {
            var additionalColumnsSql = string.Join("\n",
            categoryWithFilters.Filters
        .Select(f => GetColumnSqlForInsertProcedure(f)));
            var columnsNamesForInsert = string.Join(",",
            categoryWithFilters.Filters
        .Where(f => f.FilterTypeId == (int)EFilterType.FilterValue_OR)
        .Select(f => f.Name));
            var columnsValuesForInsert = string.Concat(categoryWithFilters.Filters
                    .Where(f => f.FilterTypeId == (int)EFilterType.FilterValue_OR)
                    .Select(f => $",@{f.Name}Id"));

            var sql = $@"
CREATE PROCEDURE Insert_Product_{categoryWithFilters.CategoryId} (
 @name nvarchar(50),
 @data nvarchar(max) --json   
)  AS  
BEGIN  	
	DECLARE @categoryId int = {categoryWithFilters.CategoryId}
	--Product
	INSERT INTO dbo.Products(CategoryId) 
	VALUES (@categoryId)
	DECLARE @ProductID int = @@IDENTITY	
	--Filters
	{additionalColumnsSql}	

	--Product_X
	INSERT INTO Products_{categoryWithFilters.CategoryId} (ProductId,Name,Data,{columnsNamesForInsert})
	VALUES(
		@ProductID,
		@name,
		@data
		{columnsValuesForInsert}
	)	
END ";
            return sql;
        }

        private string GetColumnSqlForInsertProcedure(Filter filter)
        {
            switch ((EFilterType)filter.FilterTypeId)
            {
                case EFilterType.FilterValue_OR:
                    var fname = filter.Name;
                    return $@"
DECLARE @{fname} nvarchar(max) = JSON_VALUE(@data, '$.{filter.JsonFieldName}') 
DECLARE @{fname}Id int = NULL
exec Find_or_Create_FilterValue @categoryId,'{fname}', @{fname},@{fname}Id OUTPUT";
                default:
                    return string.Empty;
            }
        }

    }
}
