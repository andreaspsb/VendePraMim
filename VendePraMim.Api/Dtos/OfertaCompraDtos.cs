using System.ComponentModel.DataAnnotations;

namespace VendePraMim.Api.Dtos
{
    public class OfertaCompraRequestDto
    {
        [Required]
        public int EventoId { get; set; }
        [Required]
        [StringLength(30)]
        public string TipoIngresso { get; set; } = string.Empty;
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PrecoMaximo { get; set; }
        [Required]
        public int CompradorId { get; set; }
    }
    public class OfertaCompraResponseDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public string EventoNome { get; set; } = string.Empty;
        public string TipoIngresso { get; set; } = string.Empty;
        public decimal PrecoMaximo { get; set; }
        public int CompradorId { get; set; }
        public string CompradorNome { get; set; } = string.Empty;
        public DateTime DataOferta { get; set; }
        public bool Ativa { get; set; }
    }
}
