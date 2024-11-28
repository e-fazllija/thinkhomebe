using BackEnd.Data;
using BackEnd.Entities;
using BackEnd.Models.Options;
using BackEnd.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.WriteIndented = false;
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Disabilita la conversione a camelCase
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseCors(
    options => options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader()
);

app.Run();