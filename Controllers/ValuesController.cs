using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WorkPulseAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuesController : ControllerBase
    {
        // Ruta pública, accesible sin autenticación
        [HttpGet("public")]
        public IActionResult GetPublic()
        {
            return Ok(new { message = "Esta es una ruta pública" });
        }

        // Ruta protegida, solo accesible con JWT
        [Authorize]
        [HttpGet("protected")]
        public IActionResult GetProtected()
        {
            return Ok(new { message = "Esta es una ruta protegida. Solo para usuarios autenticados." });
        }
    }
}
