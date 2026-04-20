using Microsoft.EntityFrameworkCore;
using ZaroShop.Server.Data;
using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.Entities;
using ZaroShop.Server.Repositories;
using ZaroShop.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductSearchEngine, ProductSearchEngine>();

builder.Services.AddScoped<IRepository<Product>, EfRepository<Product>>();

builder.Services.AddScoped<IRepository<Category>, EfRepository<Category>>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CustomLoggingMiddleware>();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
