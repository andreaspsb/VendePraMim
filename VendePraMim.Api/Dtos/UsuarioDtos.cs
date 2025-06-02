using System.ComponentModel.DataAnnotations;

namespace VendePraMim.Api.Dtos
{
    public class UsuarioRequestDto
    {
        [Required]
        [StringLength(100)]
        public string NomeCompleto { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string Cpf { get; set; } = string.Empty;
        [Required]
        public DateTime DataNascimento { get; set; }
        [Required]
        public string Cidade { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [StringLength(20)]
        public string Tipo { get; set; } = string.Empty;
    }
    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string Cidade { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}
