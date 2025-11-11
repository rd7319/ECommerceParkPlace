classDiagram
interface IUnitOfWork ["IUnitOfWork\n+Products: IProductRepository\n+Carts: ICartRepository\n+Orders: IOrderRepository\n+Repository<TEntity>()\n+SaveChangesAsync()\n+BeginTransactionAsync()\n+CommitTransactionAsync()\n+RollbackTransactionAsync()"]

interface IGenericRepository~T~ ["IGenericRepository<T>\n+GetByIdAsync(id)\n+GetAllAsync()\n+FindAsync(expr)\n+AddAsync(entity)\n+UpdateAsync(entity)\n+DeleteAsync(entity)\n+ExistsAsync(id)"]

interface IProductRepository ["IProductRepository : IGenericRepository<Product>\n+SearchProductsAsync(...)\n+GetProductsByFranchiseAsync(...)\n+GetProductsByTypeAsync(...)\n+IsProductAvailableAsync(...)\n+TryDecrementStockAsync(...)"]

interface ICartRepository ["ICartRepository : IGenericRepository<Cart>\n+GetCartByUserIdAsync(...)\n+GetCartItemAsync(...)\n+AddCartItemAsync(...)\n+UpdateCartItemAsync(...)\n+RemoveCartItemAsync(...)\n+ClearCartAsync(...)"]

interface IOrderRepository ["IOrderRepository : IGenericRepository<Order>\n+GetOrdersByUserIdAsync(...)\n+GetOrderWithItemsAsync(...)\n+GenerateOrderNumberAsync()"]

class UnitOfWork ["UnitOfWork\n- _context: IplEcommerceDbContext\n- _repositories: Dictionary<Type, object>\n- _currentTransaction: IDbContextTransaction?\n+Products: IProductRepository\n+Carts: ICartRepository\n+Orders: IOrderRepository\n+Repository<TEntity>()\n+SaveChangesAsync()\n+BeginTransactionAsync()\n+CommitTransactionAsync()\n+RollbackTransactionAsync()\n+Dispose()"]

class GenericRepository~T~ ["GenericRepository<T>\n- _context: IplEcommerceDbContext\n- _dbSet: DbSet<T>\n+GetByIdAsync(id)\n+GetAllAsync()\n+FindAsync(expr)\n+AddAsync(entity)\n+UpdateAsync(entity)\n+DeleteAsync(entity)\n+ExistsAsync(id)"]

class ProductRepository ["ProductRepository : GenericRepository<Product>, IProductRepository\n+SearchProductsAsync(...)\n+TryDecrementStockAsync(...)\n+GetByIdAsync(id)\n+GetAllAsync()"]

class CartRepository ["CartRepository : GenericRepository<Cart>, ICartRepository\n+GetCartByUserIdAsync(...)\n+GetCartItemAsync(...)\n+AddCartItemAsync(...)\n+ClearCartAsync(...)"]

class OrderRepository ["OrderRepository : GenericRepository<Order>, IOrderRepository\n+GetOrdersByUserIdAsync(...)\n+GetOrderWithItemsAsync(...)\n+GenerateOrderNumberAsync()"]

class IplEcommerceDbContext ["IplEcommerceDbContext : DbContext\n+DbSet<User> Users\n+DbSet<Franchise> Franchises\n+DbSet<Product> Products\n+DbSet<Cart> Carts\n+DbSet<CartItem> CartItems\n+DbSet<Order> Orders\n+DbSet<OrderItem> OrderItems\n+SaveChanges/SaveChangesAsync (overrides)"]

class BaseEntity ["BaseEntity\n+Id: int\n+CreatedAt: DateTime\n+UpdatedAt: DateTime?"]

class Product ["Product : BaseEntity\n+Name: string\n+Description: string\n+Price: decimal\n+Type: ProductType\n+StockQuantity: int\n+IsAvailable: bool\n+FranchiseId: int\n+Franchise: Franchise\n+CartItems: ICollection<CartItem>\n+OrderItems: ICollection<OrderItem>"]

class Cart ["Cart : BaseEntity\n+UserId: int\n+User: User\n+CartItems: ICollection<CartItem>"]

class CartItem ["CartItem : BaseEntity\n+CartId: int\n+ProductId: int\n+Quantity: int\n+Cart: Cart\n+Product: Product"]

class Order ["Order : BaseEntity\n+UserId: int\n+OrderNumber: string\n+TotalAmount: decimal\n+Status: OrderStatus\n+OrderItems: ICollection<OrderItem>\n+User: User"]

class OrderItem ["OrderItem : BaseEntity\n+OrderId: int\n+ProductId: int\n+Quantity: int\n+UnitPrice: decimal\n+TotalPrice: decimal\n+Order: Order\n+Product: Product"]

class Franchise ["Franchise : BaseEntity\n+Name: string\n+ShortName: string\n+City: string"]

class User ["User : BaseEntity\n+Email: string\n+FirstName: string\n+LastName: string\n+Cart: Cart\n+Orders: ICollection<Order>"]

class ProductsController ["ProductsController : ControllerBase\n- _unitOfWork: IUnitOfWork\n+GetProducts()\n+GetProduct(id)\n+SearchProducts(...)\n+GetProductsByFranchise(...)\n+GetProductsByType(...)"]

class OrdersController ["OrdersController : ControllerBase\n- _unitOfWork: IUnitOfWork\n+GetUserOrders(userId)\n+GetOrder(id)\n+CreateOrder(request)\n+UpdateOrderStatus(id, request)"]

%% Inheritance / Realization
IUnitOfWork <|.. UnitOfWork
IGenericRepository~T~ <|.. GenericRepository~T~
IProductRepository <|.. ProductRepository
ICartRepository <|.. CartRepository
IOrderRepository <|.. OrderRepository
GenericRepository~T~ <|-- ProductRepository
GenericRepository~T~ <|-- CartRepository
GenericRepository~T~ <|-- OrderRepository
BaseEntity <|-- Product
BaseEntity <|-- Cart
BaseEntity <|-- CartItem
BaseEntity <|-- Order
BaseEntity <|-- OrderItem
BaseEntity <|-- Franchise
BaseEntity <|-- User

%% Associations / Dependencies
UnitOfWork ..> IplEcommerceDbContext
ProductRepository ..> IplEcommerceDbContext
CartRepository ..> IplEcommerceDbContext
OrderRepository ..> IplEcommerceDbContext
ProductsController ..> IUnitOfWork
OrdersController ..> IUnitOfWork
Product "1" o-- "1" Franchise : "Franchise"
Product "1" o-- "*" CartItem : "CartItems"
Product "1" o-- "*" OrderItem : "OrderItems"
Cart "1" o-- "*" CartItem : "CartItems"
Order "1" o-- "*" OrderItem : "OrderItems"
User "1" o-- "1" Cart : "Cart"
User "1" o-- "*" Order : "Orders"
IplEcommerceDbContext ..> Product
IplEcommerceDbContext ..> Cart
IplEcommerceDbContext ..> Order
