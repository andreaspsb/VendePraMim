using VendePraMim.Api.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VendePraMim.Tests
{
    public class VendedorModelTests
    {
        [Fact]
        public void Vendedor_ComDadosInvalidos_DeveFalharValidacao()
        {
            var vendedor = new Vendedor
            {
                NomeCompleto = "",
                Cpf = "",
                DataNascimento = DateTime.MinValue,
                Cidade = "",
                Email = "",
                Tipo = "",
                ContaBancaria = ""
            };
            var context = new ValidationContext(vendedor);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(vendedor, context, results, true);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Vendedor_ComDadosValidos_DevePassarValidacao()
        {
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
            var context = new ValidationContext(vendedor);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(vendedor, context, results, true);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
