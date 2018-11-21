using Microsoft.EntityFrameworkCore;
using Webisell.Domain.Entities;
using Webisell.Persistent;

namespace Webisell.Research.EF
{
    public class WebisellSqlServerDbContext : WebisellDbContext 
    {
        public WebisellSqlServerDbContext():base()
        {

        }
        public WebisellSqlServerDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
