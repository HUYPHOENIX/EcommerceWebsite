using BussinessLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SharedViewModel.DTOs;

namespace Api.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
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

