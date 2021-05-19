using Microsoft.EntityFrameworkCore;

namespace BitirmeProjesiWeb.Models
{
    public class BitirmeContext : DbContext
    {
        public BitirmeContext(DbContextOptions dbContextOptions) : base(dbContextOptions) { }

        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Partner> Partners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>().HasIndex(u => u.NameAr).IsUnique();
            modelBuilder.Entity<UserRole>().HasData(new UserRole { Id = 1, NameTr = "Müdür" });



            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<User>().HasData(new User { Id = 1, UserRoleId = 1,
                FullName = "Yazan ALKHATIB",
                Email = "yazanalkhatib956@gmail.com",
                Password = "7B0j2NHlveB8q/K6/4abkw=="
            });
        }
    }
}