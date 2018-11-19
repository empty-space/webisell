using System;
using System.Collections.Generic;
using System.Text;

namespace Webisell.Domain.Entities
{
    public class Filter
    {
        public int FilterId { get; set; }
        public int CategoryId { get; set; }
        public int FilterTypeId { get; set; }
        public string Name { get; set; }

        public Category Category { get; set; }
        public FilterType FilterType { get; set; }
        public List<FilterValue> FilterValues { get; set; }
    }
}
