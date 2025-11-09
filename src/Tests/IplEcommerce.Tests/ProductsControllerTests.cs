using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using IplEcommerce.API.Controllers;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Domain.Enums;
using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Application.DTOs;
using IplEcommerce.Application.Common;

namespace IplEcommerce.Tests
{
    public class ProductsControllerTests
    {
        [Fact]
        public async Task GetProducts_Returns_ProductDtos()
        {
            // Arrange
            var franchise = new Franchise { Id = 1, Name = "F1", ShortName = "F1", City = "City", Color = "Blue", LogoUrl = string.Empty, Description = string.Empty };
            var product = new Product
            {
                Id = 1,
                Name = "P1",
                Description = "Desc",
                Price = 10m,
                Type = ProductType.Jersey,
                ImageUrl = string.Empty,
                StockQuantity = 5,
                IsAvailable = true,
                Size = "M",
                Color = "Red",
                FranchiseId = franchise.Id,
                Franchise = franchise
            };

            var mockProductRepo = new Mock<IProductRepository>();
            mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product> { product });

            var mockUow = new Mock<IUnitOfWork>();
            mockUow.Setup(u => u.Products).Returns(mockProductRepo.Object);

            var controller = new ProductsController(mockUow.Object);

            // Act
            var actionResult = await controller.GetProducts();

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            var ok = (OkObjectResult)actionResult.Result;
            ok.Value.Should().BeOfType<BaseResponse<IEnumerable<ProductDto>>>();

            var response = (BaseResponse<IEnumerable<ProductDto>>)ok.Value!;
            response.IsSuccess.Should().BeTrue();
            response.Data.Should().NotBeNull();
            response.Data!.Count().Should().Be(1);
            response.Data.First().Id.Should().Be(product.Id);
            response.Data.First().Name.Should().Be(product.Name);
        }
    }
}
