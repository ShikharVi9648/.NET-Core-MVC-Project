using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
//2
namespace WebApplication1.data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        
    }
}
