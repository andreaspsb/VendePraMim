using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VendePraMim.Api.Models;
using VendePraMim.Api.Services;
using Xunit;
using System.Linq;

namespace VendePraMim.Tests
{
    public class EventoServiceIntegrationTests : IDisposable
    {
        private readonly VendePraMimContext _context;
        private readonly EventoService _service;

        public EventoServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<VendePraMimContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new VendePraMimContext(options);
            _service = new EventoService(_context);
        }

        [Fact]
        public async Task Criar_Listar_Obter_Atualizar_Remover_Evento_FluxoCompleto()
        {
            // Arrange
            var evento = new Evento
            {
                Nome = "Show de Teste",
                Categoria = "Música",
                Data = DateTime.UtcNow.AddDays(10),
                Local = "Teatro Central",
                OrganizadorId = 1
            };

            // Act - Criar
            var criado = await _service.CriarAsync(evento);
            Assert.NotNull(criado);
            Assert.True(criado.Id > 0);

            // Act - Listar
            var (total, lista) = await _service.ListarAsync(1, 10);
            Assert.Equal(1, total);
            Assert.Single(lista);
            Assert.Equal("Show de Teste", lista[0].Nome);

            // Act - Obter
            var obtido = await _service.ObterPorIdAsync(criado.Id);
            Assert.NotNull(obtido);
            Assert.Equal("Show de Teste", obtido!.Nome);

            // Act - Atualizar
            var atualizado = new Evento
            {
                Nome = "Show Atualizado",
                Categoria = "Teatro",
                Data = DateTime.UtcNow.AddDays(20),
                Local = "Arena Nova",
                OrganizadorId = 2
            };
            var ok = await _service.AtualizarAsync(criado.Id, atualizado);
            Assert.True(ok);
            var atualizadoDb = await _service.ObterPorIdAsync(criado.Id);
            Assert.Equal("Show Atualizado", atualizadoDb!.Nome);
            Assert.Equal("Teatro", atualizadoDb.Categoria);
            Assert.Equal("Arena Nova", atualizadoDb.Local);
            Assert.Equal(2, atualizadoDb.OrganizadorId);

            // Act - Remover
            var removido = await _service.RemoverAsync(criado.Id);
            Assert.True(removido);
            var inexistente = await _service.ObterPorIdAsync(criado.Id);
            Assert.Null(inexistente);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
