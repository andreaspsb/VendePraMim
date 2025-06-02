using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace VendePraMim.Blazor.Services;

public class OfertaCompraService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    public OfertaCompraService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
    }

    private async Task AddJwtAsync()
    {
        var token = await _js.InvokeAsync<string>("localStorage.getItem", "jwt");
        if (!string.IsNullOrWhiteSpace(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<List<OfertaCompraDto>> ListarAsync()
    {
        var result = await _http.GetFromJsonAsync<PagedResult<OfertaCompraDto>>("api/v1/ofertascompra?page=1&pageSize=50");
        return result?.Data ?? new();
    }

    public async Task<OfertaCompraDto?> ObterAsync(int id)
    {
        return await _http.GetFromJsonAsync<OfertaCompraDto>($"api/v1/ofertascompra/{id}");
    }

    public async Task<(bool ok, string? erro)> CriarAsync(OfertaCompraRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PostAsJsonAsync("api/v1/ofertascompra", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> EditarAsync(int id, OfertaCompraRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PutAsJsonAsync($"api/v1/ofertascompra/{id}", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> ExcluirAsync(int id)
    {
        await AddJwtAsync();
        var resp = await _http.DeleteAsync($"api/v1/ofertascompra/{id}");
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public class OfertaCompraDto
    {
        public int Id { get; set; }
        public int EventoId { get; set; }
        public string EventoNome { get; set; }
        public int CompradorId { get; set; }
        public string CompradorNome { get; set; }
        public string TipoIngresso { get; set; }
        public int Quantidade { get; set; }
        public bool Ativa { get; set; }
    }
    public class OfertaCompraRequest
    {
        public int EventoId { get; set; }
        public string TipoIngresso { get; set; }
        public int Quantidade { get; set; }
        public bool Ativa { get; set; }
    }
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
    }
    public async Task<PagedResult<OfertaCompraDto>> ListarPaginadoAsync(string url)
    {
        var result = await _http.GetFromJsonAsync<PagedResult<OfertaCompraDto>>(url);
        return result ?? new PagedResult<OfertaCompraDto> { Data = new List<OfertaCompraDto>() };
    }
}
