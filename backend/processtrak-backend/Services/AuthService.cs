using Microsoft.EntityFrameworkCore;
using processtrak_backend.Api.data;
using processtrak_backend.Models;

namespace processtrak_backend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<bool> RegisterUser(User user)
        {
            // Password Hashing using BCrypt
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> AuthenticateUser(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.email == email);
            if (user == null)
                return null;

            // Password validation using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.password))
                return null;

            return user;
        }

        public async Task<string> GenerateOtp(string email)
        {
            var otpCode = new OtpCode
            {
                email = email,
                code = new Random().Next(100000, 999999).ToString(),
                expiryTime = DateTime.UtcNow.AddMinutes(10),
            };

            _context.OtpCodes.Add(otpCode);
            await _context.SaveChangesAsync();

            // Send OTP to email
            // (Assuming you have an email service)

            return otpCode.code;
        }

        public async Task<bool> ValidateOtp(string email, string code)
        {
            var otp = await _context.OtpCodes.SingleOrDefaultAsync(o =>
                o.email == email && o.code == code
            );
            if (otp == null || otp.expiryTime < DateTime.UtcNow)
                return false;

            return true;
        }

        public async Task<bool> ResetPassword(string email, string newPassword)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.email == email);
            if (user == null)
                return false;

            // Password Hashing using BCrypt
            user.password = BCrypt.Net.BCrypt.HashPassword(user.password);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
