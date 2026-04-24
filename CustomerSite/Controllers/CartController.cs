using Microsoft.AspNetCore.Mvc;
using CustomerSite.Interfaces;
using CustomerSite.Models;


namespace CustomerSite.Controllers;

public class CartController : Controller
{
    private readonly IProductApiClient _productApiClient;
    private readonly ICartService _cartService; 
    public CartController(IProductApiClient productApiClient, ICartService cartService)
    {
        _productApiClient = productApiClient;
        _cartService = cartService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = _cartService.GetCart();
        return View(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Add(int Id, string Color, string Size , int Quantity = 1)
    {
        
        var product = await _productApiClient.GetProductByIdAsync(Id);
        if (product == null) return NotFound();

        
        _cartService.AddItem(new CartItem 
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Price = product.Price,
            Quantity = Quantity,
            Size = Size,
            Color = Color
        });

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Remove(int productId, string color, string size)
    {
        _cartService.RemoveItem(productId,color,size);
        return RedirectToAction("Index");
    }
}