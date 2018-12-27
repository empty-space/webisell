using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Webisell.Web.Commands
{
    public enum ECommandResultType
    {
        Ok,
        Error
    }
    public class CommandResult<T>
    {
        public ECommandResultType Type { get; set; } = ECommandResultType.Ok;
        public string ErrorMessage { get; set; }
        public T Result { get; set; }
    }

}
