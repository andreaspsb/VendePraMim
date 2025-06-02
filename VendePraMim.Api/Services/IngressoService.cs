using Microsoft.EntityFrameworkCore;
using VendePraMim.Api.Models;

namespace VendePraMim.Api.Services
{
    /// <summary>
    /// Serviço de regras de negócio para Ingressos.
    /// </summary>
    public class IngressoService
    {
        private readonly VendePraMimContext _db;
        /// <summary>
        /// Construtor do serviço de ingressos.
        /// </summary>
        public IngressoService(VendePraMimContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Lista ingressos com filtros opcionais e paginação.
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="eventoId">Filtra por ID do evento</param>
        /// <param name="vendedorId">Filtra por ID do vendedor</param>
        /// <param name="tipo">Filtra por tipo de ingresso</param>
        /// <param name="vendido">Filtra por status vendido</param>
        /// <returns>Tuple com total de registros e lista paginada</returns>
        public async Task<(int total, List<Ingresso> data)> ListarAsync(int page, int pageSize, int? eventoId = null, int? vendedorId = null, string? tipo = null, bool? vendido = null)
        {
            var query = _db.Ingressos.Include(i => i.Evento).Include(i => i.Vendedor).AsQueryable();
            if (eventoId.HasValue)
                query = query.Where(i => i.EventoId == eventoId.Value);
            if (vendedorId.HasValue)
                query = query.Where(i => i.VendedorId == vendedorId.Value);
            if (!string.IsNullOrWhiteSpace(tipo))
                query = query.Where(i => i.Tipo == tipo);
            if (vendido.HasValue)
                query = query.Where(i => i.Vendido == vendido.Value);
            var total = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (total, data);
        }

        /// <summary>
        /// Busca um ingresso pelo ID.
        /// </summary>
        public async Task<Ingresso?> ObterPorIdAsync(int id)
        {
            return await _db.Ingressos.Include(i => i.Evento).Include(i => i.Vendedor).FirstOrDefaultAsync(i => i.Id == id);
        }

        /// <summary>
        /// Cria um novo ingresso.
        /// </summary>
        public async Task<Ingresso> CriarAsync(Ingresso ingresso)
        {
            _db.Ingressos.Add(ingresso);
            await _db.SaveChangesAsync();
            return ingresso;
        }

        /// <summary>
        /// Atualiza um ingresso existente.
        /// </summary>
        public async Task<bool> AtualizarAsync(int id, Ingresso ingressoAtualizado)
        {
            var ingresso = await _db.Ingressos.FindAsync(id);
            if (ingresso is null) return false;
            ingresso.Tipo = ingressoAtualizado.Tipo;
            ingresso.Setor = ingressoAtualizado.Setor;
            ingresso.PrecoVenda = ingressoAtualizado.PrecoVenda;
            ingresso.PrecoMinimo = ingressoAtualizado.PrecoMinimo;
            ingresso.PrecoMaximo = ingressoAtualizado.PrecoMaximo;
            ingresso.Vendido = ingressoAtualizado.Vendido;
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Remove um ingresso pelo ID.
        /// </summary>
        public async Task<bool> RemoverAsync(int id)
        {
            var ingresso = await _db.Ingressos.FindAsync(id);
            if (ingresso is null) return false;
            _db.Ingressos.Remove(ingresso);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
