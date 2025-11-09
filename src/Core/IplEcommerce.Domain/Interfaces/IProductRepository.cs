using IplEcommerce.Domain.Entities;
using IplEcommerce.Domain.Enums;

namespace IplEcommerce.Domain.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> SearchProductsAsync(string? searchTerm, ProductType? type, int? franchiseId);
    Task<IEnumerable<Product>> GetProductsByFranchiseAsync(int franchiseId);
    Task<IEnumerable<Product>> GetProductsByTypeAsync(ProductType type);
    Task<bool> IsProductAvailableAsync(int productId, int quantity);
    Task<bool> TryDecrementStockAsync(int productId, int quantity);
}