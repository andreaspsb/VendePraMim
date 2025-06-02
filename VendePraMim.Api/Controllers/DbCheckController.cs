using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using VendePraMim.Api.Models;

[ApiController]
[Route("api/v1/[controller]")]
public class DbCheckController : ControllerBase
{
    private readonly VendePraMimContext _db;
    public DbCheckController(VendePraMimContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        await _db.Database.EnsureCreatedAsync();
        return Ok("Banco de dados pronto!");
    }
}
