using CustomerSite.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CustomerSite.Controllers;

public class ProductsController : Controller
{
    private readonly IProductApiService _productApiService;
    public ProductsController(IProductApiService productApiService)
    {
        _productApiService = productApiService;
    }
    public async Task<IActionResult> Index(int? categoryId, int page = 1)
    {
        //Index just need to received cateId or Page number and Item in it will be set hardcode
        int ItemInPage = 6;
        var pagedProducts = await _productApiService.GetProductsByPageAsync(categoryId, page, ItemInPage);
        if (pagedProducts == null)
        {
            return NotFound();
        }
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