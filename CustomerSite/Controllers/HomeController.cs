using Microsoft.AspNetCore.Mvc;
using CustomerSite.Interfaces;

namespace CustomerSite.Controllers;

public class HomeController : Controller
{
    private readonly IProductApiClient _IProductApiClient;
    public HomeController (IProductApiClient productApiClient)
    {
       _IProductApiClient = productApiClient;
    }
    public async Task<IActionResult> Index()
    {
        var products = await _IProductApiClient.GetAllProductsAsync();
        if(products == null)
        {
            return NotFound();
        }
        return View(products);
    }
}
