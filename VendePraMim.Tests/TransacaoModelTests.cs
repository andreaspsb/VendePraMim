using VendePraMim.Api.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VendePraMim.Tests
{
    public class TransacaoModelTests
    {
        [Fact]
        public void Transacao_ComDadosInvalidos_DeveFalharValidacao()
        {
            var transacao = new Transacao
            {
                IngressoId = 0,
                CompradorId = 0,
                ValorPago = -1,
                DataCompra = DateTime.MinValue
            };
            var context = new ValidationContext(transacao);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(transacao, context, results, true);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Transacao_ComDadosValidos_DevePassarValidacao()
        {
            var ingresso = new Ingresso
            {
                Tipo = "Inteira",
                Setor = "Pista",
                PrecoVenda = 100,
                EventoId = 1,
                Evento = new Evento
                {
                    Nome = "Show de Rock",
                    Categoria = "Música",
                    Data = new DateTime(2025, 7, 10),
                    Local = "Estádio Municipal"
                },
                VendedorId = 1,
                Vendedor = new Vendedor
                {
                    NomeCompleto = "Maria Vendedora",
                    Cpf = "12345678901",
                    DataNascimento = new DateTime(1985, 5, 5),
                    Cidade = "Rio de Janeiro",
                    Email = "maria@vendedora.com",
                    Tipo = "Vendedor",
                    ContaBancaria = "12345-6"
                }
            };
            var comprador = new Comprador
            {
                NomeCompleto = "João Comprador",
                Cpf = "12345678901",
                DataNascimento = new DateTime(1990, 1, 1),
                Cidade = "São Paulo",
                Email = "joao@email.com",
                Tipo = "Comprador"
            };
            var transacao = new Transacao
            {
                IngressoId = 1,
                Ingresso = ingresso,
                CompradorId = 1,
                Comprador = comprador,
                ValorPago = 150,
                DataCompra = DateTime.Now
            };
            var context = new ValidationContext(transacao);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(transacao, context, results, true);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
