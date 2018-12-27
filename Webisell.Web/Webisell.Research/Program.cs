using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Webisell.Research.Mongo;

namespace Webisell.Research
{
    class Program
    {
        static void Main(string[] args)
        {
            var mongodb = new MongoWrapper();
            var phones  = mongodb.Find(new MongoWrapper.SearchViewModel
            {
                collectionName = "phones",
                filters = new List<MongoWrapper.SearchFilter>
                {
                    new MongoWrapper.SearchFilter
                    {
                        FilterType = MongoWrapper.EFilterType.Multiple_OR,
                        Name="manufacturer",
                        Matches =  new List<string>{"Apple" }
                    }
                }
            });
                        
            Console.WriteLine(phones.Result);
            Console.ReadKey();
        }
        //static void Main(string[] args)
        //{
        //    var sqlrepo = new SqlServerJsonRepository();
        //    var repositories = new List<ITableRepository> {sqlrepo };

        //    var products = sqlrepo.GetProducts(0);

        //    foreach (var p in products)
        //    {
        //        Console.WriteLine($"{p.ProductId}\t{p.Name}\t{p.Price}\t{p.Data} ");
        //    }

        //    foreach (var repo in repositories)
        //    {
        //        Console.WriteLine(repo.GetType().Name);
        //        var result = ResearchUtil.MeasureSearchTime(repo);
        //        Console.WriteLine($"{result.AvgTimeMilliseconds}");
        //        Console.WriteLine(new string('=', 20));
        //    }
        //    Console.ReadKey();            
        //}
    }

    
}
