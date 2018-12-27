using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webisell.Web.Configuration
{
    public class DefaultSettingsProvider: ISettingsProvider
    {
        public string MongoConnectionString { get; set; } = "mongodb+srv://devr:jQvwl0pjEE85sli4@clusterdev0-cvkvj.mongodb.net/test?retryWrites=true";

        public int CountOfSearchResults { get; set; } = 5;
    }
}
