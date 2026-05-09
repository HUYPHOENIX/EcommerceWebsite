using System.Security.Claims;
using CustomerSite.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
namespace CustomerSite.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService accountService)
    {
        _authService = accountService;
    }
    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginRequestDto request, string returnUrl = null!)
    {
        if (!ModelState.IsValid) return View(request);

        var result = await _authService.LoginAsync(request);

        if (result != null && result.IsSuccess)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(result.AccessToken);
            var claims = jwtToken.Claims.ToList();
            claims.Add(new Claim("AccessToken", result.AccessToken));
            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = jwtToken.ValidTo,
                IsPersistent = true
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal, authProperties);
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
        ViewData["ReturnUrl"] = returnUrl;
        return View(request);
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        if (!ModelState.IsValid) return View(request);
        var result = await _authService.RegisterAsync(request);
        if (result != null)
        {
            return RedirectToAction("Index", "Home");
        }
        ModelState.AddModelError(string.Empty, "Đăng ký thất bại. Vui lòng thử lại.");
        return View(request);
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
}