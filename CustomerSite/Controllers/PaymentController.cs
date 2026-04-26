using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;
using CustomerSite.Interfaces;

namespace CustomerSite.Controllers;

public class PaymentController : Controller
{
    private readonly IOrderApiService _orderApiService;
    private readonly ICartService _cartService;

    public PaymentController(IOrderApiService orderApiService, ICartService cartService)
    {
       _orderApiService = orderApiService;
        _cartService = cartService;
    }

    public IActionResult Index()
    {
        var Items = _cartService.GetCart();
        return View(Items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder()
    {
        var cart = _cartService.GetCart();
        var orderRequest = new OrderRequestDto
        {
            UserId = "Guest_User", // Update this code after feature Authorize/Authentication are developed
            Items = cart.Select(item => new OrderItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Price = item.Price,
                Quantity = item.Quantity,
                Size = item.Size!,
                Color = item.Color!
            }).ToList()
        };

        var newOrderId = await  _orderApiService.CreateOrderAsync(orderRequest);
        if (newOrderId.HasValue)
        {
            _cartService.ClearCart();
            return RedirectToAction("Success", new { id = newOrderId.Value });
        }
        ModelState.AddModelError("", "Unable to process payment. Please try again.");
        return View("Index", cart);


    }
    [HttpGet]
    public IActionResult Success(int id)
    {
        ViewBag.OrderId = id;
        return View();
    }
}