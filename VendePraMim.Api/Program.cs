using Microsoft.EntityFrameworkCore;
using VendePraMim.Api.Models;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

// Configuração do DbContext com SQLite
builder.Services.AddDbContext<VendePraMimContext>(options =>
    options.UseSqlite("Data Source=vendepra.db"));

// Adiciona suporte a controllers
builder.Services.AddControllers();

// Adiciona suporte ao Swagger
builder.Services.AddEndpointsApiExplorer();
var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

// Adiciona autenticação JWT
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
        };
    });

// Configuração do CORS para permitir qualquer origem (ajuste conforme necessário)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddScoped<VendePraMim.Api.Services.IngressoService>();
builder.Services.AddScoped<VendePraMim.Api.Services.EventoService>();
builder.Services.AddScoped<VendePraMim.Api.Services.UsuarioService>();
builder.Services.AddScoped<VendePraMim.Api.Services.OfertaCompraService>();
builder.Services.AddScoped<VendePraMim.Api.Services.TransacaoService>();

var app = builder.Build();

// Middleware global de tratamento de erros
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        var errorResponse = new VendePraMim.Api.Models.ErrorResponse
        {
            Error = "Erro interno do servidor",
            Details = app.Environment.IsDevelopment() ? ex.ToString() : null
        };
        await context.Response.WriteAsJsonAsync(errorResponse);
    }
});

// Ativa o Swagger em ambiente de desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usa o roteamento de controllers
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

app.Run();
