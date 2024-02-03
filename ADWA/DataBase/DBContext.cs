using ADWA.Models;
using Microsoft.EntityFrameworkCore;

namespace ADWA.DataBase
{
    public class DBContext : DbContext
    {
        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasKey(e => e.SamAccountName);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            // Укажите строку подключения к базе данных SQLite
            optionsBuilder.UseSqlite("Data Source=RemoteAccess.db");
        }
    }
}
