using Bakery.Data;
using Bakery.Endpoints;
using Bakery.Repositories;
using Bakery.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bakery API", Description = "Welcome to the Bakery!", Version = "v1" });
});

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlite<BakeryDbContext>(connectionString);

// Register services
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IOptionRepository, OptionRepository>();


var app = builder.Build();

app.MapProductsEndpoints();
app.MapCategoriesEndpoints();
app.MapOptionsEndpoints();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
