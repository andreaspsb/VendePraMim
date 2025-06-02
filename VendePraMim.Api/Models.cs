using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace VendePraMim.Api.Models
{
    /// <summary>
    /// Contexto do banco de dados principal da aplicação VendePraMim.
    /// </summary>
    public class VendePraMimContext : DbContext
    {
        public VendePraMimContext(DbContextOptions<VendePraMimContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Vendedor> Vendedores { get; set; }
        public DbSet<Comprador> Compradores { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Ingresso> Ingressos { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<OfertaCompra> OfertasCompra { get; set; }
        public DbSet<Organizador> Organizadores { get; set; }
    }

    /// <summary>
    /// Representa um usuário do sistema.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único do usuário.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        public required string NomeCompleto { get; set; }
        /// <summary>
        /// CPF do usuário (11 dígitos).
        /// </summary>
        [Required]
        [StringLength(20)]
        [RegularExpression(@"\d{11}", ErrorMessage = "CPF deve conter 11 dígitos numéricos.")]
        public required string Cpf { get; set; }
        /// <summary>
        /// Data de nascimento do usuário.
        /// </summary>
        [Required]
        public DateTime DataNascimento { get; set; }
        /// <summary>
        /// Cidade de residência do usuário.
        /// </summary>
        public required string Cidade { get; set; }
        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        /// <summary>
        /// Tipo do usuário (Vendedor, Comprador, Organizador).
        /// </summary>
        [Required]
        [StringLength(20)]
        public required string Tipo { get; set; } // Vendedor, Comprador, Organizador
    }

    /// <summary>
    /// Representa um vendedor, herda de Usuário.
    /// </summary>
    public class Vendedor : Usuario
    {
        /// <summary>
        /// Conta bancária do vendedor.
        /// </summary>
        [Required]
        public required string ContaBancaria { get; set; }

        /// <summary>
        /// Ingressos cadastrados pelo vendedor.
        /// </summary>
        public ICollection<Ingresso> Ingressos { get; set; } = new List<Ingresso>();
    }

    /// <summary>
    /// Representa um comprador, herda de Usuário.
    /// </summary>
    public class Comprador : Usuario
    {
        /// <summary>
        /// Transações realizadas pelo comprador.
        /// </summary>
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }

    /// <summary>
    /// Representa um organizador de eventos, herda de Usuário.
    /// </summary>
    public class Organizador : Usuario
    {
        /// <summary>
        /// CNPJ do organizador (14 dígitos).
        /// </summary>
        [Required]
        [StringLength(20)]
        [RegularExpression(@"\d{14}", ErrorMessage = "CNPJ deve conter 14 dígitos numéricos.")]
        public string? Cnpj { get; set; }

        /// <summary>
        /// Eventos organizados.
        /// </summary>
        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }

    /// <summary>
    /// Representa um evento disponível na plataforma.
    /// </summary>
    public class Evento
    {
        /// <summary>
        /// Identificador único do evento.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do evento.
        /// </summary>
        [Required]
        [StringLength(100)]
        public required string Nome { get; set; }

        /// <summary>
        /// Categoria do evento.
        /// </summary>
        [Required]
        [StringLength(50)]
        public required string Categoria { get; set; }

        /// <summary>
        /// Data e hora do evento.
        /// </summary>
        [Required]
        public DateTime Data { get; set; }

        /// <summary>
        /// Local do evento.
        /// </summary>
        [Required]
        [StringLength(150)]
        public required string Local { get; set; }

        /// <summary>
        /// Identificador do organizador do evento.
        /// </summary>
        public int? OrganizadorId { get; set; }

        /// <summary>
        /// Organizador do evento.
        /// </summary>
        public Organizador? Organizador { get; set; }

        /// <summary>
        /// Ingressos disponíveis para o evento.
        /// </summary>
        public ICollection<Ingresso> Ingressos { get; set; } = new List<Ingresso>();
    }

    /// <summary>
    /// Representa um ingresso de evento.
    /// </summary>
    public class Ingresso
    {
        /// <summary>
        /// Identificador único do ingresso.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do evento relacionado.
        /// </summary>
        [Required]
        public int EventoId { get; set; }

        /// <summary>
        /// Evento relacionado ao ingresso.
        /// </summary>
        public Evento? Evento { get; set; }

        /// <summary>
        /// Tipo do ingresso (ex: inteira, meia).
        /// </summary>
        [Required]
        [StringLength(30)]
        public required string Tipo { get; set; }

        /// <summary>
        /// Setor do ingresso (ex: pista, camarote).
        /// </summary>
        [Required]
        [StringLength(30)]
        public required string Setor { get; set; }

        /// <summary>
        /// Preço de venda do ingresso.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PrecoVenda { get; set; }

        /// <summary>
        /// Preço mínimo aceito para o ingresso (opcional).
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? PrecoMinimo { get; set; }

        /// <summary>
        /// Preço máximo aceito para o ingresso (opcional).
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal? PrecoMaximo { get; set; }

        /// <summary>
        /// Identificador do vendedor do ingresso.
        /// </summary>
        [Required]
        public int VendedorId { get; set; }

        /// <summary>
        /// Vendedor do ingresso.
        /// </summary>
        public Vendedor? Vendedor { get; set; }

        /// <summary>
        /// Indica se o ingresso já foi vendido.
        /// </summary>
        public bool Vendido { get; set; }

        /// <summary>
        /// Transações relacionadas ao ingresso.
        /// </summary>
        public ICollection<Transacao> Transacoes { get; set; } = new List<Transacao>();
    }

    /// <summary>
    /// Representa uma transação de compra de ingresso.
    /// </summary>
    public class Transacao
    {
        /// <summary>
        /// Identificador único da transação.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do ingresso comprado.
        /// </summary>
        [Required]
        public int IngressoId { get; set; }

        /// <summary>
        /// Ingresso comprado.
        /// </summary>
        public required Ingresso Ingresso { get; set; }

        /// <summary>
        /// Identificador do comprador.
        /// </summary>
        [Required]
        public int CompradorId { get; set; }

        /// <summary>
        /// Comprador da transação.
        /// </summary>
        public required Comprador Comprador { get; set; }

        /// <summary>
        /// Data e hora da compra.
        /// </summary>
        [Required]
        public DateTime DataCompra { get; set; }

        /// <summary>
        /// Valor pago na transação.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal ValorPago { get; set; }

        /// <summary>
        /// Indica se a transação foi validada.
        /// </summary>
        public bool Validado { get; set; }

        /// <summary>
        /// Valor da taxa da plataforma.
        /// </summary>
        [Range(0, double.MaxValue)]
        public decimal TaxaPlataforma { get; set; }
    }

    /// <summary>
    /// Representa uma oferta de compra de ingresso.
    /// </summary>
    public class OfertaCompra
    {
        /// <summary>
        /// Identificador único da oferta.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do evento desejado.
        /// </summary>
        [Required]
        public int EventoId { get; set; }

        /// <summary>
        /// Evento desejado na oferta.
        /// </summary>
        public required Evento Evento { get; set; }

        /// <summary>
        /// Tipo de ingresso desejado.
        /// </summary>
        [Required]
        [StringLength(30)]
        public required string TipoIngresso { get; set; }

        /// <summary>
        /// Preço máximo que o comprador está disposto a pagar.
        /// </summary>
        [Required]
        [Range(0, double.MaxValue)]
        public decimal PrecoMaximo { get; set; }

        /// <summary>
        /// Identificador do comprador.
        /// </summary>
        [Required]
        public int CompradorId { get; set; }

        /// <summary>
        /// Comprador que fez a oferta.
        /// </summary>
        public required Comprador Comprador { get; set; }

        /// <summary>
        /// Data e hora da oferta.
        /// </summary>
        [Required]
        public DateTime DataOferta { get; set; }

        /// <summary>
        /// Indica se a oferta está ativa.
        /// </summary>
        public bool Ativa { get; set; }
    }
}
