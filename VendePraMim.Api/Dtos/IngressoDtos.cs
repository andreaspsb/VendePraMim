using System.ComponentModel.DataAnnotations;

namespace VendePraMim.Api.Dtos
{
    public class IngressoRequestDto
    {
        [Required]
        public int EventoId { get; set; }
        [Required]
        [StringLength(30)]
        public string Tipo { get; set; } = string.Empty;
        [Required]
        [StringLength(30)]
        public string Setor { get; set; } = string.Empty;
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PrecoVenda { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? PrecoMinimo { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? PrecoMaximo { get; set; }
        [Required]
        public int VendedorId { get; set; }
    }
    public class IngressoResponseDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public string EventoNome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Setor { get; set; } = string.Empty;
        public decimal PrecoVenda { get; set; }
        public decimal? PrecoMinimo { get; set; }
        public decimal? PrecoMaximo { get; set; }
        public int VendedorId { get; set; }
        public string VendedorNome { get; set; } = string.Empty;
        public bool Vendido { get; set; }
    }
}
