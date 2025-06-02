using VendePraMim.Api.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VendePraMim.Tests
{
    public class EventoModelTests
    {
        [Fact]
        public void Evento_ComDadosInvalidos_DeveFalharValidacao()
        {
            var evento = new Evento
            {
                Nome = "",
                Categoria = "",
                Data = DateTime.MinValue,
                Local = ""
            };
            var context = new ValidationContext(evento);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(evento, context, results, true);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void Evento_ComDadosValidos_DevePassarValidacao()
        {
            var evento = new Evento
            {
                Nome = "Show de Rock",
                Categoria = "Música",
                Data = new DateTime(2025, 7, 10),
                Local = "Estádio Municipal"
            };
            var context = new ValidationContext(evento);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(evento, context, results, true);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
