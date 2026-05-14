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
    public async Task<IActionResult> Index(int? categoryId, int page = 1)
    {
        int ItemInPage = 4;
        var pagedProducts = await _productApiService.GetProductsByPageAsync(categoryId, page, ItemInPage);
        if (pagedProducts == null)
        {
            TempData["ErrorMessage"] = "Không Thể kết nối đến Api.";
            return RedirectToAction("Error", "Home");
        }
        ViewBag.CurrentCategory = categoryId;
        return View(pagedProducts);
    }

    public IActionResult Error()
    {
        return View();
    }
}
