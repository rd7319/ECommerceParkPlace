namespace IplEcommerce.Domain.Interfaces;
  using Microsoft.EntityFrameworkCore.Storage;
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    ICartRepository Carts { get; }
    IOrderRepository Orders { get; }
    IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}