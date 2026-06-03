using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models;
using ProductService.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ProductDbContext>(opt =>
    opt.UseInMemoryDatabase("ProductsDb"));

builder.Services.AddValidatorsFromAssemblyContaining<ProductValidator>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    db.Database.EnsureCreated();

    if (!db.Products.Any())
    {
        db.Products.AddRange(
            new Product { Id = 1, Name = "Laptop", Description = "Yuksek performansli dizustu bilgisayar", Price = 45000m, Stock = 10 },
            new Product { Id = 2, Name = "Mouse", Description = "Kablosuz mouse", Price = 350m, Stock = 50 },
            new Product { Id = 3, Name = "Klavye", Description = "Mekanik klavye", Price = 1200m, Stock = 30 }
        );
        db.SaveChanges();
    }
}

app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.Run();
