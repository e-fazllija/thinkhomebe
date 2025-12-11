using BackEnd.Data;
using BackEnd.Entities;
using BackEnd.Models.Options;
using BackEnd.Services;
using BackEnd.Interfaces;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = false;
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Disabilita la conversione a camelCase
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register KeyVault service as singleton for better performance
builder.Services.AddSingleton<IKeyVaultService, KeyVaultService>();

// Configure services using the centralized KeyVault service
builder.ConfigureDatabase();
builder.ConfigureServices();
builder.Services.Configure<PaginationOptions>(builder.Configuration.GetSection("PaginationOptions"));
builder.Services.Configure<MailOptions>(builder.Configuration.GetSection("MailOptions"));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddCors();

// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.ConfigureJwt();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors(
    options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()
);

// Seed locations data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await LocationDataSeeder.SeedLocations(context);
}

app.Run();
