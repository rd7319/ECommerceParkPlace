using Microsoft.AspNetCore.Mvc;
using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Domain.Enums;
using IplEcommerce.Application.DTOs;
using IplEcommerce.Application.Common;

namespace IplEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductsController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<IEnumerable<ProductDto>>>> GetProducts()
    {
        try
        {
            var products = await _unitOfWork.Products.GetAllAsync();
            var productDtos = products.Select(MapToDto);
            return Ok(BaseResponse<IEnumerable<ProductDto>>.Success(productDtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<IEnumerable<ProductDto>>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaseResponse<ProductDto>>> GetProduct(int id)
    {
        try
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(BaseResponse<ProductDto>.Failure("Product not found"));
            }

            var productDto = MapToDto(product);
            return Ok(BaseResponse<ProductDto>.Success(productDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<ProductDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<BaseResponse<IEnumerable<ProductDto>>>> SearchProducts(
        [FromQuery] string? searchTerm,
        [FromQuery] ProductType? type,
        [FromQuery] int? franchiseId)
    {
        try
        {
            var products = await _unitOfWork.Products.SearchProductsAsync(searchTerm, type, franchiseId);
            var productDtos = products.Select(MapToDto);
            return Ok(BaseResponse<IEnumerable<ProductDto>>.Success(productDtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<IEnumerable<ProductDto>>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpGet("franchise/{franchiseId}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<ProductDto>>>> GetProductsByFranchise(int franchiseId)
    {
        try
        {
            var products = await _unitOfWork.Products.GetProductsByFranchiseAsync(franchiseId);
            var productDtos = products.Select(MapToDto);
            return Ok(BaseResponse<IEnumerable<ProductDto>>.Success(productDtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<IEnumerable<ProductDto>>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpGet("type/{type}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<ProductDto>>>> GetProductsByType(ProductType type)
    {
        try
        {
            var products = await _unitOfWork.Products.GetProductsByTypeAsync(type);
            var productDtos = products.Select(MapToDto);
            return Ok(BaseResponse<IEnumerable<ProductDto>>.Success(productDtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<IEnumerable<ProductDto>>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Type = product.Type,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity,
            IsAvailable = product.IsAvailable,
            Size = product.Size,
            Color = product.Color,
            FranchiseId = product.FranchiseId,
            Franchise = new FranchiseDto
            {
                Id = product.Franchise.Id,
                Name = product.Franchise.Name,
                ShortName = product.Franchise.ShortName,
                City = product.Franchise.City,
                Color = product.Franchise.Color,
                LogoUrl = product.Franchise.LogoUrl,
                Description = product.Franchise.Description
            }
        };
    }
}