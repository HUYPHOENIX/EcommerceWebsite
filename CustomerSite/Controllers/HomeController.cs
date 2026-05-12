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

        return View();
    }
}
