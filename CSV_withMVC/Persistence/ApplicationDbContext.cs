using CSV_withMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace CSV_withMVC.Persistence
{
    public class ApplicationDbContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        
    }
}
