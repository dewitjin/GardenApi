using Microsoft.EntityFrameworkCore;
using GardenApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<GardenDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GardenDb")));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Add later.
}

app.MapOpenApi();

app.UseAuthorization();

app.MapControllers();

app.Run();
