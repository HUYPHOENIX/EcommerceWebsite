using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;

public class ProductsController : Controller
{
    private readonly IHttpClientFactory _httpIHttpClientFactory;

    public ProductsController (IHttpClientFactory httpClientFactory)
    {
        _httpIHttpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Detail(int id)
    {
        var client = _httpIHttpClientFactory.CreateClient("Api");
        var response = await client.GetAsync($"api/products/{id}");
        if (response.IsSuccessStatusCode)
        {
            var product = await response.Content.ReadFromJsonAsync<ProductDto> ();

            if(product == null)
            {
                return NotFound();
            }

            return View(product);
        }
        return RedirectToAction("Index");
    }
}