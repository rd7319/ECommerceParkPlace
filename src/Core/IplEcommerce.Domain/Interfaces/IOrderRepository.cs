using IplEcommerce.Domain.Entities;

namespace IplEcommerce.Domain.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
    Task<Order?> GetOrderWithItemsAsync(int orderId);
    Task<string> GenerateOrderNumberAsync();
}