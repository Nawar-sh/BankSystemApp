using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BankingSystem.Application.Interfaces;
using BankingSystem.Infrastructure.Data;
using BankingSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// 1. ربط الـ DbContext مع SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. تسجيل الـ Repository
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Banking System API V1");
    c.RoutePrefix = string.Empty; // هذا السطر سيجعل الـ Swagger يفتح مباشرة على الصفحة الرئيسية!
});
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();