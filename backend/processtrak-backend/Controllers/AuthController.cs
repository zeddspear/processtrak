using Microsoft.AspNetCore.Mvc;
using processtrak_backend.DTO;
using processtrak_backend.Models;
using processtrak_backend.Services;
using processtrak_backend.Utils;

namespace processtrak_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto userDto)
        {
            if (
                !ValidationHelper.IsValidEmail(userDto.email)
                || !ValidationHelper.IsValidPhone(userDto.phone)
                || !ValidationHelper.IsValidPassword(userDto.password)
            )
            {
                return BadRequest("Invalid input.");
            }

            var user = new User
            {
                name = userDto.name,
                email = userDto.email,
                password = userDto.password,
                phone = userDto.phone,
            };

            var result = await _authService.RegisterUser(user);
            if (!result)
            {
                return BadRequest("Registration failed.");
            }

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _authService.AuthenticateUser(email, password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok("Login successful.");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var otpCode = await _authService.GenerateOtp(email);
            if (otpCode == null)
            {
                return BadRequest("Failed to generate OTP.");
            }

            return Ok("OTP sent to email.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(
            string email,
            string code,
            string newPassword
        )
        {
            var isValidOtp = await _authService.ValidateOtp(email, code);
            if (!isValidOtp)
            {
                return BadRequest("Invalid or expired OTP.");
            }

            var isSuccess = await _authService.ResetPassword(email, newPassword);
            if (!isSuccess)
            {
                return BadRequest("Failed to reset password.");
            }

            return Ok("Password reset successfully.");
        }
    }
}
