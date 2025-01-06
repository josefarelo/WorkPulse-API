using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Cargar el archivo .env al inicio
try
{
    Env.Load(); // Esto buscará automáticamente un archivo .env en la raíz del proyecto
    Console.WriteLine(".env cargado correctamente.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error al cargar el archivo .env: {ex.Message}");
}

// Acceder a la configuración desde builder
IConfiguration configuration = builder.Configuration;


// Obtener JWT settings desde las variables de entorno
var jwtSettings = new JwtSettings
{
    Key = Environment.GetEnvironmentVariable("JWT_KEY") ?? throw new InvalidOperationException("JWT_KEY no está definida en el archivo .env."),
    Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? throw new InvalidOperationException("JWT_ISSUER no está definida en el archivo .env."),
    Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? throw new InvalidOperationException("JWT_AUDIENCE no está definida en el archivo .env.")
};

if (string.IsNullOrEmpty(jwtSettings.Key) || 
    string.IsNullOrEmpty(jwtSettings.Issuer) || 
    string.IsNullOrEmpty(jwtSettings.Audience))
{
    throw new InvalidOperationException("Las variables de entorno JWT_KEY, JWT_ISSUER o JWT_AUDIENCE están faltando en el archivo .env.");
}

// Continuar con el uso de jwtSettings
var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true
    };
});

// Acceder a variable de entorno GITHUB_TOKEN
var githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");

// Verificar si el token se cargó correctamente
if (string.IsNullOrEmpty(githubToken))
{
    Console.WriteLine("El token de GitHub no se ha configurado o no se encuentra en el archivo .env.");
}
else
{
    Console.WriteLine("Token de GitHub cargado correctamente.");
}

// Registrar servicios
builder.Services.AddControllers(); // Agrega servicios al controlador
builder.Services.AddSingleton<JwtService>(); // Registra JwtService
builder.Services.AddSingleton<UserService>(); // Indica el uso de una sola instancia de UserService durante la vida de la aplicación
builder.Services.AddHttpClient<GitHubService>(); // Registra GitHubService para solicitudes HTTP
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<GitHubService>(); // Servicio para llamadas a GitHub
builder.Services.AddScoped<PullRequestService>(); // Servicio para Pull Requests
builder.Services.AddScoped<CommitsService>(); // Servicio para Commits
builder.Services.AddScoped<IssuesService>(); // Servicio para Issues



// Agregar ApplicationDbContext al contenedor de servicios
var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? throw new InvalidOperationException("La cadena de conexión no está definida en el archivo .env.");

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Agrega Swagger al contenedor de servicios
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WorkPulse API v1"));
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();