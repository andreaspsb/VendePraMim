using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using VendePraMim.Api.Models;
using VendePraMim.Api.Services;
using VendePraMim.Api;

[ApiController]
[Route("api/v1/[controller]")]
public class OfertasCompraController : ControllerBase
{
    private readonly OfertaCompraService _ofertaCompraService;
    private readonly ILogger<OfertasCompraController> _logger;
    public OfertasCompraController(OfertaCompraService ofertaCompraService, ILogger<OfertasCompraController> logger)
    {
        _ofertaCompraService = ofertaCompraService;
        _logger = logger;
    }

    /// <summary>
    /// Lista ofertas de compra com filtros opcionais e paginação.
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="eventoId">Filtra por ID do evento</param>
    /// <param name="compradorId">Filtra por ID do comprador</param>
    /// <param name="ativa">Filtra por status ativa</param>
    /// <param name="tipoIngresso">Filtra por tipo de ingresso</param>
    /// <returns>Lista paginada de ofertas de compra</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? eventoId = null,
        [FromQuery] int? compradorId = null,
        [FromQuery] bool? ativa = null,
        [FromQuery] string? tipoIngresso = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        (int total, List<OfertaCompra> ofertas) = await _ofertaCompraService.ListarAsync(page, pageSize, eventoId, compradorId, ativa, tipoIngresso);
        var result = new PagedResult<OfertaCompra>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Data = ofertas
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var oferta = await _ofertaCompraService.ObterPorIdAsync(id);
        return oferta is null ? NotFound() : Ok(oferta);
    }

    [HttpPost]
    [Authorize(Roles = "Comprador")]
    public async Task<IActionResult> Post(OfertaCompra oferta)
    {
        _logger.LogInformation("Tentativa de criar oferta de compra para evento {EventoId} por comprador {CompradorId}", oferta.EventoId, oferta.CompradorId);
        if (!MiniValidator.TryValidate(oferta, out var errors))
        {
            _logger.LogWarning("Falha de validação ao criar oferta de compra: {@Errors}", errors);
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        }
        var criado = await _ofertaCompraService.CriarAsync(oferta);
        _logger.LogInformation("Oferta de compra criada com sucesso: {OfertaId}", criado.Id);
        return Created($"/api/v1/ofertascompra/{criado.Id}", criado);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Comprador")]
    public async Task<IActionResult> Put(int id, OfertaCompra ofertaAtualizada)
    {
        var oferta = await _ofertaCompraService.ObterPorIdAsync(id);
        if (oferta is null) return NotFound();
        oferta.TipoIngresso = ofertaAtualizada.TipoIngresso;
        oferta.PrecoMaximo = ofertaAtualizada.PrecoMaximo;
        oferta.Ativa = ofertaAtualizada.Ativa;
        if (!MiniValidator.TryValidate(oferta, out var errors))
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        await _ofertaCompraService.AtualizarAsync(oferta);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Comprador")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _ofertaCompraService.RemoverAsync(id);
        if (!removido) return NotFound();
        _logger.LogInformation("Oferta de compra removida: {OfertaId}", id);
        return NoContent();
    }
}
