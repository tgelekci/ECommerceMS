using FluentValidation;
using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models;
using UserService.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDbContext>(opt =>


    opt.UseInMemoryDatabase("UsersDb"));

builder.Services.AddValidatorsFromAssemblyContaining<UserValidator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new User { Id = 1, Name = "Ali Yilmaz", Email = "ali@example.com", Phone = "555-0001" },
            new User { Id = 2, Name = "Ayde Kaya", Email = "ayse@example.com", Phone = "555-0002" },
            new User { Id = 3, Name = "Mehmet Demir", Email = "mehmet@example.com", Phone = "555-0003" }
        );
        db.SaveChanges();
    }
}


app.UseSwagger();

app.UseSwaggerUI();

app.MapControllers();

app.Run();
