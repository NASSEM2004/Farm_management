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

        // إضافة جدول العيادة
        public DbSet<Clinic> Clinics { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. تعطيل الحذف المتسلسل لقسم التفريخ (Hatchery)
            modelBuilder.Entity<Hatchery>()
                .HasOne(h => h.MaleAnimal)
                .WithMany()
                .HasForeignKey(h => h.MaleAnimalId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Hatchery>()
                .HasOne(h => h.FemaleAnimal)
                .WithMany()
                .HasForeignKey(h => h.FemaleAnimalId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Hatchery>()
                .HasOne(h => h.ProductionBarn)
                .WithMany()
                .HasForeignKey(h => h.ProductionBarnId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. تعطيل الحذف المتسلسل لقسم العيادة (Clinic)
            modelBuilder.Entity<Clinic>()
                .HasOne(c => c.Animal)
                .WithMany()
                .HasForeignKey(c => c.AnimalId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Clinic>()
                .HasOne(c => c.Barn)
                .WithMany()
                .HasForeignKey(c => c.BarnId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}