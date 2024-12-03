using API_Modul295.Data;
using API_Modul295.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DbContext konfigurieren
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrierung des OrderService
builder.Services.AddScoped<IOrderService, OrderService>();

// Swagger konfigurieren
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_Modul295", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Swagger-Middleware hinzufügen
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_Modul295 v1");
});

app.UseAuthorization();

app.MapControllers();

app.Run();