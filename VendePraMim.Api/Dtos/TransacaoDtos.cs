using System.ComponentModel.DataAnnotations;

namespace VendePraMim.Api.Dtos
{
    public class TransacaoRequestDto
    {
        [Required]
        public int IngressoId { get; set; }
        [Required]
        public int CompradorId { get; set; }
        [Required]
        public DateTime DataCompra { get; set; }
        [Required]
        [Range(0, double.MaxValue)]
        public decimal ValorPago { get; set; }
        public bool Validado { get; set; }
        [Range(0, double.MaxValue)]
        public decimal TaxaPlataforma { get; set; }
    }
    public class TransacaoResponseDto
    {
        public int Id { get; set; }
        public int IngressoId { get; set; }
        public string IngressoTipo { get; set; } = string.Empty;
        public int CompradorId { get; set; }
        public string CompradorNome { get; set; } = string.Empty;
        public DateTime DataCompra { get; set; }
        public decimal ValorPago { get; set; }
        public bool Validado { get; set; }
        public decimal TaxaPlataforma { get; set; }
    }
}
