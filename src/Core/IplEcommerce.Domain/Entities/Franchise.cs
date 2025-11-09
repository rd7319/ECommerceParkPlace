using IplEcommerce.Domain.Common;

namespace IplEcommerce.Domain.Entities;

public class Franchise : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}