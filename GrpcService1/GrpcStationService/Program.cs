using GrpcStationService.Data;
using GrpcStationService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Настройка подключения к PostgreSQL
builder.Services.AddDbContext<WagonsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Добавление gRPC
builder.Services.AddGrpc();

var app = builder.Build();

app.MapGrpcService<WagonService>(); // Регистрация сервиса
app.MapGet("/", () => "Use a gRPC client to communicate with this server.");

app.Run();