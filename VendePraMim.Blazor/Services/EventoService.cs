using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Text;

namespace VendePraMim.Blazor.Services;

public class EventoService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    public EventoService(HttpClient http, IJSRuntime js)
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

    public async Task<List<EventoDto>> ListarAsync()
    {
        var result = await _http.GetFromJsonAsync<PagedResult<EventoDto>>("api/v1/eventos?page=1&pageSize=50");
        return result?.Data ?? new();
    }

    public async Task<EventoDto?> ObterAsync(int id)
    {
        return await _http.GetFromJsonAsync<EventoDto>($"api/v1/eventos/{id}");
    }

    public async Task<(bool ok, string? erro)> CriarAsync(EventoRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PostAsJsonAsync("api/v1/eventos", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> EditarAsync(int id, EventoRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PutAsJsonAsync($"api/v1/eventos/{id}", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> ExcluirAsync(int id)
    {
        await AddJwtAsync();
        var resp = await _http.DeleteAsync($"api/v1/eventos/{id}");
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<List<EventoDto>> ListarPorUrlAsync(string url)
    {
        var result = await _http.GetFromJsonAsync<PagedResult<EventoDto>>(url);
        return result?.Data ?? new();
    }

    public class EventoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public DateTime Data { get; set; }
        public string Local { get; set; }
        public int? OrganizadorId { get; set; }
        public string OrganizadorNome { get; set; }
    }
    public class EventoRequest
    {
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public DateTime Data { get; set; }
        public string Local { get; set; }
        public int? OrganizadorId { get; set; }
    }
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
    }
    public async Task<PagedResult<EventoDto>> ListarPaginadoAsync(string url)
    {
        var result = await _http.GetFromJsonAsync<PagedResult<EventoDto>>(url);
        return result ?? new PagedResult<EventoDto> { Data = new List<EventoDto>() };
    }
}
