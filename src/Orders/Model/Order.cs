using System;

namespace Orders.Model;

public class Order
{
    public int Id { get; set; }
    public required string CustomerName { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? Notes { get; set; }
    public List<OrderItem> Items { get; set; } = [];
}
