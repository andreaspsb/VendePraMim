using Microsoft.EntityFrameworkCore;
using VendePraMim.Api.Models;

namespace VendePraMim.Api.Services
{
    /// <summary>
    /// Serviço de regras de negócio para Usuários.
    /// </summary>
    public class UsuarioService
    {
        private readonly VendePraMimContext _db;
        /// <summary>
        /// Construtor do serviço de usuários.
        /// </summary>
        public UsuarioService(VendePraMimContext db) => _db = db;

        /// <summary>
        /// Lista usuários com filtros opcionais e paginação.
        /// </summary>
        /// <param name="page">Número da página</param>
        /// <param name="pageSize">Tamanho da página</param>
        /// <param name="nome">Filtra por nome</param>
        /// <param name="email">Filtra por email</param>
        /// <param name="tipo">Filtra por tipo de usuário</param>
        /// <param name="cidade">Filtra por cidade</param>
        /// <returns>Tuple com total de registros e lista paginada</returns>
        public async Task<(int total, List<Usuario> data)> ListarAsync(int page, int pageSize, string? nome = null, string? email = null, string? tipo = null, string? cidade = null)
        {
            var query = _db.Usuarios.AsQueryable();
            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(u => EF.Functions.Like(u.NomeCompleto, $"%{nome}%"));
            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(u => EF.Functions.Like(u.Email, $"%{email}%"));
            if (!string.IsNullOrWhiteSpace(tipo))
                query = query.Where(u => u.Tipo == tipo);
            if (!string.IsNullOrWhiteSpace(cidade))
                query = query.Where(u => EF.Functions.Like(u.Cidade, $"%{cidade}%"));
            var total = await query.CountAsync();
            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (total, data);
        }

        /// <summary>
        /// Busca um usuário pelo ID.
        /// </summary>
        public async Task<Usuario?> ObterPorIdAsync(int id)
        {
            return await _db.Usuarios.FindAsync(id);
        }

        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        public async Task<Usuario> CriarAsync(Usuario usuario)
        {
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return usuario;
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        public async Task<bool> AtualizarAsync(int id, Usuario usuarioAtualizado)
        {
            var usuario = await _db.Usuarios.FindAsync(id);
            if (usuario is null) return false;
            usuario.NomeCompleto = usuarioAtualizado.NomeCompleto;
            usuario.Cpf = usuarioAtualizado.Cpf;
            usuario.DataNascimento = usuarioAtualizado.DataNascimento;
            usuario.Cidade = usuarioAtualizado.Cidade;
            usuario.Email = usuarioAtualizado.Email;
            usuario.Tipo = usuarioAtualizado.Tipo;
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Remove um usuário pelo ID.
        /// </summary>
        public async Task<bool> RemoverAsync(int id)
        {
            var usuario = await _db.Usuarios.FindAsync(id);
            if (usuario is null) return false;
            _db.Usuarios.Remove(usuario);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
