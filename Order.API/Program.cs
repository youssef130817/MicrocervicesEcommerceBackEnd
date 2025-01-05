using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Order.API.Data;
using Order.API.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.MaxDepth = 64;
    });

// Configure JWT Authentication
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured"))
            )
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var tokenValidationService = context.HttpContext.RequestServices.GetRequiredService<TokenValidationService>();

                // Récupérer le token depuis l'en-tête Authorization
                var token = context.HttpContext.Request.Headers["Authorization"]
                    .ToString().Replace("Bearer ", "");

                var validationResult = await tokenValidationService.ValidateTokenAsync(token);
                if (validationResult == null || !validationResult.IsValid)
                {
                    context.Fail("Token non valide");
                    return;
                }

                // Mettre à jour les claims avec les informations de Authentication.API
                if (!string.IsNullOrEmpty(validationResult.UserId))
                {
                    var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, validationResult.UserId));

                    if (!string.IsNullOrEmpty(validationResult.Role))
                    {
                        identity.AddClaim(new Claim(ClaimTypes.Role, validationResult.Role));
                    }

                    var principal = new ClaimsPrincipal(identity);
                    context.Principal = principal;
                }
            }
        };
    });

// Configure PostgreSQL
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Order Service
builder.Services.AddScoped<OrderService>();

// Add Token Validation Service
builder.Services.AddSingleton<TokenValidationService>();

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialiser le TokenValidationService au démarrage
var tokenValidationService = app.Services.GetRequiredService<TokenValidationService>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Désactivé pour le développement
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

app.Run();
