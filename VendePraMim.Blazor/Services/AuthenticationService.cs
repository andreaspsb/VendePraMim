using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace VendePraMim.Blazor.Services;

public class AuthenticationService : AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;
    private readonly IJSRuntime _js;
    private const string TokenKey = "jwt_token";
    private ClaimsPrincipal _anonymous => new(new ClaimsIdentity());

    public AuthenticationService(HttpClient http, NavigationManager nav, IJSRuntime js)
    {
        _http = http;
        _nav = nav;
        _js = js;
    }

    public async Task<bool> LoginAsync(string email, string senha)
    {
        var resp = await _http.PostAsJsonAsync("api/v1/auth/login", new { email, senha });
        if (!resp.IsSuccessStatusCode) return false;
        var result = await resp.Content.ReadFromJsonAsync<LoginResult>();
        if (result is null || string.IsNullOrWhiteSpace(result.Token)) return false;
        await SetToken(result.Token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return true;
    }

    public async Task LogoutAsync()
    {
        await SetToken(null);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        _nav.NavigateTo("/");
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await GetToken();
        if (string.IsNullOrWhiteSpace(token))
            return new AuthenticationState(_anonymous);
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public async Task<string?> GetToken()
    {
        return await _js.InvokeAsync<string>("localStorage.getItem", TokenKey);
    }

    private async Task SetToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            await _js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        else
            await _js.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
        await Task.CompletedTask;
    }

    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        return Convert.FromBase64String(base64);
    }

    private class LoginResult { public string Token { get; set; } = string.Empty; }
}
