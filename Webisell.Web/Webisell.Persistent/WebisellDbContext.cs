using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using Webisell.Domain.Entities;

namespace Webisell.Persistent
{
    public class WebisellDbContext : DbContext
    {
        public WebisellDbContext():base()
        {
        }
        public WebisellDbContext(DbContextOptions options):base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {         
            if(!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer("Server=DESKTOP-HSIM5BD;Database=Webisell.Database;User Id=sa;Password=energysuite;");
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Filter> Filters { get; set; }
        public DbSet<FilterValue> FilterValues { get; set; }
    }
}
