using Microsoft.EntityFrameworkCore;
using Orders.Model;

namespace Orders.Services;

public class OrderService(OrdersDbContext context) : IOrderServices
{
    private readonly OrdersDbContext _context = context;

    public async Task<int> CreateOrderAsync(Order order)
    {
        order.OrderDate = DateTime.UtcNow;
        order.Status = OrderStatus.Pending;
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order.Id;
    }

    public async Task<Order?> GetOrderAsync(int orderId)
    {
        return await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<List<Order>> GetOrdersAsync(int page = 1, int pageSize = 10)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<bool> ProcessOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null || order.Status != OrderStatus.Pending)
        {
            return false; // Or throw an exception
        }

        order.Status = OrderStatus.Processing;
        // Here you might add logic to interact with Inventory or Payment services
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CompleteOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null || order.Status != OrderStatus.Processing) // Assuming an order must be Processing to be Completed
        {
            return false; // Or throw an exception
        }

        order.Status = OrderStatus.Completed;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelOrderAsync(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order == null || order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
        {
            return false; // Or throw an exception
        }

        // Add logic here to release any reserved inventory if applicable
        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }
}
