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
        public DbSet<Farm_management.Models.Feeding> Feeding { get; set; }
        public DbSet<Hatchery> Hatcheries { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تعطيل الحذف المتسلسل للذكر لتجنب التعارض
            modelBuilder.Entity<Hatchery>()
                .HasOne(h => h.MaleAnimal)
                .WithMany()
                .HasForeignKey(h => h.MaleAnimalId)
                .OnDelete(DeleteBehavior.NoAction); // هذا هو السطر السحري

            // تعطيل الحذف المتسلسل للأنثى لتجنب التعارض
            modelBuilder.Entity<Hatchery>()
                .HasOne(h => h.FemaleAnimal)
                .WithMany()
                .HasForeignKey(h => h.FemaleAnimalId)
                .OnDelete(DeleteBehavior.NoAction); // وهذا أيضاً

            // يمكنك إبقاء الحظيرة كما هي أو جعلها NoAction أيضاً للأمان
            modelBuilder.Entity<Hatchery>()
                .HasOne(h => h.ProductionBarn)
                .WithMany()
                .HasForeignKey(h => h.ProductionBarnId)
                .OnDelete(DeleteBehavior.NoAction);
        }

    }
}
