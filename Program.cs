using API_Modul295.Data;
using API_Modul295.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DbContext konfigurieren
builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrierung von Services
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<AuthService>(); // AuthService registrieren

// JWT-Authentifizierung konfigurieren
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Konfiguration der JWT-Tokenprüfung
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Aus der Konfiguration lesen
            ValidAudience = builder.Configuration["Jwt:Audience"], // Aus der Konfiguration lesen
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // Geheimer Schlüssel aus der Konfiguration
        };

        // Optional: Einrichten von Ereignissen wie z.B. Fehlerbehandlung
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

// Swagger konfigurieren
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API_Modul295", Version = "v1" });

    // JWT-Token Header für Swagger konfigurieren
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Please enter 'Bearer' followed by a space and then your JWT token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API_Modul295 v1");

    // Entfernt die ModelRendering-Einstellung, die den Fehler verursacht hat
    c.DefaultModelsExpandDepth(-1); // Keine Modelle im UI anzeigen
    c.DefaultModelExpandDepth(2); // Modelle anzeigen, aber standardmäßig zusammenklappen
});

// Authentifizierung und Autorisierung Middleware
app.UseAuthentication();  // Authentifizierung Middleware
app.UseAuthorization();   // Autorisierungs Middleware

// Routing und Controller
app.MapControllers();

app.Run();
