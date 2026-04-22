using System.Text.Json;
using CustomerSite.Interfaces;
using CustomerSite.Models;

public class CartService : ICartService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private ISession? Session => _httpContextAccessor.HttpContext?.Session;

    public CartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public List<CartItem> GetCart()
    {
        return GetJson<List<CartItem>>("ShoppingCart") ?? new List<CartItem>();
    }

    public void AddItem(CartItem item)
    {
        var cart = GetCart();

        var existingItem = cart.FirstOrDefault(c =>
            c.ProductId == item.ProductId &&
            c.Size == item.Size &&
            c.Color == item.Color);

        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            cart.Add(item);
        }
        if (Session != null)
        {
            SetJson("ShoppingCart", cart);
        }
    }

    public void ClearCart()
    {
        Session?.Remove("ShoppingCart");
    }

    // ---------------------------------------------------
    // 3. PRIVATE HELPER METHODS (Replaces Extensions)
    // ---------------------------------------------------

    private void SetJson(string key, object value)
    {
        if (Session == null) return;

        var jsonText = JsonSerializer.Serialize(value);
        Session.SetString(key, jsonText);
    }

    private T? GetJson<T>(string key)
    {
        if (Session == null) return default;

        var sessionData = Session.GetString(key);

        if (string.IsNullOrEmpty(sessionData))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(sessionData);
    }
}