using Microsoft.EntityFrameworkCore;
using IplEcommerce.Domain.Entities;
using IplEcommerce.Domain.Enums;
using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Infrastructure.Data;

namespace IplEcommerce.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(IplEcommerceDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string? searchTerm, ProductType? type, int? franchiseId)
    {
        var query = _dbSet.Include(p => p.Franchise).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.Contains(searchTerm) ||
                                   p.Description.Contains(searchTerm) ||
                                   p.Franchise.Name.Contains(searchTerm));
        }

        if (type.HasValue)
        {
            query = query.Where(p => p.Type == type.Value);
        }

        if (franchiseId.HasValue)
        {
            query = query.Where(p => p.FranchiseId == franchiseId.Value);
        }

        return await query.Where(p => p.IsAvailable).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByFranchiseAsync(int franchiseId)
    {
        return await _dbSet.Include(p => p.Franchise)
                          .Where(p => p.FranchiseId == franchiseId && p.IsAvailable)
                          .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByTypeAsync(ProductType type)
    {
        return await _dbSet.Include(p => p.Franchise)
                          .Where(p => p.Type == type && p.IsAvailable)
                          .ToListAsync();
    }

    public async Task<bool> IsProductAvailableAsync(int productId, int quantity)
    {
        var product = await _dbSet.FindAsync(productId);
        return product != null && product.IsAvailable && product.StockQuantity >= quantity;
    }

    public async Task<bool> TryDecrementStockAsync(int productId, int quantity)
    {
        if (quantity <= 0) return false;

        var affected = await _context.Database.ExecuteSqlInterpolatedAsync($@"UPDATE Products
                                                                               SET StockQuantity = StockQuantity - {quantity}
                                                                               WHERE Id = {productId} AND StockQuantity >= {quantity}");

        return affected > 0;
    }

    public override async Task<Product?> GetByIdAsync(int id)
    {
        return await _dbSet.Include(p => p.Franchise)
                          .FirstOrDefaultAsync(p => p.Id == id);
    }

    public override async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _dbSet.Include(p => p.Franchise)
                          .Where(p => p.IsAvailable)
                          .ToListAsync();
    }
}