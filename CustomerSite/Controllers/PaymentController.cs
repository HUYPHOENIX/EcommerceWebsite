using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;
using CustomerSite.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CustomerSite.Controllers;

[Authorize]
public class PaymentController : Controller
{
    private readonly IOrderApiService _orderApiService;
    private readonly ICartService _cartService;

    public PaymentController(IOrderApiService orderApiService, ICartService cartService)
    {
       _orderApiService = orderApiService;
        _cartService = cartService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        var Items = _cartService.GetCart();
        return View(Items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder()
    {
        var accessToken = User.FindFirst("AccessToken")?.Value;
        if(string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login", "Auth");
        }
        var cart = _cartService.GetCart();
        var orderRequest = new OrderRequestDto
        { 
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

        var newOrderId = await  _orderApiService.CreateOrderAsync(orderRequest, accessToken!);
        if (newOrderId.HasValue)
        {
            _cartService.ClearCart();
            return RedirectToAction("Success", new { id = newOrderId.Value });
        }
        ModelState.AddModelError("", "Không thanh toán được đơn hàng. Vui lòng thử lại.");
        return View("Index", cart);
    }
    
    [HttpGet]
    public IActionResult Success(int id)
    {
        ViewBag.OrderId = id;
        return View();
    }
}