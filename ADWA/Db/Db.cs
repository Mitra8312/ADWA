using ADWA.Models;
using Microsoft.EntityFrameworkCore;

namespace ADWA.Db
{
    public class ContextDb : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=ADWA",
                providerOptions => { providerOptions.EnableRetryOnFailure(); 
                });
        }
    }
}
