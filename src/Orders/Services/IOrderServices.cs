using Orders.Model;

namespace Orders.Services;

public interface IOrderServices
{
    public Task<int> CreateOrderAsync(Order order);
    public Task<Order?> GetOrderAsync(int orderId);
    public Task<List<Order>> GetOrdersAsync(int page = 1, int pageSize = 10);
    public Task<bool> ProcessOrderAsync(int orderId);
    public Task<bool> CompleteOrderAsync(int orderId);
    public Task<bool> CancelOrderAsync(int orderId);
}
