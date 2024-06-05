using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChocoLuxAPI.Models
{
    public partial class AppDbContext : IdentityDbContext<UserModel, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        //public DbSet<UserModel> tblUsers { get; set; }
        public DbSet<Product> tblProducts { get; set; }
        public DbSet<Category> tblCategories { get; set; }
        public DbSet<Orders> tblOrders { get; set; }
        public DbSet<OrderDetails> tblOrderDetails { get; set; }
        public DbSet<Cart> tblCart { get; set; }
        public DbSet<CartItem> tblCartItems { get; set; }
        public DbSet<Session> tblSession { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Cart>()
            //.HasMany(c => c.Items)
            //.WithOne(ci => ci.Cart)
            //.HasForeignKey(ci => ci.CartId);

            // Configure primary keys
            modelBuilder.Entity<Session>().HasKey(s => s.SessionId);
            modelBuilder.Entity<Cart>().HasKey(c => c.CartId);
            modelBuilder.Entity<CartItem>().HasKey(ci => ci.CartItemId);

            // Configure foreign key relationships
            modelBuilder.Entity<Session>()
                .HasMany(s => s.Carts)
                .WithOne(c => c.Session)
                .HasForeignKey(c => c.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItem)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<IdentityUserRole<string>>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
            });
            modelBuilder.Entity<IdentityUserToken<string>>(userToken =>
            {
                userToken.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });
            });

            // Seed categories
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3301"), CategoryName = "Dark Chocolate" },
                new Category { CategoryId = Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3302"), CategoryName = "Milk Chocolate" },
                new Category { CategoryId = Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3303"), CategoryName = "White Chocolate" },
                new Category { CategoryId = Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3304"), CategoryName = "Candy" }
            );

        }
    }
}
