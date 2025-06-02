namespace VendePraMim.Api
{
    /// <summary>
    /// Representa um resultado paginado de uma consulta.
    /// </summary>
    /// <typeparam name="T">Tipo dos itens retornados na lista de dados.</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Número da página atual (inicia em 1).
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Quantidade de itens por página.
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// Total de itens encontrados na consulta.
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// Lista de itens da página atual.
        /// </summary>
        public IEnumerable<T> Data { get; set; } = [];
    }
}
