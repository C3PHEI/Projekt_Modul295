using Microsoft.EntityFrameworkCore;
using API_Modul295.Models;

namespace API_Modul295.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Order> Orders { get; set; }
        public DbSet<Service> Services { get; set; }
    }
}