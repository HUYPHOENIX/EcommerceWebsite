using CustomerSite.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CustomerSite.Controllers;

public class ProductsController : Controller
{
    private readonly IProductApiService _productApiService;
    private readonly ICategoryApiService _categoryApiService;
    public ProductsController(IProductApiService productApiService, ICategoryApiService categoryApiService)
    {
        _productApiService = productApiService;
        _categoryApiService = categoryApiService;
    }
    public async Task<IActionResult> Index(int? categoryId, int page = 1)
    {
        var cateList = await _categoryApiService.GetCate();

        int ItemInPage = 6;
        var pagedProducts = await _productApiService.GetProductsByPageAsync(categoryId, page, ItemInPage);
        if (pagedProducts == null || cateList == null)
        {
            TempData["ErrorMessage"] = "Không Thể kết nối đến Api.";
            return RedirectToAction("Error", "Home");
        }
        ViewBag.CateList = cateList.ToList();
        ViewBag.CurrentCategory = categoryId;
        return View(pagedProducts);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var product = await _productApiService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }
}