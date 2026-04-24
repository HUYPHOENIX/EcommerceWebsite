using CustomerSite.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CustomerSite.Controllers;
public class ProductsController : Controller
{
    private readonly IProductApiClient _IProductApiClient;
    public ProductsController(IProductApiClient productApiClient)
    {
        _IProductApiClient = productApiClient;
    }

    public async Task<IActionResult> Detail(int id)
    {
        var product = await _IProductApiClient.GetProductByIdAsync(id);
        if(product == null)
        {
            return NotFound();
        }
        return View(product);
    }
}