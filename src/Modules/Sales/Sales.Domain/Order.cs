using BuildingBlocks;

namespace Sales.Domain;

public sealed class Order : Entity
{
    private Order() { }

    public Order(Guid id, string orderNo, string customerName, decimal total, string status, DateTimeOffset createdAt)
    {
        Id = id;
        OrderNo = orderNo;
        CustomerName = customerName;
        Total = total;
        Status = status;
        CreatedAt = createdAt;
    }

    public string OrderNo { get; private set; } = string.Empty;
    public string CustomerName { get; private set; } = string.Empty;
    public decimal Total { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
}
