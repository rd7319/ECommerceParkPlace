using IplEcommerce.Domain.Common;

namespace IplEcommerce.Domain.Entities;

public class Cart : BaseEntity
{
    public int UserId { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    
    // Computed property
    public decimal TotalAmount => CartItems.Sum(item => item.Quantity * item.Product.Price);
}