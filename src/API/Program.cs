using MediatR;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateClienteCommand).Assembly));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirect desativado por enquanto, para facilitar testes locais via HTTP no Swagger
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
