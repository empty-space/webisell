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
    partial class SqlServerJsonRepository : ITableRepository
    {
        private int SeedCategoryId =0;

        private string ConnectionString = ConfigurationManager.ConnectionStrings["Webisell.Database"].ConnectionString;
        private WebisellSqlServerDbContext _dbContext = new WebisellSqlServerDbContext();

        public SqlServerJsonRepository()
        {
            SeedDatabase();
        }

        public void SeedDatabase()
        {            
            //----
            var categoryId = CreateCategoryAndCorrespondingTable(new Category
            {
                Name = "Seed category " + DateTime.Now,
                Filters = new List<Filter>
                {
                    new Filter
                    {
                        FilterTypeId= (int)EFilterType.FilterValue_OR,
                        Name = "Manufacturer"
                    },
                    new Filter
                    {
                        FilterTypeId= (int) EFilterType.Integer_Range,
                        Name = "price"
                    },
                    new Filter
                    {
                        FilterTypeId= (int) EFilterType.FilterValue_OR,
                        Name = "color"
                    },
                    new Filter
                    {
                        FilterTypeId= (int) EFilterType.FilterValue_OR,
                        Name = "internal_storage_size"
                    },
                    new Filter
                    {
                        FilterTypeId= (int) EFilterType.MultipleColumns_AND,
                        Name = "connection_standart",
                        FilterValues = new List<FilterValue>
                        {
                            new FilterValue { Value= "2G"},
                            new FilterValue { Value= "3G"},
                            new FilterValue { Value= "4G"},
                            new FilterValue { Value= "CDMA"}
                        }
                    }
                }
            });
            SeedCategoryId = categoryId;
            _dbContext.SaveChanges();
            //
            var category = _dbContext.Categories.FirstOrDefault(c => c.CategoryId == categoryId);
            if (category != null)
            {
                var jsonData = @"{ 
	                            ""manufacturer"":""Apple"",	
                                ""price"":""1005"",	
	                            ""operating_system"":""iOS"",
	                            ""camera_resolution"":24, 
	                            ""screen_size"":5, 
	                            ""color"":""white"",
	                            ""features"":[""smartphone"",""excellent camera""],
	                            ""internal_storage_size"": 32, 
	                            ""battery"": 1750,
	                            ""connection_standart"": [""2G"",""3G"",""4G"",""LTE""]
                            }";
                for (int i = 0; i < 5; i++)
                    InsertProduct(categoryId, "Seed product name", jsonData);
            }
            //
            SearchProductsExample(categoryId);
        }

          
        int InsertProduct(int categoryId, string name, string jsonData)
        {
            using (var context = new WebisellSqlServerDbContext())
            {
                using (var connection = context.Database.GetDbConnection())
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText =
                            $"exec Insert_Product_{categoryId} '{name}','{jsonData}' \n" +
                            $"SELECT  @@IDENTITY";
                        //TODO why decimal?
                        return Convert.ToInt32((decimal)command.ExecuteScalar());
                    }
                }
            }
        }

        

        public void DeleteAll()
        {
            Thread.Sleep(1);
        }

        public void InsertMany(int count)
        {
            var category = _dbContext.Categories.FirstOrDefault(c => c.CategoryId == SeedCategoryId);
            if (category != null)
            {
                var r = new Random(DateTime.Now.Millisecond);
                var connection = new List<string> {"2G","3G","4G","LTE" };
                var connections = "\"" + string.Join("\",\"", connection.Take(r.Next(connection.Count-1)+1)) + "\"";
                for (int i = 0; i < count; i++)
                {
                    var jsonData = $@"{{ 
	                            ""manufacturer"":""Apple"",	
                                ""price"":""1005"",	
	                            ""operating_system"":""iOS"",
	                            ""camera_resolution"":24, 
	                            ""screen_size"":{4+r.Next(5)}, 
	                            ""color"":""white"",
	                            ""features"":[""smartphone"",""excellent camera""],
	                            ""internal_storage_size"": {16*2^r.Next(5)}, 
	                            ""battery"": {1000+ 100 * r.Next(51)},
	                            ""connection_standart"": [""2G"",""3G"",""4G"",""LTE""]
                            }}";
                    InsertProduct(category.CategoryId, "Seed product name", jsonData);
                }
            }
        }

        public void NextSearch()
        {
            Thread.Sleep(1);
        }
    }
}
