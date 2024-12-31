using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuration de base
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Ajouter Ocelot
builder.Services.AddOcelot(builder.Configuration);

// Ajouter CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Activer CORS
app.UseCors("AllowAll");

// Configuration Ocelot
await app.UseOcelot();

app.Run();
