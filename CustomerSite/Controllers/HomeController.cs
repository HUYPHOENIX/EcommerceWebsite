using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CustomerSite.Models;
using SharedViewModel.DTOs;
using System.Text.Json;

namespace CustomerSite.Controllers;

public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpIHttpClientFactory;
    public HomeController (IHttpClientFactory httpClientFactory)
    {
        _httpIHttpClientFactory = httpClientFactory;
    }
    public async Task<IActionResult> Index()
    {
        // 1. Create the client we configured in Program.cs
        var client = _httpIHttpClientFactory.CreateClient("Api");

        // 2. Make a GET request to your API
        var response = await client.GetAsync("api/products");

        if(response.IsSuccessStatusCode)
        {
            var products = await response.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();

            //We will check null here and check empty outside
            if(products == null)
            {
                return View(new List<ProductDto>());
            }
            
            return View(products);
        }
        return View(new List<ProductDto>());
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Error()
    {
        return View(Request.Body);
    }
}
