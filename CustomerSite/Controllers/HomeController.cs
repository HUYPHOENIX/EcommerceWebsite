using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CustomerSite.Models;
using SharedViewModel.DTOs;
using System.Text.Json;
using Microsoft.VisualBasic;
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
