using System;
using System.Collections.Generic;
using System.Text;

namespace Webisell.Research
{    
    public class SearchFilterDto
    {
        public int FilterId { get; set; }
        public string FilterName { get; set; }
        public int FilterTypeId { get; set; }

        //FilterValues or integers
        public List<int> Values { get; set; }
        //names of columns of multivalue filter
        public List<string> MultiColumnNames { get; set; }
        //Range integers
        public int RangeMax { get; set; }
        public int RangeMin { get; set; }
    }
}
