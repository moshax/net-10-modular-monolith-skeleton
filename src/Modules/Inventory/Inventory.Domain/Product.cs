using BuildingBlocks;

namespace Inventory.Domain;

public sealed class Product : Entity
{
    private Product() { }

    public Product(Guid id, string sku, string name, int qtyOnHand, DateTimeOffset createdAt)
    {
        Id = id;
        Sku = sku;
        Name = name;
        QtyOnHand = qtyOnHand;
        CreatedAt = createdAt;
    }

    public string Sku { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public int QtyOnHand { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
}
