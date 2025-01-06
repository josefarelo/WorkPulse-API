using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

public class PullRequest
{
    [Key]
    public int Id { get; set; } // Clave primaria
    [JsonPropertyName("user")]
    public User? Author { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("merged_at")]
    public DateTime? MergedAt { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
    public string Repository { get; set; } = string.Empty;
}

public class User
{
    [JsonPropertyName("login")]
    public string? Login { get; set; }

    [JsonPropertyName("html_url")]
    public string? HtmlUrl { get; set; }
        public int Id { get; set; } // ID único para el usuario
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Valor predeterminado es "User"
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiration { get; set; } = DateTime.UtcNow;
}