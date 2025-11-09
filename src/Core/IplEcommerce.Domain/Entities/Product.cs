using IplEcommerce.Domain.Common;
using IplEcommerce.Domain.Enums;

namespace IplEcommerce.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public ProductType Type { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; } = true;
    public string Size { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    
    // Foreign Key
    public int FranchiseId { get; set; }
    
    // Navigation properties
    public virtual Franchise Franchise { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}