using Microsoft.AspNetCore.Mvc;
using CustomerSite.Interfaces;

namespace CustomerSite.Controllers;

public class HomeController : Controller
{
    private readonly IProductApiService _productApiService;
    public HomeController(IProductApiService productApiClient)
    {
        _productApiService = productApiClient;
    }
    public async Task<IActionResult> Index()
    {
        var products = await _productApiService.GetAllProductsAsync();
        if (products == null)
        {
            return NotFound();
        }
        return View(products);
    }
}
