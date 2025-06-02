using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace VendePraMim.Blazor.Services;

public class TransacaoService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    public TransacaoService(HttpClient http, IJSRuntime js)
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

    public async Task<List<TransacaoDto>> ListarAsync()
    {
        var result = await _http.GetFromJsonAsync<PagedResult<TransacaoDto>>("api/v1/transacoes?page=1&pageSize=50");
        return result?.Data ?? new();
    }

    public async Task<TransacaoDto?> ObterAsync(int id)
    {
        return await _http.GetFromJsonAsync<TransacaoDto>($"api/v1/transacoes/{id}");
    }

    public async Task<(bool ok, string? erro)> CriarAsync(TransacaoRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PostAsJsonAsync("api/v1/transacoes", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> EditarAsync(int id, TransacaoRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PutAsJsonAsync($"api/v1/transacoes/{id}", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> ExcluirAsync(int id)
    {
        await AddJwtAsync();
        var resp = await _http.DeleteAsync($"api/v1/transacoes/{id}");
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public class TransacaoDto
    {
        public int Id { get; set; }
        public int IngressoId { get; set; }
        public string IngressoTipo { get; set; }
        public int CompradorId { get; set; }
        public string CompradorNome { get; set; }
        public int VendedorId { get; set; }
        public string VendedorNome { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
    }
    public class TransacaoRequest
    {
        public int IngressoId { get; set; }
        public int CompradorId { get; set; }
        public int VendedorId { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
    }
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
    }
    public async Task<PagedResult<TransacaoDto>> ListarPaginadoAsync(string url)
    {
        var result = await _http.GetFromJsonAsync<PagedResult<TransacaoDto>>(url);
        return result ?? new PagedResult<TransacaoDto> { Data = new List<TransacaoDto>() };
    }
}
