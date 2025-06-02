using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniValidation;
using VendePraMim.Api;
using VendePraMim.Api.Dtos;
using VendePraMim.Api.Models;
using VendePraMim.Api.Services;

[ApiController]
[Route("api/v1/[controller]")]
public class IngressosController : ControllerBase
{
    private readonly VendePraMimContext _db;
    private readonly ILogger<IngressosController> _logger;
    private readonly IngressoService _service;
    public IngressosController(VendePraMimContext db, ILogger<IngressosController> logger, IngressoService service)
    {
        _db = db;
        _logger = logger;
        _service = service;
    }

    /// <summary>
    /// Lista ingressos com filtros opcionais e paginação.
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="eventoId">Filtra por ID do evento</param>
    /// <param name="vendedorId">Filtra por ID do vendedor</param>
    /// <param name="tipo">Filtra por tipo de ingresso</param>
    /// <param name="vendido">Filtra por status vendido</param>
    /// <returns>Lista paginada de ingressos</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? eventoId = null,
        [FromQuery] int? vendedorId = null,
        [FromQuery] string? tipo = null,
        [FromQuery] bool? vendido = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        (int total, List<Ingresso> ingressos) = await _service.ListarAsync(page, pageSize, eventoId, vendedorId, tipo, vendido);
        var result = new PagedResult<IngressoResponseDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Data = ingressos.Select(i => new IngressoResponseDto
            {
                Id = i.Id,
                EventoId = i.EventoId,
                EventoNome = i.Evento?.Nome ?? string.Empty,
                Tipo = i.Tipo,
                Setor = i.Setor,
                PrecoVenda = i.PrecoVenda,
                PrecoMinimo = i.PrecoMinimo,
                PrecoMaximo = i.PrecoMaximo,
                VendedorId = i.VendedorId,
                VendedorNome = i.Vendedor?.NomeCompleto ?? string.Empty,
                Vendido = i.Vendido
            })
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var ingresso = await _service.ObterPorIdAsync(id);
        if (ingresso is null) return NotFound();
        var dto = new IngressoResponseDto
        {
            Id = ingresso.Id,
            EventoId = ingresso.EventoId,
            EventoNome = ingresso.Evento?.Nome ?? string.Empty,
            Tipo = ingresso.Tipo,
            Setor = ingresso.Setor,
            PrecoVenda = ingresso.PrecoVenda,
            PrecoMinimo = ingresso.PrecoMinimo,
            PrecoMaximo = ingresso.PrecoMaximo,
            VendedorId = ingresso.VendedorId,
            VendedorNome = ingresso.Vendedor?.NomeCompleto ?? string.Empty,
            Vendido = ingresso.Vendido
        };
        return Ok(dto);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Post([FromBody] IngressoRequestDto dto)
    {
        _logger.LogInformation("Tentativa de criar ingresso para evento {EventoId} por vendedor {VendedorId}", dto.EventoId, dto.VendedorId);
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            _logger.LogWarning("Falha de validação ao criar ingresso: {@Errors}", errors);
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        }
        var ingresso = new Ingresso
        {
            EventoId = dto.EventoId,
            Tipo = dto.Tipo,
            Setor = dto.Setor,
            PrecoVenda = dto.PrecoVenda,
            PrecoMinimo = dto.PrecoMinimo,
            PrecoMaximo = dto.PrecoMaximo,
            VendedorId = dto.VendedorId
            // Evento e Vendedor serão resolvidos pelo EF Core
        };
        var criado = await _service.CriarAsync(ingresso);
        var response = new IngressoResponseDto
        {
            Id = criado.Id,
            EventoId = criado.EventoId,
            EventoNome = criado.Evento?.Nome ?? string.Empty,
            Tipo = criado.Tipo,
            Setor = criado.Setor,
            PrecoVenda = criado.PrecoVenda,
            PrecoMinimo = criado.PrecoMinimo,
            PrecoMaximo = criado.PrecoMaximo,
            VendedorId = criado.VendedorId,
            VendedorNome = criado.Vendedor?.NomeCompleto ?? string.Empty,
            Vendido = criado.Vendido
        };
        _logger.LogInformation("Ingresso criado com sucesso: {IngressoId}", criado.Id);
        return Created($"/api/v1/ingressos/{criado.Id}", response);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Put(int id, [FromBody] IngressoRequestDto dto)
    {
        var ingressoAtualizado = new Ingresso
        {
            EventoId = dto.EventoId,
            Tipo = dto.Tipo,
            Setor = dto.Setor,
            PrecoVenda = dto.PrecoVenda,
            PrecoMinimo = dto.PrecoMinimo,
            PrecoMaximo = dto.PrecoMaximo,
            VendedorId = dto.VendedorId
            // Evento e Vendedor serão resolvidos pelo EF Core
        };
        var ok = await _service.AtualizarAsync(id, ingressoAtualizado);
        if (!ok) return NotFound();
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            _logger.LogWarning("Falha de validação ao atualizar ingresso: {@Errors}", errors);
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await _service.RemoverAsync(id);
        if (!ok)
        {
            _logger.LogWarning("Tentativa de deletar ingresso inexistente: {IngressoId}", id);
            return NotFound();
        }
        _logger.LogInformation("Ingresso deletado: {IngressoId}", id);
        return NoContent();
    }
}
