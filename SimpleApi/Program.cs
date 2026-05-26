using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SimpleApi.Data;
using SimpleApi.Handlers;
using SimpleApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// 1. DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. Repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// 3. AutoMapper
builder.Services.AddAutoMapper(config => config.AddMaps(typeof(Program).Assembly));

// 4. FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// 5. Global Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();