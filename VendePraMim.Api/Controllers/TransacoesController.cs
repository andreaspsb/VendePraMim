using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniValidation;
using VendePraMim.Api;
using VendePraMim.Api.Models;
using VendePraMim.Api.Services;

[ApiController]
[Route("api/v1/[controller]")]
public class TransacoesController : ControllerBase
{
    private readonly TransacaoService _transacaoService;
    private readonly ILogger<TransacoesController> _logger;
    public TransacoesController(TransacaoService transacaoService, ILogger<TransacoesController> logger)
    {
        _transacaoService = transacaoService;
        _logger = logger;
    }

    /// <summary>
    /// Lista transações com filtros opcionais e paginação.
    /// </summary>
    /// <param name="page">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Tamanho da página (padrão: 10)</param>
    /// <param name="valorPago">Filtra por valor pago exato</param>
    /// <param name="validado">Filtra por status de validação</param>
    /// <param name="compradorId">Filtra por ID do comprador</param>
    /// <returns>Lista paginada de transações</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] decimal? valorPago = null,
        [FromQuery] bool? validado = null,
        [FromQuery] int? compradorId = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        (int total, List<Transacao> transacoes) = await _transacaoService.ListarAsync(page, pageSize, valorPago, validado, compradorId);
        var result = new PagedResult<Transacao>
        {
            Page = page,
            PageSize = pageSize,
            Total = total,
            Data = transacoes
        };
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var transacao = await _transacaoService.ObterPorIdAsync(id);
        return transacao is null ? NotFound() : Ok(transacao);
    }

    [HttpPost]
    [Authorize(Roles = "Comprador")]
    public async Task<IActionResult> Post(Transacao transacao)
    {
        _logger.LogInformation("Tentativa de criar transação para ingresso {IngressoId} por comprador {CompradorId}", transacao.IngressoId, transacao.CompradorId);
        if (!MiniValidator.TryValidate(transacao, out var errors))
        {
            _logger.LogWarning("Falha de validação ao criar transação: {@Errors}", errors);
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        }
        var criado = await _transacaoService.CriarAsync(transacao);
        _logger.LogInformation("Transação criada com sucesso: {TransacaoId}", criado.Id);
        return Created($"/api/v1/transacoes/{criado.Id}", criado);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Comprador")]
    public async Task<IActionResult> Put(int id, Transacao transacaoAtualizada)
    {
        var transacao = await _transacaoService.ObterPorIdAsync(id);
        if (transacao is null) return NotFound();
        transacao.ValorPago = transacaoAtualizada.ValorPago;
        transacao.Validado = transacaoAtualizada.Validado;
        transacao.TaxaPlataforma = transacaoAtualizada.TaxaPlataforma;
        if (!MiniValidator.TryValidate(transacao, out var errors))
            return BadRequest(new ErrorResponse {
                Error = "Erro de validação",
                Details = errors
            });
        await _transacaoService.AtualizarAsync(transacao);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var removido = await _transacaoService.RemoverAsync(id);
        if (!removido) return NotFound();
        _logger.LogInformation("Transação removida: {TransacaoId}", id);
        return NoContent();
    }
}
