using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.JSInterop;
using System.Text.Json;

namespace VendePraMim.Blazor.Services;

public class IngressoService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    public IngressoService(HttpClient http, IJSRuntime js)
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

    public async Task<List<IngressoDto>> ListarAsync()
    {
        var result = await _http.GetFromJsonAsync<PagedResult<IngressoDto>>("api/v1/ingressos?page=1&pageSize=50");
        return result?.Data ?? new();
    }

    public async Task<IngressoDto?> ObterAsync(int id)
    {
        return await _http.GetFromJsonAsync<IngressoDto>($"api/v1/ingressos/{id}");
    }

    public async Task<(bool ok, string? erro)> CriarAsync(IngressoRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PostAsJsonAsync("api/v1/ingressos", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> EditarAsync(int id, IngressoRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PutAsJsonAsync($"api/v1/ingressos/{id}", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> ExcluirAsync(int id)
    {
        await AddJwtAsync();
        var resp = await _http.DeleteAsync($"api/v1/ingressos/{id}");
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public class IngressoDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; }
        public decimal Preco { get; set; }
        public int EventoId { get; set; }
        public string EventoNome { get; set; }
        public bool Disponivel { get; set; }
    }
    public class IngressoRequest
    {
        public string Tipo { get; set; }
        public decimal Preco { get; set; }
        public int EventoId { get; set; }
        public bool Disponivel { get; set; }
    }
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
    }
    public async Task<PagedResult<IngressoDto>> ListarPaginadoAsync(string url)
    {
        var result = await _http.GetFromJsonAsync<PagedResult<IngressoDto>>(url);
        return result ?? new PagedResult<IngressoDto> { Data = new List<IngressoDto>() };
    }
}
