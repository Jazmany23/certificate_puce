using certificated_unemi.Models.certificate;
using certificated_unemi.Models.role;
using certificated_unemi.Models.socialauth;
using certificated_unemi.Models.users;
using Microsoft.EntityFrameworkCore;

namespace certificated_unemi.Data.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<SocialAuth> SocialAuths { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.IdentificationNumber)
                .IsRequired()
                .HasMaxLength(20);

            // Relationship: User -> SocialAuth
            modelBuilder.Entity<User>()
                .HasMany(u => u.SocialAuths)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role Configuration
            modelBuilder.Entity<Role>()
                .HasKey(r => r.Id);

            // SocialAuth Configuration
            modelBuilder.Entity<SocialAuth>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<SocialAuth>()
                .Property(s => s.Provider)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<SocialAuth>()
                .Property(s => s.ProviderId)
                .IsRequired()
                .HasMaxLength(200);

            modelBuilder.Entity<Certificate>()
     .HasKey(c => c.Id);

            modelBuilder.Entity<Certificate>()
                .Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Certificate>()
                .Property(c => c.PdfLink)
                .IsRequired();

            modelBuilder.Entity<Certificate>()
                .Property(c => c.SizeInMb)
                .HasPrecision(10, 2); // Para manejar números decimales

            modelBuilder.Entity<Certificate>()
                .HasOne(c => c.User)
                .WithMany(u => u.Certificates)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
