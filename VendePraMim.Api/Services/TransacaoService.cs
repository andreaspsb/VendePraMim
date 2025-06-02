using VendePraMim.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace VendePraMim.Api.Services
{
    /// <summary>
    /// Serviço de regras de negócio para Transações.
    /// </summary>
    public class TransacaoService
    {
        private readonly VendePraMimContext _context;
        /// <summary>
        /// Construtor do serviço de transações.
        /// </summary>
        public TransacaoService(VendePraMimContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista transações com filtros opcionais e paginação.
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="valorPago">Filtra por valor pago exato</param>
        /// <param name="validado">Filtra por status de validação</param>
        /// <param name="compradorId">Filtra por ID do comprador</param>
        /// <returns>Tuple com total de registros e lista paginada</returns>
        public async Task<(int total, List<Transacao> data)> ListarAsync(int page, int pageSize, decimal? valorPago = null, bool? validado = null, int? compradorId = null)
        {
            var query = _context.Transacoes.AsQueryable();
            if (valorPago.HasValue)
                query = query.Where(t => t.ValorPago == valorPago.Value);
            if (validado.HasValue)
                query = query.Where(t => t.Validado == validado.Value);
            if (compradorId.HasValue)
                query = query.Where(t => t.CompradorId == compradorId.Value);
            var total = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (total, data);
        }

        /// <summary>
        /// Busca uma transação pelo ID.
        /// </summary>
        public async Task<Transacao?> ObterPorIdAsync(int id)
        {
            return await _context.Transacoes.FindAsync(id);
        }

        /// <summary>
        /// Cria uma nova transação.
        /// </summary>
        public async Task<Transacao> CriarAsync(Transacao transacao)
        {
            _context.Transacoes.Add(transacao);
            await _context.SaveChangesAsync();
            return transacao;
        }

        /// <summary>
        /// Atualiza uma transação existente.
        /// </summary>
        public async Task<bool> AtualizarAsync(Transacao transacao)
        {
            _context.Entry(transacao).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Transacoes.AnyAsync(e => e.Id == transacao.Id))
                    return false;
                throw;
            }
        }

        /// <summary>
        /// Remove uma transação pelo ID.
        /// </summary>
        public async Task<bool> RemoverAsync(int id)
        {
            var transacao = await _context.Transacoes.FindAsync(id);
            if (transacao == null) return false;
            _context.Transacoes.Remove(transacao);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
