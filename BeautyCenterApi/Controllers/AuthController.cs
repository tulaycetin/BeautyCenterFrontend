using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using BeautyCenterApi.Services;
using BeautyCenterApi.DTOs;
using BeautyCenterApi.Models;

namespace BeautyCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(AuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _authService.LoginAsync(loginDto.Username, loginDto.Password);
                
                if (token == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var user = await _authService.GetUserByUsernameAsync(loginDto.Username);
                
                return Ok(new
                {
                    token = token,
                    user = new
                    {
                        id = user!.Id,
                        username = user.Username,
                        email = user.Email,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        role = user.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var user = _mapper.Map<User>(registerDto);
                var createdUser = await _authService.RegisterAsync(user, registerDto.Password);

                if (createdUser == null)
                {
                    return BadRequest(new { message = "Username or email already exists" });
                }

                return Ok(new
                {
                    message = "User registered successfully",
                    user = new
                    {
                        id = createdUser.Id,
                        username = createdUser.Username,
                        email = createdUser.Email,
                        firstName = createdUser.FirstName,
                        lastName = createdUser.LastName,
                        role = createdUser.Role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
    }
}