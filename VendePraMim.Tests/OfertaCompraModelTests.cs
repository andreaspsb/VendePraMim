using VendePraMim.Api.Models;
using Xunit;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VendePraMim.Tests
{
    public class OfertaCompraModelTests
    {
        [Fact]
        public void OfertaCompra_ComDadosInvalidos_DeveFalharValidacao()
        {
            var oferta = new OfertaCompra
            {
                TipoIngresso = "",
                PrecoMaximo = -1,
                EventoId = 0,
                CompradorId = 0
            };
            var context = new ValidationContext(oferta);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(oferta, context, results, true);
            Assert.False(isValid);
            Assert.NotEmpty(results);
        }

        [Fact]
        public void OfertaCompra_ComDadosValidos_DevePassarValidacao()
        {
            var evento = new Evento
            {
                Nome = "Show de Rock",
                Categoria = "Música",
                Data = new DateTime(2025, 7, 10),
                Local = "Estádio Municipal"
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
            var oferta = new OfertaCompra
            {
                TipoIngresso = "Meia",
                PrecoMaximo = 200,
                EventoId = 1,
                Evento = evento,
                CompradorId = 1,
                Comprador = comprador,
                DataOferta = DateTime.Now
            };
            var context = new ValidationContext(oferta);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(oferta, context, results, true);
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}
