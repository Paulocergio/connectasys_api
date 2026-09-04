using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using connectasys_api.Infrastructure.Persistence.Context;
using connectasys_api.Infrastructure.Persistence.Repositories;
using connectasys_api.Core.Application.Interfaces.Repositories;
using connectasys_api.Core.Application.Interfaces.Services;
using connectasys_api.Infrastructure.Security;
using connectasys_api.Core.Application.Commands.Clientes.CreateCliente;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<IContaPagarRepository, ContaPagarRepository>();
builder.Services.AddScoped<IContaReceberRepository, ContaReceberRepository>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<ITokenService, TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateClienteCommand).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string DevCorsPolicy = "DevCors";
builder.Services.AddCors(options =>
{
    // Dev-only: libera qualquer origem em localhost/127.0.0.1 (a porta do Vite/hub pode variar).
    // Sem AllowCredentials porque a autenticação usa Bearer token, não cookie.
    options.AddPolicy(DevCorsPolicy, policy =>
        policy.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseCors(DevCorsPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
