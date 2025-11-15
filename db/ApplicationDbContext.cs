using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.db;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.ProductId, r.UserId })
            .IsUnique();

        modelBuilder.Entity<ShoppingCartItem>()
            .HasOne(sci => sci.User)
            .WithMany(u => u.ShoppingCartItems)
            .HasForeignKey(sci => sci.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShoppingCartItem>()
            .HasOne(sci => sci.Product)
            .WithMany(p => p.ShoppingCartItems)
            .HasForeignKey(sci => sci.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WishlistItem>()
            .HasOne(wi => wi.User)
            .WithMany(u => u.WishlistItems)
            .HasForeignKey(wi => wi.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WishlistItem>()
            .HasOne(wi => wi.Product)
            .WithMany(p => p.WishlistItems)
            .HasForeignKey(wi => wi.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Ноутбуки", Description = "Портативні комп'ютери" },
            new Category { Id = 2, Name = "Комп'ютери", Description = "Настільні комп'ютери та системні блоки" },
            new Category { Id = 3, Name = "Монітори", Description = "Дисплеї та монітори" },
            new Category { Id = 4, Name = "Комплектуючі", Description = "Комплектуючі для ПК" },
            new Category { Id = 5, Name = "Периферія", Description = "Клавіатури, миші, навушники" },
            new Category { Id = 6, Name = "Мережеве обладнання", Description = "Роутери, комутатори" }
        );
        
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "ASUS ROG Strix G15",
                Description = "Ігровий ноутбук з процесором AMD Ryzen 7 та відеокартою NVIDIA RTX 3060",
                Price = 45000,
                CategoryId = 1,
                Brand = "ASUS",
                StockQuantity = 15,
                ImageUrl = "/images/products/laptop1.jpg",
                IsAvailable = true,
                Specifications =
                    "{\"cpu\":\"AMD Ryzen 7 5800H\",\"ram\":\"16GB DDR4\",\"storage\":\"512GB SSD\",\"gpu\":\"NVIDIA RTX 3060\",\"display\":\"15.6 FHD 144Hz\"}"
            },
            new Product
            {
                Id = 2,
                Name = "Lenovo IdeaPad 3",
                Description = "Офісний ноутбук для роботи та навчання",
                Price = 18000,
                CategoryId = 1,
                Brand = "Lenovo",
                StockQuantity = 25,
                ImageUrl = "/images/products/laptop1.jpg",
                IsAvailable = true,
                Specifications =
                    "{\"cpu\":\"Intel Core i5-1135G7\",\"ram\":\"8GB DDR4\",\"storage\":\"256GB SSD\",\"display\":\"15.6 FHD\"}"
            },
            new Product
            {
                Id = 3,
                Name = "Logitech G502 HERO",
                Description = "Ігрова миша з RGB підсвічуванням",
                Price = 1500,
                CategoryId = 5,
                Brand = "Logitech",
                StockQuantity = 50,
                ImageUrl = "/images/products/laptop1.jpg",
                IsAvailable = true,
                Specifications = "{\"sensor\":\"HERO 25K\",\"dpi\":\"25600\",\"buttons\":\"11\",\"weight\":\"121g\"}"
            }
        );
    }
}