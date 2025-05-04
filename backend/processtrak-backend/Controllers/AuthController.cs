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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

            var expiryTime = DateTime.UtcNow.AddDays(30); // Set token expiration time

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

        [HttpPost("guest-login")]
        public async Task<IActionResult> GuestLogin()
        {
            var guestUser = await _authService.CreateGuestUser();

            var expiryTime = DateTime.UtcNow.AddHours(2); // Or shorter for guest
            var token = _authService.GenerateJwtToken(guestUser, expiryTime);
            var sessionSaved = await _authService.SaveSession(guestUser, token, expiryTime);

            if (!sessionSaved)
            {
                return BadRequest("Failed to save session.");
            }

            return Ok(
                new
                {
                    message = "Guest login successful.",
                    token,
                    expiryTime,
                }
            );
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] UserForgotPasswordDto body)
        {
            var otpCode = await _authService.GenerateOtp(body.email);
            if (otpCode == null)
            {
                return BadRequest("Failed to generate OTP.");
            }

            return Ok(new { message = "Otp sent to your provided email", body.email });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordDto body)
        {
            var isValidOtp = await _authService.ValidateOtp(body.email, body.code);
            if (!isValidOtp)
            {
                return BadRequest("Invalid or expired OTP.");
            }

            var isSuccess = await _authService.ResetPassword(body.email, body.newPassword);
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
