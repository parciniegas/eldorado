using Inventory.Model;

namespace Inventory.Services;

public interface IProductServices
{
    Product? GetProductById(int id);
    Product CreateProduct(Product product);
    Product UpdateProduct(Product product);
    void DeleteProduct(int id);
    Product[] GetAllProducts();
    void ReserveProduct(int id, int quantity);
    void ReleaseReserve(int id, int quantity);
    void ConfirmReserve(int id, int quantity);
}
