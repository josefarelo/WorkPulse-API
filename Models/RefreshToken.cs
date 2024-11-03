public class RefreshToken
{
    public required string Token { get; set; }
    public DateTime Expiration { get; set; }
    public int UserId { get; set; }
}