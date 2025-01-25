using GrpcStationService.Data;
using GrpcStationService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Настройка подключения к PostgreSQL
builder.Services.AddDbContext<WagonsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// Добавление gRPC и CORS
builder.Services.AddGrpc();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost") // Укажите URL клиента
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.Urls.Add("http://localhost:5000");
// Подключение CORS и gRPC-Web
app.UseCors();
app.UseGrpcWeb();

// Настройка маршрутов gRPC
app.MapGrpcService<WagonService>().EnableGrpcWeb();

app.MapGet("/", () => "Сервер работает. Используйте gRPC клиент.");
app.Run();
