using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using processtrak_backend.Dto;
using processtrak_backend.Models;
using processtrak_backend.Services;
using processtrak_backend.Utils;

namespace processtrak_backend.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto userDto)
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
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            var user = await _authService.AuthenticateUser(userLogin.email, userLogin.password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            var expiryTime = DateTime.UtcNow.AddHours(2); // Set token expiration time

            // Generate JWT token and save session
            var token = _authService.GenerateJwtToken(user, expiryTime);
            var sessionSaved = await _authService.SaveSession(user, token, expiryTime);

            if (!sessionSaved)
            {
                return BadRequest("Failed to save session.");
            }

            return Ok(
                new
                {
                    message = "Login successful.",
                    token,
                    expiryTime,
                }
            );
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
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
            [FromBody] string email,
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

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromHeader] string token)
        {
            var result = await _authService.DeleteSession(token);
            if (!result)
            {
                return BadRequest("Invalid or expired session.");
            }

            return Ok("Logged out successfully.");
        }
    }
}
