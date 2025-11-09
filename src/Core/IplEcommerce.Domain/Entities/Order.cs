using IplEcommerce.Domain.Common;
using IplEcommerce.Domain.Enums;

namespace IplEcommerce.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime OrderDate { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? TrackingNumber { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}