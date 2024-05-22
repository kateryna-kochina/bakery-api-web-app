using Bakery.Data;
using Bakery.Endpoints;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Bakery") ?? "Data Source=Bakery.db";

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bakery API", Description = "Welcome to the Bakery!", Version = "v1" });
});
builder.Services.AddSqlite<BakeryDbContext>(connectionString);

var app = builder.Build();

app.MapProductsEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
