using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace VendePraMim.Blazor.Services;

public class AuthenticationService : AuthenticationStateProvider
{
    private readonly HttpClient _http;
    private readonly NavigationManager _nav;
    private const string TokenKey = "jwt_token";
    private ClaimsPrincipal _anonymous => new(new ClaimsIdentity());

    public AuthenticationService(HttpClient http, NavigationManager nav)
    {
        _http = http;
        _nav = nav;
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
        return await Task.FromResult(LocalStorage.Get(TokenKey));
    }

    private async Task SetToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            LocalStorage.Remove(TokenKey);
        else
            LocalStorage.Set(TokenKey, token);
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

// Simples LocalStorage helper (pode ser substituído por pacote NuGet)
public static class LocalStorage
{
    public static void Set(string key, string value) =>
        Microsoft.JSInterop.JSRuntime.Current.InvokeVoidAsync("localStorage.setItem", key, value);
    public static string? Get(string key) =>
        Microsoft.JSInterop.JSRuntime.Current.InvokeAsync<string>("localStorage.getItem", key).Result;
    public static void Remove(string key) =>
        Microsoft.JSInterop.JSRuntime.Current.InvokeVoidAsync("localStorage.removeItem", key);
}
