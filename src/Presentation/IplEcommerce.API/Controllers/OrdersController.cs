using Microsoft.AspNetCore.Mvc;
using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Domain.Enums;
using IplEcommerce.Application.DTOs;
using IplEcommerce.Application.Common;

namespace IplEcommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public OrdersController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<OrderDto>>>> GetUserOrders(int userId)
    {
        try
        {
            var orders = await _unitOfWork.Orders.GetOrdersByUserIdAsync(userId);
            var orderDtos = orders.Select(MapToDto);
            return Ok(BaseResponse<IEnumerable<OrderDto>>.Success(orderDtos));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<IEnumerable<OrderDto>>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaseResponse<OrderDto>>> GetOrder(int id)
    {
        try
        {
            var order = await _unitOfWork.Orders.GetOrderWithItemsAsync(id);
            if (order == null)
            {
                return NotFound(BaseResponse<OrderDto>.Failure("Order not found"));
            }

            var orderDto = MapToDto(order);
            return Ok(BaseResponse<OrderDto>.Success(orderDto));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<OrderDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<BaseResponse<OrderDto>>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            // Get user's cart
            var cart = await _unitOfWork.Carts.GetCartByUserIdAsync(request.UserId);
            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest(BaseResponse<OrderDto>.Failure("Cart is empty"));
            }   

            // Load products used in the cart (single query)
            var ids = cart.CartItems.Select(ci => ci.ProductId).Distinct().ToList();
            var products = (await _unitOfWork.Repository<Product>().FindAsync(p => ids.Contains(p.Id)))
                               .ToDictionary(p => p.Id);

            // Quick validation before attempting DB updates
            foreach (var cartItem in cart.CartItems)
            {
                products.TryGetValue(cartItem.ProductId, out var product);
                var isAvailable = product != null && product.IsAvailable && product.StockQuantity >= cartItem.Quantity;
                if (!isAvailable)
                {
                    return BadRequest(BaseResponse<OrderDto>.Failure(
                        $"Insufficient stock for product: {product?.Name ?? cartItem.ProductId.ToString()}"));
                }
            }

            // Atomically decrement stock in DB for each cart item inside the transaction
            foreach (var cartItem in cart.CartItems)
            {
                var decremented = await _unitOfWork.Products.TryDecrementStockAsync(cartItem.ProductId, cartItem.Quantity);
                if (!decremented)
                {
                    await transaction.RollbackAsync();
                    products.TryGetValue(cartItem.ProductId, out var p);
                    return BadRequest(BaseResponse<OrderDto>.Failure($"Insufficient stock for product: {p?.Name ?? cartItem.ProductId.ToString()}"));
                }
            }

            // Create order and attach order items (save once)
            var orderNumber = await _unitOfWork.Orders.GenerateOrderNumberAsync();
            var order = new Order
            {
                UserId = request.UserId,
                OrderNumber = orderNumber,
                TotalAmount = cart.CartItems.Sum(ci => ci.Quantity * (products[ci.ProductId]?.Price ?? 0m)),
                Status = OrderStatus.Delivered, 
                OrderDate = DateTime.UtcNow,
                ShippingAddress = request.ShippingAddress,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = products[ci.ProductId].Price,
                    TotalPrice = ci.Quantity * products[ci.ProductId].Price
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);

            // Clear cart (will be persisted on SaveChanges)
            await _unitOfWork.Carts.ClearCartAsync(cart.Id);

            // Persist all changes in one call
            await _unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();

            // Return created order
            var createdOrder = await _unitOfWork.Orders.GetOrderWithItemsAsync(order.Id);
            var orderDto = MapToDto(createdOrder!);
            return Ok(BaseResponse<OrderDto>.Success(orderDto, "Order created successfully"));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, BaseResponse<OrderDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<BaseResponse<OrderDto>>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var order = await _unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound(BaseResponse<OrderDto>.Failure("Order not found"));
            }

            order.Status = request.Status;
            if (!string.IsNullOrEmpty(request.TrackingNumber))
            {
                order.TrackingNumber = request.TrackingNumber;
            }

            await _unitOfWork.Orders.UpdateAsync(order);
            await _unitOfWork.SaveChangesAsync();

            var updatedOrder = await _unitOfWork.Orders.GetOrderWithItemsAsync(id);
            var orderDto = MapToDto(updatedOrder!);
            return Ok(BaseResponse<OrderDto>.Success(orderDto, "Order status updated successfully"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, BaseResponse<OrderDto>.Failure($"Internal server error: {ex.Message}"));
        }
    }

    private static OrderDto MapToDto(Order order)
    {
        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderNumber = order.OrderNumber,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            OrderDate = order.OrderDate,
            ShippingAddress = order.ShippingAddress,
            TrackingNumber = order.TrackingNumber,
            OrderItems = order.OrderItems.Select(oi => new OrderItemDto
            {
                Id = oi.Id,
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                TotalPrice = oi.TotalPrice,
                Product = new ProductDto
                {
                    Id = oi.Product.Id,
                    Name = oi.Product.Name,
                    Description = oi.Product.Description,
                    Price = oi.Product.Price,
                    Type = oi.Product.Type,
                    ImageUrl = oi.Product.ImageUrl,
                    StockQuantity = oi.Product.StockQuantity,
                    IsAvailable = oi.Product.IsAvailable,
                    Size = oi.Product.Size,
                    Color = oi.Product.Color,
                    FranchiseId = oi.Product.FranchiseId,
                    Franchise = new FranchiseDto
                    {
                        Id = oi.Product.Franchise.Id,
                        Name = oi.Product.Franchise.Name,
                        ShortName = oi.Product.Franchise.ShortName,
                        City = oi.Product.Franchise.City,
                        Color = oi.Product.Franchise.Color,
                        LogoUrl = oi.Product.Franchise.LogoUrl,
                        Description = oi.Product.Franchise.Description
                    }
                }
            }).ToList()
        };
    }
}

public class CreateOrderRequest
{
    public int UserId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
}

public class UpdateOrderStatusRequest
{
    public OrderStatus Status { get; set; }
    public string? TrackingNumber { get; set; }
}