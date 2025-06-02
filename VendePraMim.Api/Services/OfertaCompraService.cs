using VendePraMim.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace VendePraMim.Api.Services
{
    /// <summary>
    /// Serviço de regras de negócio para Ofertas de Compra.
    /// </summary>
    public class OfertaCompraService
    {
        private readonly VendePraMimContext _context;
        /// <summary>
        /// Construtor do serviço de ofertas de compra.
        /// </summary>
        public OfertaCompraService(VendePraMimContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista ofertas de compra com filtros opcionais e paginação.
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="eventoId">Filtra por ID do evento</param>
        /// <param name="compradorId">Filtra por ID do comprador</param>
        /// <param name="ativa">Filtra por status ativa</param>
        /// <param name="tipoIngresso">Filtra por tipo de ingresso</param>
        /// <returns>Tuple com total de registros e lista paginada</returns>
        public async Task<(int total, List<OfertaCompra> data)> ListarAsync(int page, int pageSize, int? eventoId = null, int? compradorId = null, bool? ativa = null, string? tipoIngresso = null)
        {
            var query = _context.OfertasCompra.AsQueryable();
            if (eventoId.HasValue)
                query = query.Where(o => o.EventoId == eventoId.Value);
            if (compradorId.HasValue)
                query = query.Where(o => o.CompradorId == compradorId.Value);
            if (ativa.HasValue)
                query = query.Where(o => o.Ativa == ativa.Value);
            if (!string.IsNullOrWhiteSpace(tipoIngresso))
                query = query.Where(o => o.TipoIngresso == tipoIngresso);
            var total = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (total, data);
        }

        /// <summary>
        /// Busca uma oferta de compra pelo ID.
        /// </summary>
        public async Task<OfertaCompra?> ObterPorIdAsync(int id)
        {
            return await _context.OfertasCompra.FindAsync(id);
        }

        /// <summary>
        /// Cria uma nova oferta de compra.
        /// </summary>
        public async Task<OfertaCompra> CriarAsync(OfertaCompra oferta)
        {
            _context.OfertasCompra.Add(oferta);
            await _context.SaveChangesAsync();
            return oferta;
        }

        /// <summary>
        /// Atualiza uma oferta de compra existente.
        /// </summary>
        public async Task<bool> AtualizarAsync(OfertaCompra oferta)
        {
            _context.Entry(oferta).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.OfertasCompra.AnyAsync(e => e.Id == oferta.Id))
                    return false;
                throw;
            }
        }

        /// <summary>
        /// Remove uma oferta de compra pelo ID.
        /// </summary>
        public async Task<bool> RemoverAsync(int id)
        {
            var oferta = await _context.OfertasCompra.FindAsync(id);
            if (oferta == null) return false;
            _context.OfertasCompra.Remove(oferta);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
