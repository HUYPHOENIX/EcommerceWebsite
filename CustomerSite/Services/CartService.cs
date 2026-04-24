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

    public void RemoveItem(int productId, string color, string size)
    {
        var cart = GetCart();
        var existingItem = cart.FirstOrDefault(c =>
            c.ProductId == productId &&
            c.Size == size &&
            c.Color == color);

        if(existingItem != null)
        {
            cart.Remove(existingItem);
            SetJson("ShoppingCart", cart);
        }
    }
    public void ClearCart()
    {
        Session?.Remove("ShoppingCart");
    }


    private void SetJson(string key, object value)
    {
        if (Session == null) return;
        //This code is to turn C# data in to Json type
        var jsonText = JsonSerializer.Serialize(value);
        //Add this to session with key "ShoppingCart" value is json form of cart
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
        // This code is reverse of thing above from json to c#
        return JsonSerializer.Deserialize<T>(sessionData);
    }
}