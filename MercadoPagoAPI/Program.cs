using MercadoPagoAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MercadoPagoAPI.Models.AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<ICheckoutService, MercadoPagoService>();
builder.Services.AddScoped<IPreferenceService, PreferenceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Bloque para ejecutar migraciones automáticas
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<MercadoPagoAPI.Models.AppDbContext>();
        // Esto aplica cualquier migración pendiente en cada inicio
        context.Database.Migrate();
        Console.WriteLine("--> Migraciones ejecutadas con éxito.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Error ejecutando migraciones: {ex.Message}");
    }
}

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
