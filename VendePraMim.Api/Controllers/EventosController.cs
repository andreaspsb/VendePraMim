using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using VendePraMim.Api.Dtos;
using VendePraMim.Api.Models;
using VendePraMim.Api.Services;
using VendePraMim.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class EventosController : ControllerBase
{
    private readonly EventoService _eventoService;
    private readonly ILogger<EventosController> _logger;
    public EventosController(EventoService eventoService, ILogger<EventosController> logger)
    {
        _eventoService = eventoService;
        _logger = logger;
    }

    /// <summary>
    /// Lista eventos com filtros opcionais e paginação.
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="nome">Filtra por nome</param>
    /// <param name="categoria">Filtra por categoria</param>
    /// <param name="data">Filtra por data</param>
    /// <returns>Lista paginada de eventos</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? nome = null,
        [FromQuery] string? categoria = null,
        [FromQuery] DateTime? data = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        (int total, List<Evento> eventos) = await _eventoService.ListarAsync(page, pageSize, nome, categoria, data);
        var result = new PagedResult<EventoResponseDto>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Data = eventos.Select(e => new EventoResponseDto
            {
                Id = e.Id,
                Nome = e.Nome,
                Categoria = e.Categoria,
                Data = e.Data,
                Local = e.Local,
                OrganizadorId = e.OrganizadorId,
                OrganizadorNome = e.Organizador?.NomeCompleto
            })
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var evento = await _eventoService.ObterPorIdAsync(id);
        if (evento is null) return NotFound();
        var dto = new EventoResponseDto
        {
            Id = evento.Id,
            Nome = evento.Nome,
            Categoria = evento.Categoria,
            Data = evento.Data,
            Local = evento.Local,
            OrganizadorId = evento.OrganizadorId,
            OrganizadorNome = evento.Organizador?.NomeCompleto
        };
        return Ok(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Post([FromBody] EventoRequestDto dto)
    {
        _logger.LogInformation("Tentativa de criar evento: {Nome}", dto.Nome);
        if (!MiniValidator.TryValidate(dto, out var errors))
        {
            _logger.LogWarning("Falha de validação ao criar evento: {@Errors}", errors);
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        }
        var evento = new Evento
        {
            Nome = dto.Nome,
            Categoria = dto.Categoria,
            Data = dto.Data,
            Local = dto.Local,
            OrganizadorId = dto.OrganizadorId
        };
        var criado = await _eventoService.CriarAsync(evento);
        var response = new EventoResponseDto
        {
            Id = criado.Id,
            Nome = criado.Nome,
            Categoria = criado.Categoria,
            Data = criado.Data,
            Local = criado.Local,
            OrganizadorId = criado.OrganizadorId,
            OrganizadorNome = criado.Organizador?.NomeCompleto
        };
        _logger.LogInformation("Evento criado com sucesso: {EventoId}", criado.Id);
        return Created($"/api/v1/eventos/{criado.Id}", response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Put(int id, [FromBody] EventoRequestDto dto)
    {
        if (!MiniValidator.TryValidate(dto, out var errors))
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        var eventoAtualizado = new Evento
        {
            Nome = dto.Nome,
            Categoria = dto.Categoria,
            Data = dto.Data,
            Local = dto.Local,
            OrganizadorId = dto.OrganizadorId
        };
        var atualizado = await _eventoService.AtualizarAsync(id, eventoAtualizado);
        if (!atualizado) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Organizador")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _eventoService.RemoverAsync(id);
        if (!removido) return NotFound();
        _logger.LogInformation("Evento removido: {EventoId}", id);
        return NoContent();
    }
}
