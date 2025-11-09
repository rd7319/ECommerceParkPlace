using Microsoft.AspNetCore.Mvc;
using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Application.DTOs;
using IplEcommerce.Application.Common;

namespace IplEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public CartController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<BaseResponse<CartDto>>> GetUserCart(int userId)
    {
        try
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                // Create a new cart for the user
                cart = new Cart { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
                cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            }

            var cartDto = MapToDto(cart!);
            return Ok(BaseResponse<CartDto>.Success(cartDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<CartDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult<BaseResponse<CartDto>>> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            // Check if product exists and has sufficient stock
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product == null || !product.IsAvailable)
            {
                return BadRequest(BaseResponse<CartDto>.Failure("Product not found or not available"));
            }

            if (product.StockQuantity < request.Quantity)
            {
                return BadRequest(BaseResponse<CartDto>.Failure("Insufficient stock"));
            }

            // Get or create cart
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(request.UserId);
            if (cart == null)
            {
                cart = new Cart { UserId = request.UserId };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveChangesAsync();
                cart = await _unitOfWork.Carts.GetCartByUserIdAsync(request.UserId);
            }

            // Check if item already exists in cart
            var existingItem = await _unitOfWork.Carts.GetCartItemAsync(cart!.Id, request.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += request.Quantity;
                await _unitOfWork.Carts.UpdateCartItemAsync(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await _unitOfWork.Carts.AddCartItemAsync(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();

            // Return updated cart
            cart = await _unitOfWork.Carts.GetCartByUserIdAsync(request.UserId);
            var cartDto = MapToDto(cart!);
            return Ok(BaseResponse<CartDto>.Success(cartDto, "Product added to cart successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<CartDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpPut("update")]
    public async Task<ActionResult<BaseResponse<CartDto>>> UpdateCartItem([FromBody] UpdateCartItemRequest request)
    {
        try
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(request.UserId);
            if (cart == null)
            {
                return NotFound(BaseResponse<CartDto>.Failure("Cart not found"));
            }

            var cartItem = await _unitOfWork.Carts.GetCartItemAsync(cart.Id, request.ProductId);
            if (cartItem == null)
            {
                return NotFound(BaseResponse<CartDto>.Failure("Cart item not found"));
            }

            // Check stock availability
            var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId);
            if (product != null && product.StockQuantity < request.Quantity)
            {
                return BadRequest(BaseResponse<CartDto>.Failure("Insufficient stock"));
            }

            cartItem.Quantity = request.Quantity;
            await _unitOfWork.Carts.UpdateCartItemAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            // Return updated cart
            cart = await _unitOfWork.Carts.GetCartByUserIdAsync(request.UserId);
            var cartDto = MapToDto(cart!);
            return Ok(BaseResponse<CartDto>.Success(cartDto, "Cart item updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<CartDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpDelete("remove/{userId}/{productId}")]
    public async Task<ActionResult<BaseResponse<CartDto>>> RemoveFromCart(int userId, int productId)
    {
        try
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound(BaseResponse<CartDto>.Failure("Cart not found"));
            }

            var cartItem = await _unitOfWork.Carts.GetCartItemAsync(cart.Id, productId);
            if (cartItem == null)
            {
                return NotFound(BaseResponse<CartDto>.Failure("Cart item not found"));
            }

            await _unitOfWork.Carts.RemoveCartItemAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();

            // Return updated cart
            cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            var cartDto = MapToDto(cart!);
            return Ok(BaseResponse<CartDto>.Success(cartDto, "Item removed from cart successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<CartDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpDelete("clear/{userId}")]
    public async Task<ActionResult<BaseResponse<CartDto>>> ClearCart(int userId)
    {
        try
        {
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return NotFound(BaseResponse<CartDto>.Failure("Cart not found"));
            }

            await _unitOfWork.Carts.ClearCartAsync(cart.Id);
            await _unitOfWork.SaveChangesAsync();

            var cartDto = MapToDto(cart);
            cartDto.CartItems.Clear();
            return Ok(BaseResponse<CartDto>.Success(cartDto, "Cart cleared successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<CartDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    private static CartDto MapToDto(Cart cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            CartItems = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                Product = new ProductDto
                {
                    Id = ci.Product.Id,
                    Name = ci.Product.Name,
                    Description = ci.Product.Description,
                    Price = ci.Product.Price,
                    Type = ci.Product.Type,
                    ImageUrl = ci.Product.ImageUrl,
                    StockQuantity = ci.Product.StockQuantity,
                    IsAvailable = ci.Product.IsAvailable,
                    Size = ci.Product.Size,
                    Color = ci.Product.Color,
                    FranchiseId = ci.Product.FranchiseId,
                    Franchise = new FranchiseDto
                    {
                        Id = ci.Product.Franchise.Id,
                        Name = ci.Product.Franchise.Name,
                        ShortName = ci.Product.Franchise.ShortName,
                        City = ci.Product.Franchise.City,
                        Color = ci.Product.Franchise.Color,
                        LogoUrl = ci.Product.Franchise.LogoUrl,
                        Description = ci.Product.Franchise.Description
                    }
                }
            }).ToList(),
            TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price)
        };
    }
}

public class AddToCartRequest
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartItemRequest
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}