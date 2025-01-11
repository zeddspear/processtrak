using System.Text.RegularExpressions;

namespace processtrak_backend.Utils
{
    public static class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public static bool IsValidPhone(string phone)
        {
            return Regex.IsMatch(phone, @"^\+?[1-9]\d{1,14}$");
        }

        public static bool IsValidPassword(string password)
        {
            return Regex.IsMatch(
                password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$"
            );
        }
    }
}
