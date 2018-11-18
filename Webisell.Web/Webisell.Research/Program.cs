using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Webisell.Research
{
    class Program
    {
        static void Main(string[] args)
        {
            var repositories = new List<ITableRepository> { new SqlServerJsonRepository() };
            foreach (var repo in repositories)
            {
                Console.WriteLine(repo.GetType().Name);
                var result = ResearchUtil.MeasureSearchTime(repo);
                Console.WriteLine($"{result.AvgTimeMilliseconds}");
                Console.WriteLine(new string('=', 20));
            }
            Console.ReadKey();            
        }
    }

    
}
