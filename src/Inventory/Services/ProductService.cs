using Inventory.Model;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Services;

public class ProductService(InventoryDbContext context) : IProductServices
{
    private readonly InventoryDbContext _context = context;

    public Product CreateProduct(Product product)
    {
        _context.Products.Add(product);
        _context.SaveChanges();
        return product;
    }

    public void DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }

    public Product[] GetAllProducts()
    {
        return _context.Products.ToArray();
    }

    public Product? GetProductById(int id)
    {
        return _context.Products.Find(id);
    }

    public Product UpdateProduct(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        _context.SaveChanges();
        return product;
    }

    public void ReserveProduct(int id, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to reserve must be positive.");
        }

        var product = _context.Products.Find(id)
            ?? throw new ArgumentException($"Product with ID {id} not found.", nameof(id));
        if (product.StockAvailable < quantity)
        {
            throw new InvalidOperationException($"Insufficient stock for product ID {id}. Available: {product.StockAvailable}, Requested: {quantity}.");
        }

        product.StockReserved += quantity;
        product.UpdatedAt = DateTime.UtcNow;
        _context.SaveChanges();
    }

    public void ReleaseReserve(int id, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to release must be positive.");
        }

        var product = _context.Products.Find(id)
            ?? throw new ArgumentException($"Product with ID {id} not found.", nameof(id));
        product.StockReserved -= quantity;
        product.UpdatedAt = DateTime.UtcNow;
        _context.SaveChanges();
    }

    public void ConfirmReserve(int id, int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity to confirm must be positive.");
        }

        var product = _context.Products.Find(id)
            ?? throw new ArgumentException($"Product with ID {id} not found.", nameof(id));

        if (product.StockReserved < quantity)
        {
            throw new InvalidOperationException($"Insufficient reserved stock for product ID {id}. Reserved: {product.StockReserved}, Requested: {quantity}.");
        }
        product.StockQuantity -= quantity;
        product.StockReserved -= quantity;
        product.UpdatedAt = DateTime.UtcNow;
        _context.SaveChanges();
    }
}
