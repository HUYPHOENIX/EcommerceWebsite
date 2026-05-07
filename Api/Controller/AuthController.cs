using BussinessLogic.Entities;
using BussinessLogic.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedViewModel.DTOs;

namespace Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly UserManager<User> _userManager;
        public AuthController(IAuthRepository authRepository, UserManager<User> userManager)
        {
            _authRepository = authRepository;
            _userManager = userManager;
        }

        // [HttpPost("refresh")]
        // public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto request)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     var newTokenResponse = await _authRepository.RefreshTokenAsync(request);
        //     if (!newTokenResponse.IsSuccess)
        //     {
        //         return Unauthorized(newTokenResponse);
        //     }
        //     return Ok(newTokenResponse);
        // }

        [HttpPost("admin-login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginRequestDto request)
        {
            var response = await _authRepository.LoginAsync(request);
            if (response.IsSuccess == false)
            {
                return Unauthorized(response.IsSuccess);
            }
            if(response.Roles == null || !response.Roles.Contains("Admin"))
            {
                return Forbid();
            }
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            var response = await _authRepository.RegisterAsync(request);
            if (!response.IsSuccess)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _authRepository.LoginAsync(request);
            if (!response.IsSuccess)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}

