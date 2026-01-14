using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Farm_management.Models;

namespace Farm_management.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Farm_management.Models.Barns> Barns { get; set; } = default!;
        public DbSet<Farm_management.Models.Animals> Animals { get; set; } = default!;
    }
}
