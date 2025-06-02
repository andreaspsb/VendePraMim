using Microsoft.EntityFrameworkCore;
using VendePraMim.Api.Models;

namespace VendePraMim.Api.Services
{
    /// <summary>
    /// Serviço de regras de negócio para Eventos.
    /// </summary>
    public class EventoService
    {
        private readonly VendePraMimContext _db;
        /// <summary>
        /// Construtor do serviço de eventos.
        /// </summary>
        /// <param name="db">Contexto do banco de dados</param>
        public EventoService(VendePraMimContext db) => _db = db;

        /// <summary>
        /// Lista eventos com filtros opcionais e paginação.
        /// </summary>
        /// <param name="page">Número da página (inicia em 1).</param>
        /// <param name="pageSize">Quantidade de itens por página.</param>
        /// <param name="nome">(Opcional) Filtro pelo nome do evento (contém).</param>
        /// <param name="categoria">(Opcional) Filtro pela categoria do evento (igualdade).</param>
        /// <param name="data">(Opcional) Filtro pela data do evento (apenas data, ignora hora).</param>
        /// <returns>Uma tupla contendo o total de registros encontrados e a lista paginada de eventos.</returns>
        public async Task<(int total, List<Evento> data)> ListarAsync(int page, int pageSize, string? nome = null, string? categoria = null, DateTime? data = null)
        {
            var query = _db.Eventos.Include(e => e.Organizador).AsQueryable();
            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(e => EF.Functions.Like(e.Nome, $"%{nome}%"));
            if (!string.IsNullOrWhiteSpace(categoria))
                query = query.Where(e => e.Categoria == categoria);
            if (data.HasValue)
                query = query.Where(e => e.Data.Date == data.Value.Date);
            var total = await query.CountAsync();
            var dataList = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (total, dataList);
        }

        /// <summary>
        /// Busca um evento pelo seu identificador único.
        /// </summary>
        /// <param name="id">Identificador do evento.</param>
        /// <returns>O evento encontrado ou null se não existir.</returns>
        public async Task<Evento?> ObterPorIdAsync(int id)
        {
            return await _db.Eventos.Include(e => e.Organizador).FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Cria um novo evento no sistema.
        /// </summary>
        /// <param name="evento">Objeto do tipo Evento a ser persistido.</param>
        /// <returns>O evento criado, incluindo o ID gerado.</returns>
        public async Task<Evento> CriarAsync(Evento evento)
        {
            _db.Eventos.Add(evento);
            await _db.SaveChangesAsync();
            return evento;
        }

        /// <summary>
        /// Atualiza os dados de um evento existente.
        /// </summary>
        /// <param name="id">Identificador do evento a ser atualizado.</param>
        /// <param name="eventoAtualizado">Objeto com os novos dados do evento.</param>
        /// <returns>True se o evento foi atualizado com sucesso, false se não encontrado.</returns>
        public async Task<bool> AtualizarAsync(int id, Evento eventoAtualizado)
        {
            var evento = await _db.Eventos.FindAsync(id);
            if (evento is null) return false;
            evento.Nome = eventoAtualizado.Nome;
            evento.Categoria = eventoAtualizado.Categoria;
            evento.Data = eventoAtualizado.Data;
            evento.Local = eventoAtualizado.Local;
            evento.OrganizadorId = eventoAtualizado.OrganizadorId;
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Remove um evento do sistema pelo seu identificador.
        /// </summary>
        /// <param name="id">Identificador do evento a ser removido.</param>
        /// <returns>True se o evento foi removido, false se não encontrado.</returns>
        public async Task<bool> RemoverAsync(int id)
        {
            var evento = await _db.Eventos.FindAsync(id);
            if (evento is null) return false;
            _db.Eventos.Remove(evento);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
