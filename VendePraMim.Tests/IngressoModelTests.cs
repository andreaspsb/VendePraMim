using VendePraMim.Api.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VendePraMim.Tests
{
    public class IngressoModelTests
    {
        [Fact]
        public void Ingresso_ComDadosInvalidos_DeveFalharValidacao()
        {
            var ingresso = new Ingresso
            {
                Tipo = "",
                Setor = "",
                PrecoVenda = -10,
                EventoId = 0,
                VendedorId = 0
            };
            var context = new ValidationContext(ingresso);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(ingresso, context, results, true);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Ingresso_ComDadosValidos_DevePassarValidacao()
        {
            var evento = new Evento
            {
                Nome = "Show de Rock",
                Categoria = "Música",
                Data = new DateTime(2025, 7, 10),
                Local = "Estádio Municipal"
            };
            var vendedor = new Vendedor
            {
                NomeCompleto = "Maria Vendedora",
                Cpf = "12345678901",
                DataNascimento = new DateTime(1985, 5, 5),
                Cidade = "Rio de Janeiro",
                Email = "maria@vendedora.com",
                Tipo = "Vendedor",
                ContaBancaria = "12345-6"
            };
            var ingresso = new Ingresso
            {
                Tipo = "Inteira",
                Setor = "Pista",
                PrecoVenda = 100,
                EventoId = 1,
                Evento = evento,
                VendedorId = 1,
                Vendedor = vendedor
            };
            var context = new ValidationContext(ingresso);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(ingresso, context, results, true);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
