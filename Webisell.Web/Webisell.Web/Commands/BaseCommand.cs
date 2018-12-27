using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webisell.Web.Commands
{
    public class BaseCommand
    {
        protected CommandResult<T> WrapResult<T>(T result)
        {
            return new CommandResult<T> { Result = result };
        }
    }
}
