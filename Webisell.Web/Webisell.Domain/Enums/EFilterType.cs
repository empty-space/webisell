using System;
using System.Collections.Generic;
using System.Text;

namespace Webisell.Domain.Enums
{
    public enum EFilterType:int
    {
        FilterValue_OR = 1,
        MultipleColumns_OR,
        MultipleColumns_AND,
        Integer_OR,
        Integer_Range,
    }
}
