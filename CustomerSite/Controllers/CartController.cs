using Microsoft.AspNetCore.Mvc;
using CustomerSite.Interfaces;
using CustomerSite.Models;


namespace EcommerceShop.Web.Controllers;

public class CartController : Controller
{
    private readonly IProductApiClient _productApiClient;
    private readonly ICartService _cartService; // 1. Create a spot for the manager

    // 2. Ask for the manager in the constructor
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
}