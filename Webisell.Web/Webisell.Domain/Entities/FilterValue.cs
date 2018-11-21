using System;
using System.Collections.Generic;
using System.Text;

namespace Webisell.Domain.Entities
{
    public class FilterValue
    {
        public int FilterValueId { get; set; }
        public int FilterId { get; set; }
        public string Value { get; set; }
    }
}
