using manutencao_api.Models;
using manutencao_api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. REGISTRO DE SERVIÇOS
// ==========================================

// Configuração do CORS (Liberando para todos)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Permite requisições de qualquer URL (ex: localhost:3000, localhost:5173)
              .AllowAnyMethod()   // Permite qualquer método (GET, POST, PUT, DELETE, OPTIONS)
              .AllowAnyHeader();  // Permite qualquer cabeçalho (Authorization, Content-Type, etc)
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// Registrar o gerador do Swagger (Swashbuckle)
builder.Services.AddSwaggerGen();

// Registra o TokenService e o Banco de Dados
builder.Services.AddScoped<TokenService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==========================================
// JWT
// ==========================================
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]!);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});

// ==========================================
// 2. CONSTRUÇÃO DA APLICAÇÃO
// ==========================================
var app = builder.Build();

// ==========================================
// 3. MIDDLEWARES
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ==========================================
// CORS (Deve ser chamado ANTES de Authentication e Authorization)
// ==========================================
app.UseCors("AllowAll");

// ==========================================
// SEGURANÇA
// ==========================================
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();