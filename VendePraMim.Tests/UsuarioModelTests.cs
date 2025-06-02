using VendePraMim.Api.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VendePraMim.Tests
{
    public class UsuarioModelTests
    {
        [Fact]
        public void Usuario_ComDadosInvalidos_DeveFalharValidacao()
        {
            var usuario = new Usuario
            {
                NomeCompleto = "",
                Cpf = "123",
                DataNascimento = DateTime.Now,
                Cidade = "",
                Email = "email-invalido",
                Tipo = ""
            };
            var context = new ValidationContext(usuario);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(usuario, context, results, true);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Usuario_ComDadosValidos_DevePassarValidacao()
        {
            var usuario = new Usuario
            {
                NomeCompleto = "João da Silva",
                Cpf = "12345678901",
                DataNascimento = new DateTime(1990, 1, 1),
                Cidade = "São Paulo",
                Email = "joao@email.com",
                Tipo = "Vendedor"
            };
            var context = new ValidationContext(usuario);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(usuario, context, results, true);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
