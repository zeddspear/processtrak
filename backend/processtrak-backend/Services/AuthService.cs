using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using processtrak_backend.Api.data;
using processtrak_backend.Models;
using processtrak_backend.Utils;

namespace processtrak_backend.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AuthService(
            AppDbContext context,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration
        )
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
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
            var user = await _context
                .Users.Include(u => u.UserSessions)
                .SingleOrDefaultAsync(u => u.email == email);
            if (user == null)
                return null;

            // Password validation using BCrypt
            if (!BCrypt.Net.BCrypt.Verify(password, user.password))
                return null;

            return user;
        }

        public async Task<bool> SaveSession(User user, string token, DateTime expiryTime)
        {
            var session = new UserSession
            {
                UserId = user.id,
                Token = token,
                ExpiryTime = expiryTime,
                User = user,
            };

            user.UserSessions!.Add(session); // Add to the user's session collection
            _context.UserSessions.Add(session); // Save the session to the database
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteSession(string token)
        {
            var session = await _context
                .UserSessions.Include(s => s.User)
                .SingleOrDefaultAsync(s => s.Token == token);
            if (session == null || session.ExpiryTime < DateTime.UtcNow)
            {
                return false; // Invalid or expired session
            }

            _context.UserSessions.Remove(session);
            await _context.SaveChangesAsync();
            return true;
        }

        public string GenerateJwtToken(User user, DateTime expiryTime)
        {
            var key = Encoding.ASCII.GetBytes(GlobalVariables.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.PrimarySid, user.id.ToString()),
                        new Claim(ClaimTypes.Email, user.email),
                    }
                ),
                Expires = expiryTime, // Token validity
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
            user.password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
