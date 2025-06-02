using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.JSInterop;

namespace VendePraMim.Blazor.Services;

public class UsuarioService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;
    public UsuarioService(HttpClient http, IJSRuntime js)
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

    public async Task<List<UsuarioDto>> ListarAsync()
    {
        var result = await _http.GetFromJsonAsync<PagedResult<UsuarioDto>>("api/v1/usuarios?page=1&pageSize=50");
        return result?.Data ?? new();
    }

    public async Task<UsuarioDto?> ObterAsync(int id)
    {
        return await _http.GetFromJsonAsync<UsuarioDto>($"api/v1/usuarios/{id}");
    }

    public async Task<(bool ok, string? erro)> CriarAsync(UsuarioRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PostAsJsonAsync("api/v1/usuarios", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> EditarAsync(int id, UsuarioRequest dto)
    {
        await AddJwtAsync();
        var resp = await _http.PutAsJsonAsync($"api/v1/usuarios/{id}", dto);
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public async Task<(bool ok, string? erro)> ExcluirAsync(int id)
    {
        await AddJwtAsync();
        var resp = await _http.DeleteAsync($"api/v1/usuarios/{id}");
        if (resp.IsSuccessStatusCode) return (true, null);
        var msg = await resp.Content.ReadAsStringAsync();
        return (false, msg);
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
    public class UsuarioRequest
    {
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Senha { get; set; }
        public string Role { get; set; }
    }
    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public List<T> Data { get; set; }
    }
    public async Task<PagedResult<UsuarioDto>> ListarPaginadoAsync(string url)
    {
        var result = await _http.GetFromJsonAsync<PagedResult<UsuarioDto>>(url);
        return result ?? new PagedResult<UsuarioDto> { Data = new List<UsuarioDto>() };
    }
}
