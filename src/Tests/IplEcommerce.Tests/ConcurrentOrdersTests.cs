using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using IplEcommerce.Infrastructure.Data;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Infrastructure.UnitOfWork;
using IplEcommerce.API.Controllers;

namespace IplEcommerce.Tests
{
    public class ConcurrentOrdersTests : IDisposable
    {
        private readonly string _connectionString;
        private readonly SqliteConnection _keepAliveConnection;

        public ConcurrentOrdersTests()
        {
            // use a shared in-memory SQLite database
            _connectionString = new SqliteConnectionStringBuilder { DataSource = "file:concurrentorders?mode=memory&cache=shared" }.ToString();
            _keepAliveConnection = new SqliteConnection(_connectionString);
            _keepAliveConnection.Open();

            var options = new DbContextOptionsBuilder<IplEcommerceDbContext>()
                .UseSqlite(_keepAliveConnection)
                .Options;

            // create schema
            using var context = new IplEcommerceDbContext(options);
            context.Database.EnsureCreated();
        }

        [Fact]
        public async Task CreateOrder_ConcurrentRequests_ShouldNotOversell()
        {
            // Seed data: one product with limited stock and two users with carts containing that product
            int productId, user1Id, user2Id;

            var seedOptions = new DbContextOptionsBuilder<IplEcommerceDbContext>()
                .UseSqlite(_connectionString)
                .Options;

            using (var context = new IplEcommerceDbContext(seedOptions))
            {
                var franchise = new Franchise { Name = "F", ShortName = "F", City = "City", Color = "Blue", LogoUrl = string.Empty, Description = string.Empty };
                context.Franchises.Add(franchise);
                await context.SaveChangesAsync();

                var product = new Product { Name = "P", Price = 100m, StockQuantity = 5, IsAvailable = true, FranchiseId = franchise.Id };
                context.Products.Add(product);
                await context.SaveChangesAsync();
                productId = product.Id;

                var user1 = new User { Email = "u1@example.com", FirstName = "U1", LastName = "L1", IsActive = true, Address = string.Empty, City = string.Empty, PostalCode = string.Empty, Phone = string.Empty };
                var user2 = new User { Email = "u2@example.com", FirstName = "U2", LastName = "L2", IsActive = true, Address = string.Empty, City = string.Empty, PostalCode = string.Empty, Phone = string.Empty };
                context.Users.AddRange(user1, user2);
                await context.SaveChangesAsync();
                user1Id = user1.Id;
                user2Id = user2.Id;

                var cart1 = new Cart { UserId = user1Id };
                var cart2 = new Cart { UserId = user2Id };
                context.Carts.AddRange(cart1, cart2);
                await context.SaveChangesAsync();

                var ci1 = new CartItem { CartId = cart1.Id, ProductId = productId, Quantity = 3 };
                var ci2 = new CartItem { CartId = cart2.Id, ProductId = productId, Quantity = 3 };
                context.CartItems.AddRange(ci1, ci2);
                await context.SaveChangesAsync();
            }

            // Run two concurrent create-order requests
            var task1 = Task.Run(async () =>
            {
                var options1 = new DbContextOptionsBuilder<IplEcommerceDbContext>()
                    .UseSqlite(_connectionString)
                    .Options;

                await using var ctx1 = new IplEcommerceDbContext(options1);
                var uow1 = new UnitOfWork(ctx1);
                var controller1 = new OrdersController(uow1);

                return await controller1.CreateOrder(new CreateOrderRequest { UserId = user1Id, ShippingAddress = "Addr1" });
            });

            var task2 = Task.Run(async () =>
            {
                var options2 = new DbContextOptionsBuilder<IplEcommerceDbContext>()
                    .UseSqlite(_connectionString)
                    .Options;

                await using var ctx2 = new IplEcommerceDbContext(options2);
                var uow2 = new UnitOfWork(ctx2);
                var controller2 = new OrdersController(uow2);

                return await controller2.CreateOrder(new CreateOrderRequest { UserId = user2Id, ShippingAddress = "Addr2" });
            });

            await Task.WhenAll(task1, task2);

            var result1 = task1.Result;
            var result2 = task2.Result;

            // Exactly one of the results should be successful (Ok)
            var ok1 = result1.Result is OkObjectResult;
            var ok2 = result2.Result is OkObjectResult;

            (ok1 ^ ok2).Should().BeTrue("only one order should succeed because combined requested quantity exceeds stock");

            // Verify final stock and orders count
            var verifyOptions = new DbContextOptionsBuilder<IplEcommerceDbContext>()
                .UseSqlite(_connectionString)
                .Options;

            using var verifyCtx = new IplEcommerceDbContext(verifyOptions);
            var finalProduct = await verifyCtx.Products.FindAsync(productId);
            finalProduct.Should().NotBeNull();
            finalProduct!.StockQuantity.Should().Be(2);

            var orders = await verifyCtx.Orders.ToListAsync();
            orders.Count.Should().Be(1);
        }

        public void Dispose()
        {
            _keepAliveConnection?.Dispose();
        }
    }
}
