using Microsoft.AspNetCore.Mvc;
using WorkPulseAPI.Models;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;
using System.Linq;

namespace WorkPulseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly JwtService _jwtService;

        public AuthController(UserService userService, JwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        // Ruta de register 
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (_userService.UserExists(model.Email))
            {
                return BadRequest(new { message = "User already exists" });
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var user = new User { Username = model.Username, Email = model.Email, PasswordHash = hashedPassword };

            _userService.CreateUser(user);

            return Ok(new { message = "User registered successfully" });
        }

        // Ruta de login para obtener el JWT 
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login) // La acción de login verifica las credenciales y, si son correctas, genera un token usando el JwtService
        {
            // Acá se valida el login
            if (login.Username == "user" && login.Password == "password")
            {
                var token = _jwtService.GenerateToken(login.Username); // Este método crea un JWT con el usuario autenticado
                return Ok(new { token });
            }
            return Unauthorized();
        }

        // Ruta para verificar si la API responde
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API is running");
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            // Obtener el username del token JWT
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            // Obtener la información del usuario y verificar si existe
            var user = _userService.GetUserByUsername(username);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Devolver el perfil del usuario sin la contraseña
            return Ok(new
            {
                Username = user.Username,
                Email = user.Email
            });
        }
    }

    public class RegisterModel
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
