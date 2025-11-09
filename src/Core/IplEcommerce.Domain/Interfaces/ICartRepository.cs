using IplEcommerce.Domain.Entities;

namespace IplEcommerce.Domain.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart?> GetCartByUserIdAsync(int userId);
    Task<CartItem?> GetCartItemAsync(int cartId, int productId);
    Task AddCartItemAsync(CartItem cartItem);
    Task UpdateCartItemAsync(CartItem cartItem);
    Task RemoveCartItemAsync(CartItem cartItem);
    Task ClearCartAsync(int cartId);
}