using Microsoft.EntityFrameworkCore;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Domain.Enums;
using IplEcommerce.Infrastructure.Data;

namespace IplEcommerce.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IplEcommerceDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Check if data already exists

        // Seed Franchises
        var franchises = new List<Franchise>
        {
            new Franchise
            {
                Name = "Chennai Super Kings",
                ShortName = "CSK",
                City = "Chennai",
                Color = "#FFFF00",
                LogoUrl = "https://example.com/csk-logo.png",
                Description = "The Chennai Super Kings are one of the most successful teams in IPL history.",
                IsActive = true
            },
            new Franchise
            {
                Name = "Mumbai Indians",
                ShortName = "MI",
                City = "Mumbai",
                Color = "#004BA0",
                LogoUrl = "https://example.com/mi-logo.png",
                Description = "Mumbai Indians are the most successful franchise in the IPL.",
                IsActive = true
            },
            new Franchise
            {
                Name = "Royal Challengers Bangalore",
                ShortName = "RCB",
                City = "Bangalore",
                Color = "#EC1C24",
                LogoUrl = "https://example.com/rcb-logo.png",
                Description = "Royal Challengers Bangalore known for their passionate fanbase.",
                IsActive = true
            },
            new Franchise
            {
                Name = "Kolkata Knight Riders",
                ShortName = "KKR",
                City = "Kolkata",
                Color = "#3A225D",
                LogoUrl = "https://example.com/kkr-logo.png",
                Description = "Kolkata Knight Riders owned by Bollywood superstar Shah Rukh Khan.",
                IsActive = true
            },
            new Franchise
            {
                Name = "Delhi Capitals",
                ShortName = "DC",
                City = "Delhi",
                Color = "#282968",
                LogoUrl = "https://example.com/dc-logo.png",
                Description = "Delhi Capitals, formerly Delhi Daredevils, represent the capital city.",
                IsActive = true
            },
            new Franchise
            {
                Name = "Rajasthan Royals",
                ShortName = "RR",
                City = "Jaipur",
                Color = "#254AA5",
                LogoUrl = "https://example.com/rr-logo.png",
                Description = "Rajasthan Royals were the inaugural IPL champions in 2008.",
                IsActive = true
            },
            new Franchise
            {
                Name = "Sunrisers Hyderabad",
                ShortName = "SRH",
                City = "Hyderabad",
                Color = "#FF822A",
                LogoUrl = "https://example.com/srh-logo.png",
                Description = "Sunrisers Hyderabad known for their strong bowling attack.",
                IsActive = true
            },
            new Franchise
            {
                Name = "Punjab Kings",
                ShortName = "PBKS",
                City = "Mohali",
                Color = "#ED1B24",
                LogoUrl = "https://example.com/pbks-logo.png",
                Description = "Punjab Kings, formerly Kings XI Punjab, represent Punjab state.",
                IsActive = true
            }
        };

        if (!await context.Franchises.AnyAsync())
        {
            context.Franchises.AddRange(franchises);
            await context.SaveChangesAsync();
        }
        //await context.SaveChangesAsync();

        // Seed Products
        var products = new List<Product>();
        var productTypes = Enum.GetValues<ProductType>();

        foreach (var franchise in franchises)
        {
            // Create products for each franchise
            products.AddRange(new List<Product>
            {
                new Product
                {
                    Name = $"{franchise.Name} Official Jersey 2024",
                    Description = $"Official team jersey for {franchise.Name}. Made with premium quality fabric with team colors and logos.",
                    Price = 2499.99M,
                    Type = ProductType.Jersey,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-jersey.jpg",
                    StockQuantity = 50,
                    IsAvailable = true,
                    Size = "Large",
                    Color = franchise.Color,
                    FranchiseId = franchise.Id
                },
                new Product
                {
                    Name = $"{franchise.Name} Player Jersey - Captain Edition",
                    Description = $"Premium captain edition jersey with official player name and number for {franchise.Name}.",
                    Price = 3499.99M,
                    Type = ProductType.Jersey,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-captain-jersey.jpg",
                    StockQuantity = 25,
                    IsAvailable = true,
                    Size = "Medium",
                    Color = franchise.Color,
                    FranchiseId = franchise.Id
                },
                new Product
                {
                    Name = $"{franchise.Name} Official Cap",
                    Description = $"Official team cap with embroidered logo for {franchise.Name}. Adjustable size, one size fits all.",
                    Price = 799.99M,
                    Type = ProductType.Cap,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-cap.jpg",
                    StockQuantity = 100,
                    IsAvailable = true,
                    Size = "OS",
                    Color = franchise.Color,
                    FranchiseId = franchise.Id
                },
                new Product
                {
                    Name = $"{franchise.Name} Team Flag",
                    Description = $"Large team flag perfect for displaying your support for {franchise.Name}. High quality polyester material.",
                    Price = 599.99M,
                    Type = ProductType.Flag,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-flag.jpg",
                    StockQuantity = 75,
                    IsAvailable = true,
                    Size = "3x5ft",
                    Color = franchise.Color,
                    FranchiseId = franchise.Id
                },
                new Product
                {
                    Name = $"{franchise.Name} Captain Autographed Photo",
                    Description = $"Limited edition autographed photo of {franchise.Name} team captain. Certificate of authenticity included.",
                    Price = 4999.99M,
                    Type = ProductType.AutographedPhoto,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-autograph.jpg",
                    StockQuantity = 10,
                    IsAvailable = true,
                    Size = "8x10",
                    Color = "Multi-color",
                    FranchiseId = franchise.Id
                },
                new Product
                {
                    Name = $"{franchise.Name} Victory Poster",
                    Description = $"Commemorative poster celebrating {franchise.Name}'s greatest victories. Perfect for wall decoration.",
                    Price = 299.99M,
                    Type = ProductType.Poster,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-poster.jpg",
                    StockQuantity = 200,
                    IsAvailable = true,
                    Size = "A2",
                    Color = "Full Color",
                    FranchiseId = franchise.Id
                },
                new Product
                {
                    Name = $"{franchise.Name} Team Keychain",
                    Description = $"Premium metal keychain with {franchise.Name} logo. Perfect accessory for fans.",
                    Price = 199.99M,
                    Type = ProductType.Keychain,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-keychain.jpg",
                    StockQuantity = 500,
                    IsAvailable = true,
                    Size = "Small",
                    Color = franchise.Color,
                    FranchiseId = franchise.Id
                },
                new Product
                {
                    Name = $"{franchise.Name} Coffee Mug",
                    Description = $"Ceramic coffee mug with {franchise.Name} design. Dishwasher and microwave safe.",
                    Price = 399.99M,
                    Type = ProductType.Mug,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-mug.jpg",
                    StockQuantity = 150,
                    IsAvailable = true,
                    Size = "350ml",
                    Color = franchise.Color,
                    FranchiseId = franchise.Id
                },
                new Product
                {
                    Name = $"{franchise.Name} Fan T-Shirt",
                    Description = $"Comfortable cotton t-shirt for {franchise.Name} fans. Available in multiple sizes.",
                    Price = 899.99M,
                    Type = ProductType.TShirt,
                    ImageUrl = $"https://example.com/{franchise.ShortName.ToLower()}-tshirt.jpg",
                    StockQuantity = 80,
                    IsAvailable = true,
                    Size = "Large",
                    Color = franchise.Color,
                    FranchiseId = franchise.Id
                }
            });
        }

        if (!await context.Products.AnyAsync())
        {
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Seed a test user
        var testUsers = new List<User> {
        new User{
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            Phone = "+91-9876543210",
            Address = "123 Test Street, Test Area",
            City = "Mumbai",
            PostalCode = "400001",
            IsActive = true
        },
        new User
        {
            Email = "test1@example.com",
            FirstName = "Test1",
            LastName = "User1",
            Phone = "+91-9876543220",
            Address = "124 Test Street, Test Area",
            City = "Mumbai",
            PostalCode = "400001",
            IsActive = true
        },
        new User
        {
            Email = "test2@example.com",
            FirstName = "Test2",
            LastName = "User2",
            Phone = "+91-9876543230",
            Address = "125 Test Street, Test Area",
            City = "Mumbai",
            PostalCode = "400001",
            IsActive = true
        }
        };
        if (!await context.Users.AnyAsync())
        {
            context.Users.AddRange(testUsers);
            await context.SaveChangesAsync();
    }
}
}