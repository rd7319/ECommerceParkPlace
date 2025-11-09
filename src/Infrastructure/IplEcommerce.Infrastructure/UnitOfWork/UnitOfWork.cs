using IplEcommerce.Domain.Interfaces;
using IplEcommerce.Infrastructure.Data;
using IplEcommerce.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace IplEcommerce.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly IplEcommerceDbContext _context;
    private readonly Dictionary<Type, object> _repositories;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(IplEcommerceDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>();
        
        Products = new ProductRepository(_context);
        Carts = new CartRepository(_context);
        Orders = new OrderRepository(_context);
    }

    public IProductRepository Products { get; private set; }
    public ICartRepository Carts { get; private set; }
    public IOrderRepository Orders { get; private set; }

    public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        if (_repositories.ContainsKey(typeof(TEntity)))
        {
            return (IGenericRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        var repository = new GenericRepository<TEntity>(_context);
        _repositories.Add(typeof(TEntity), repository);
        return repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    // Begin a new database transaction
    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_currentTransaction != null)
            return _currentTransaction; // Return existing transaction if already open

        _currentTransaction = await _context.Database.BeginTransactionAsync();
        return _currentTransaction;
    }

    //  Commit current transaction
    public async Task CommitTransactionAsync()
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        await _currentTransaction.CommitAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }

    //  Rollback current transaction
    public async Task RollbackTransactionAsync()
    {
        if (_currentTransaction == null)
            return;

        await _currentTransaction.RollbackAsync();
        await _currentTransaction.DisposeAsync();
        _currentTransaction = null;
    }
    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}