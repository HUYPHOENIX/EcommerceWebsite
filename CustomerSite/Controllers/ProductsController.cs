using CustomerSite.Interfaces;
using CustomerSite.Services;
using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;

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