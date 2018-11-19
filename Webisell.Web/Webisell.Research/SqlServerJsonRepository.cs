using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using Webisell.Domain.Entities;
using Webisell.Research.EF;

namespace Webisell.Research
{
    class SqlServerJsonRepository : ITableRepository
    {
        const string CategoryName = "TestCategory";
        int CategoryId = -1;

        private WebisellSqlServerDbContext _dbContext = new WebisellSqlServerDbContext();
        public SqlServerJsonRepository()
        {
            SeedDatabase();
        }

        public void SeedDatabase()
        {
            var category = _dbContext.Categories.FirstOrDefault(c => c.Name == CategoryName);
            if (category==null)
            {
                category = new Category { Name = CategoryName };
                var filters = new List<Filter> {
                    new Filter{Name="color",Category=category, FilterTypeId=1},
                    new Filter{Name="operating_system",Category=category, FilterTypeId=1},
                    new Filter{Name="manufacturer",Category=category, FilterTypeId=1},
                };
                _dbContext.Categories.Add(category);
                _dbContext.Filters.AddRange(filters);
                _dbContext.SaveChanges();
                CategoryId = category.CategoryId;
            }
            CategoryId = category.CategoryId;
            var jsonData = @"{ 
                                ""price"":""1005"",	
	                            ""manufacturer"":""Apple"",	
	                            ""operating_system"":""iOS"",
	                            ""camera_resolution"":24, 
	                            ""screen_size"":5, 
	                            ""color"":""white"",
	                            ""features"":[""smartphone"",""excellent camera""],
	                            ""internal_storage_size"": 32, 
	                            ""battery"": 1750,
	                            ""connection_standart"": [""2G"",""3G"",""4G"",""LTE""]
                            }";
            InsertProduct(CategoryId, "Seed name", jsonData);
        }

        int InsertProduct(int categoryId, string name, string jsonData)
        {
            using (var connection = _dbContext.Database.GetDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText =
                        $"exec Insert_Product_category_id {categoryId},'{name}','{jsonData}' \n" + 
                        $"SELECT  @@IDENTITY";
                    return Convert.ToInt32((decimal)command.ExecuteScalar());
                }
            }
        }      

        public void DeleteAll()
        {
            Thread.Sleep(1);
        }

        public void InsertMany(int count)
        {
            Thread.Sleep(1);
        }

        public void NextSearch()
        {
            Thread.Sleep(1);
        }
    }
}
