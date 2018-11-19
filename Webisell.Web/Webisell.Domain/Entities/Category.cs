using System;
using System.Collections.Generic;
using System.Text;

namespace Webisell.Domain.Entities
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public ICollection<Filter> Filters { get; set; }
    }
}
