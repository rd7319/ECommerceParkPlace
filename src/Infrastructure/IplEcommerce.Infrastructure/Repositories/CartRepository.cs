using Microsoft.EntityFrameworkCore;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Infrastructure.Data;

namespace IplEcommerce.Infrastructure.Repositories;

public class CartRepository : GenericRepository<Cart>, ICartRepository
{
    public CartRepository(IplEcommerceDbContext context) : base(context)
    {
    }

    public async Task<Cart?> GetCartByUserIdAsync(int userId)
    {
        return await _dbSet.Include(c => c.CartItems)
                          .ThenInclude(ci => ci.Product)
                          .ThenInclude(p => p.Franchise)
                          .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<CartItem?> GetCartItemAsync(int cartId, int productId)
    {
        return await _context.CartItems
                            .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == productId);
    }

    public async Task AddCartItemAsync(CartItem cartItem)
    {
        await _context.CartItems.AddAsync(cartItem);
    }

    public Task UpdateCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Update(cartItem);
        return Task.CompletedTask;
    }

    public Task RemoveCartItemAsync(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
        return Task.CompletedTask;
    }

    public async Task ClearCartAsync(int cartId)
    {
        var cartItems = await _context.CartItems
                                     .Where(ci => ci.CartId == cartId)
                                     .ToListAsync();
        _context.CartItems.RemoveRange(cartItems);
    }
}