using VendePraMim.Api.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VendePraMim.Tests
{
    public class OrganizadorModelTests
    {
        [Fact]
        public void Organizador_ComDadosInvalidos_DeveFalharValidacao()
        {
            var organizador = new Organizador
            {
                NomeCompleto = "",
                Cpf = "",
                DataNascimento = DateTime.MinValue,
                Cidade = "",
                Email = "",
                Tipo = "",
                Cnpj = "123"
            };
            var context = new ValidationContext(organizador);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(organizador, context, results, true);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Organizador_ComDadosValidos_DevePassarValidacao()
        {
            var organizador = new Organizador
            {
                NomeCompleto = "Empresa Org",
                Cpf = "12345678901",
                DataNascimento = new DateTime(1980, 1, 1),
                Cidade = "Belo Horizonte",
                Email = "org@empresa.com",
                Tipo = "Organizador",
                Cnpj = "12345678000199"
            };
            var context = new ValidationContext(organizador);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(organizador, context, results, true);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
