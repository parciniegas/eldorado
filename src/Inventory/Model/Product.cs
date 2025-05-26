using System;

namespace Inventory.Model;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public int StockReserved { get; set; }
    public int StockAvailable => StockQuantity - StockReserved;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
