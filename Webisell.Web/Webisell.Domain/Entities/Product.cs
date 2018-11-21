using System;
using System.Collections.Generic;
using System.Text;

namespace Webisell.Domain.Entities
{
    public class Product
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }

        //Should be mapped dynamically (possibly FromSql):
        public int SpecificTableProductId { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public bool Available { get; set; }
        public string JsonData { get; set; }
    }
}
