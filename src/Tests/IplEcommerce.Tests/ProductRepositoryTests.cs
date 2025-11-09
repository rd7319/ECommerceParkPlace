using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using IplEcommerce.Infrastructure.Data;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Infrastructure.Repositories;

namespace IplEcommerce.Tests
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<IplEcommerceDbContext> _options;

        public ProductRepositoryTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            _options = new DbContextOptionsBuilder<IplEcommerceDbContext>()
                .UseSqlite(_connection)
                .Options;

            // create schema
            using var context = new IplEcommerceDbContext(_options);
            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task TryDecrementStockAsync_Decrements_WhenSufficientStock()
        {
            using var context = new IplEcommerceDbContext(_options);

            var franchise = new Franchise { Name = "F1", ShortName = "F1", City = "City", Color = "Blue", LogoUrl = string.Empty, Description = string.Empty };
            context.Franchises.Add(franchise);
            await context.SaveChangesAsync();

            var product = new Product { Name = "P1", Price = 10m, StockQuantity = 5, IsAvailable = true, FranchiseId = franchise.Id };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);

            var success = await repo.TryDecrementStockAsync(product.Id, 3);
            success.Should().BeTrue();

            using var verify = new IplEcommerceDbContext(_options);
            var reloaded = await verify.Products.FindAsync(product.Id);
            reloaded.Should().NotBeNull();
            reloaded!.StockQuantity.Should().Be(2);
        }

        [Fact]
        public async Task TryDecrementStockAsync_Fails_WhenInsufficientStock()
        {
            using var context = new IplEcommerceDbContext(_options);

            var franchise = new Franchise { Name = "F2", ShortName = "F2", City = "City", Color = "Green", LogoUrl = string.Empty, Description = string.Empty };
            context.Franchises.Add(franchise);
            await context.SaveChangesAsync();

            var product = new Product { Name = "P2", Price = 20m, StockQuantity = 2, IsAvailable = true, FranchiseId = franchise.Id };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repo = new ProductRepository(context);

            var success = await repo.TryDecrementStockAsync(product.Id, 5);
            success.Should().BeFalse();

            //using var verify = new IplEcommerceDbContext(_options);
            context.ChangeTracker.Clear();
            var reloaded = await context.Products.FindAsync(product.Id);
            reloaded.Should().NotBeNull();
            reloaded!.StockQuantity.Should().Be(2);
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
