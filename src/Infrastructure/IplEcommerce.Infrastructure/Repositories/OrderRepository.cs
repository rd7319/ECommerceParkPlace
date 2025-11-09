using Microsoft.EntityFrameworkCore;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Infrastructure.Data;

namespace IplEcommerce.Infrastructure.Repositories;

public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(IplEcommerceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
    {
        return await _dbSet.Include(o => o.OrderItems)
                          .ThenInclude(oi => oi.Product)
                          .ThenInclude(p => p.Franchise)
                          .Where(o => o.UserId == userId)
                          .OrderByDescending(o => o.OrderDate)
                          .ToListAsync();
    }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
    {
        return await _dbSet.Include(o => o.OrderItems)
                          .ThenInclude(oi => oi.Product)
                          .ThenInclude(p => p.Franchise)
                          .Include(o => o.User)
                          .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<string> GenerateOrderNumberAsync()
    {
        var lastOrder = await _dbSet.OrderByDescending(o => o.Id)
                                   .FirstOrDefaultAsync();
        
        var orderNumber = lastOrder?.Id + 1 ?? 1;
        return $"IPL{DateTime.UtcNow:yyyyMMdd}{orderNumber:D6}";
    }
}