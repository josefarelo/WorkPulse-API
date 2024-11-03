namespace WorkPulseAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public required string Role { get; set; }
    
        // Propiedades para el manejo del refresh token
        public required string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}