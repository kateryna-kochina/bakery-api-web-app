using Bakery.Data;
using Bakery.Dtos;
using Bakery.Endpoints;
using Bakery.Mapping;
using Bakery.Repositories;
using Bakery.Repositories.Contracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using static Bakery.Helpers.CategoryValidator;
using static Bakery.Helpers.OptionValidator;
using static Bakery.Helpers.ProductValidator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bakery API", Description = "Welcome to the Bakery!", Version = "v1" });
});
builder.Services.AddAutoMapper(typeof(MappingProfiles));

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSqlite<BakeryDbContext>(connectionString);

// Register services
builder.Services.AddTransient<IProductRepository, ProductRepository>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<IOptionRepository, OptionRepository>();

// Register validators
builder.Services.AddScoped<IValidator<CreateCategoryDto>, CreateCategoryDtoValidator>();
builder.Services.AddScoped<IValidator<CreateOptionDto>, CreateOptionDtoValidator>();
builder.Services.AddScoped<IValidator<CreateProductDto>, CreateProductDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateCategoryDto>, UpdateCategoryDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateOptionDto>, UpdateOptionDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateProductDto>, UpdateProductDtoValidator>();


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
