using CustomerSite.Models;

namespace CustomerSite.Interfaces;
public interface ICartService
{
    List<CartItem> GetCart();
    void AddItem(CartItem item);
    void RemoveItem(int productId, string color, string size);
    void ClearCart();
}