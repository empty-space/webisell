using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webisell.Web.Configuration
{
    public interface ISettingsProvider
    {
        string MongoConnectionString { get; set; }
        int CountOfSearchResults { get; set; }
    }
}
