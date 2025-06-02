using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using VendePraMim.Blazor;
using VendePraMim.Blazor.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthenticationService>());
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<IngressoService>();
builder.Services.AddScoped<OfertaCompraService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<TransacaoService>();

await builder.Build().RunAsync();
