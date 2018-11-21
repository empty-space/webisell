using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Webisell.Domain.Entities
{
    public class Filter
    {
        public int FilterId { get; set; }
        public int CategoryId { get; set; }
        public int FilterTypeId { get; set; }
        public bool IsSystem { get; set; }
        public string Name { get; set; }

        private string _jsonFieldName;

        [NotMapped]
        public string JsonFieldName
        {
            get {
                if (_jsonFieldName == null)
                    _jsonFieldName = Name?.ToLower();
                return _jsonFieldName; }
            set { _jsonFieldName = value; }
        }

        public Category Category { get; set; }
        public FilterType FilterType { get; set; }
        public List<FilterValue> FilterValues { get; set; }
    }
}
