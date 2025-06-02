using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VendePraMim.Api.Models;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly VendePraMimContext _db;
    private readonly IConfiguration _config;
    public AuthController(VendePraMimContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Busca usuário no banco
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Username);
        if (usuario == null || request.Password != "senha123") // Exemplo: senha fixa
            return Unauthorized(new ErrorResponse { Error = "Usuário ou senha inválidos" });

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario.NomeCompleto),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Tipo ?? "") // <- role padrão
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );
        return Ok(new {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}

public class LoginRequest
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
