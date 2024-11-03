using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;

public class JwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    public JwtService(IConfiguration config)
    {
        _secretKey = config["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key is missing in configuration");
        _issuer = config["Jwt:Issuer"] ?? throw new ArgumentNullException("JWT Issuer is missing in configuration");
        _audience = config["Jwt:Audience"] ?? throw new ArgumentNullException("JWT Audience is missing in configuration");
    }

    public string GenerateToken(string username, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role) // Añade el rol al token
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
{
    return new RefreshToken
    {
        Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), // Genera un token
        Expiration = DateTime.UtcNow.AddDays(7) // El refresh token expira en x cantidad de días
    };
}
}
