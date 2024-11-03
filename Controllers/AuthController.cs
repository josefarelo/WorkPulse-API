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

            // Inicializa RefreshToken y RefreshTokenExpiration
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword,
                Role = model.Role ?? "User", // Asigna el rol proporcionado o "User" por defecto
                RefreshToken = "", // Inicializa con un valor vacío o genera un token si es necesario
                RefreshTokenExpiration = DateTime.UtcNow // Puedes ajustar esta fecha según tus requisitos
            };

            _userService.CreateUser(user);

            return Ok(new { message = "User registered successfully" });
        }

        // Ruta de login para obtener el JWT 
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel login)
        {
            var user = _userService.GetUserByUsername(login.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
            {
                var token = _jwtService.GenerateToken(user.Username, user.Role); // Pasa el rol
                // Genera el refresh token
                var refreshToken = _jwtService.GenerateRefreshToken();
                user.RefreshToken = refreshToken.Token;
                user.RefreshTokenExpiration = refreshToken.Expiration;

                // Guardar el refresh token en la base de datos o en el servicio de usuarios
                _userService.UpdateUser(user);

                return Ok(new { token, refreshToken = refreshToken.Token });
            }

            return Unauthorized();
        }

        // Endpoint para obtener un nuevo access token y refresh token
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest(new { message = "The refresh token is required." });
            }

            var user = _userService.GetUserByRefreshToken(request.RefreshToken);

            if (user == null || user.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            // Generar un nuevo access token
            var accessToken = _jwtService.GenerateToken(user.Username, user.Role);

            // Generar un nuevo refresh token
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken.Token;
            user.RefreshTokenExpiration = newRefreshToken.Expiration;
            _userService.UpdateUser(user);

            return Ok(new { accessToken, refreshToken = newRefreshToken.Token });
        }

        // Ruta para acceder al perfil
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

        // Endpoint para actualizar el perfil de usuario
        [Authorize]
        [HttpPut("update-profile")]
        public IActionResult UpdateProfile([FromBody] UpdateProfileModel model)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Invalid username" });
            }

            var user = _userService.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Verificar si el nuevo email o username ya están en uso por otro usuario
            if (model.Email != null && model.Email != user.Email && _userService.UserExists(model.Email))
            {
                return BadRequest(new { message = "Email is already in use" });
            }

            if (model.Username != null && model.Username != user.Username && _userService.GetUserByUsername(model.Username) != null)
            {
                return BadRequest(new { message = "Username is already in use" });
            }

            // Actualizar el email y el nombre de usuario (si se permite)
            user.Email = model.Email ?? user.Email;
            bool usernameChanged = false;
            if (model.Username != null && model.Username != user.Username)
            {
                user.Username = model.Username;
                usernameChanged = true;
            }

            _userService.UpdateUser(user);

            // Si se cambia el username, generar un nuevo access token
            if (usernameChanged)
            {
                var newToken = _jwtService.GenerateToken(user.Username, user.Role);
                return Ok(new { message = "Profile updated successfully", accessToken = newToken });
            }

            return Ok(new { message = "Profile updated successfully" });
        }

        // Endpoint para cambiar la contraseña
        [Authorize]
        [HttpPut("change-password")]
        public IActionResult ChangePassword([FromBody] ChangePasswordModel model)
        {
            var username = User.Identity?.Name;
            
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { message = "Invalid username" });
            }

            var user = _userService.GetUserByUsername(username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash))
            {
                return Unauthorized(new { message = "Invalid current password" });
            }

            // Cambiar la contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _userService.UpdateUser(user);

            return Ok(new { message = "Password changed successfully" });
        }

        // Ruta para verificar si la API responde
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API is running");
        }

        // Endpoint accesible solo para administradores
        [Authorize(Roles = "Admin")]
        [HttpGet("all-performance")]
        public IActionResult GetAllPerformance()
        {
            return Ok("Desempeño de todos los usuarios (solo para Admins).");
        }

        // Endpoint accesible solo para usuarios
        [Authorize(Roles = "User")]
        [HttpGet("performance")]
        public IActionResult GetUserPerformance()
        {
            var username = User.Identity?.Name;
            return Ok($"Desempeño del usuario {username}.");
        }
    }
}
