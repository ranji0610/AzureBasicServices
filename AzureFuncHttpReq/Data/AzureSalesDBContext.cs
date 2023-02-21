using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFuncHttpReq.Data
{
    public class AzureSalesDBContext : DbContext
    {
        public AzureSalesDBContext(DbContextOptions<AzureSalesDBContext> dbContextOptions):base(dbContextOptions)
        {

        }

        public DbSet<SalesRequest> SalesRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SalesRequest>(entity =>
            {
                entity.HasKey(c => c.Id);
            });
        }
    }
}
