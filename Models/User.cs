using System;

namespace YourNamespace.Models
{
    public class User
    {
        public int Id { get; set; } // ID único para el usuario
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Valor predeterminado es "User"
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; } = DateTime.UtcNow;
    }
}
