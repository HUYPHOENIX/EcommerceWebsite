using BussinessLogic.Services;
using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            if (!result.IsSuccess)
                return BadRequest(new { result.Message });

            return StatusCode(201, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginCustomerAsync(request);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Message });

            return Ok(result);

        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAdminAsync(request);
            
            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Message });

            return Ok(result);
        }
    }
}