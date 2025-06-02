using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using VendePraMim.Api.Models;
using Microsoft.Extensions.Logging;
using VendePraMim.Api.Services;
using VendePraMim.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _usuarioService;
    private readonly ILogger<UsuariosController> _logger;
    public UsuariosController(UsuarioService usuarioService, ILogger<UsuariosController> logger)
    {
        _usuarioService = usuarioService;
        _logger = logger;
    }

    /// <summary>
    /// Lista usuários com filtros opcionais e paginação.
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="nome">Filtra por nome</param>
    /// <param name="email">Filtra por email</param>
    /// <param name="tipo">Filtra por tipo de usuário</param>
    /// <param name="cidade">Filtra por cidade</param>
    /// <returns>Lista paginada de usuários</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? nome = null,
        [FromQuery] string? email = null,
        [FromQuery] string? tipo = null,
        [FromQuery] string? cidade = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        (int total, List<Usuario> usuarios) = await _usuarioService.ListarAsync(page, pageSize, nome, email, tipo, cidade);
        var result = new PagedResult<Usuario>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Data = usuarios
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var usuario = await _usuarioService.ObterPorIdAsync(id);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post(Usuario usuario)
    {
        _logger.LogInformation("Tentativa de criar usuário: {Email}", usuario.Email);
        if (!MiniValidator.TryValidate(usuario, out var errors))
        {
            _logger.LogWarning("Falha de validação ao criar usuário: {@Errors}", errors);
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        }
        var criado = await _usuarioService.CriarAsync(usuario);
        _logger.LogInformation("Usuário criado com sucesso: {UsuarioId}", criado.Id);
        return Created($"/api/v1/usuarios/{criado.Id}", criado);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Organizador,Comprador,Vendedor")]
    public async Task<IActionResult> Put(int id, Usuario usuarioAtualizado)
    {
        if (!MiniValidator.TryValidate(usuarioAtualizado, out var errors))
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        var atualizado = await _usuarioService.AtualizarAsync(id, usuarioAtualizado);
        if (!atualizado) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _usuarioService.RemoverAsync(id);
        if (!removido) return NotFound();
        _logger.LogInformation("Usuário removido: {UsuarioId}", id);
        return NoContent();
    }
}
