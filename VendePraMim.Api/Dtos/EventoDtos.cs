using System.ComponentModel.DataAnnotations;

namespace VendePraMim.Api.Dtos
{
    /// <summary>
    /// DTO para requisições de criação/atualização de evento.
    /// </summary>
    public class EventoRequestDto
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        public DateTime Data { get; set; }

        [Required]
        [StringLength(150)]
        public string Local { get; set; } = string.Empty;

        public int? OrganizadorId { get; set; }
    }

    /// <summary>
    /// DTO para resposta de dados de evento.
    /// </summary>
    public class EventoResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public DateTime Data { get; set; }
        public string Local { get; set; } = string.Empty;
        public int? OrganizadorId { get; set; }
        public string? OrganizadorNome { get; set; }
    }
}
