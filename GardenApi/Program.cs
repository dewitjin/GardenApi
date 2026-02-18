using Microsoft.EntityFrameworkCore;
using GardenApi.Data;
using GardenApi.Services.Interfaces;
using GardenApi.Services;
using Azure.Storage.Blobs;
using Azure.Identity;
using Microsoft.Extensions.Azure;
using Azure.Messaging.ServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<GardenDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GardenDb")));

builder.Services.AddSingleton<BlobServiceClient>(
    sp => new BlobServiceClient(
        new Uri("https://gardenapi2026.blob.core.windows.net/"),
        new DefaultAzureCredential()));


builder.Services.AddAzureClients(azure =>
{
    azure.AddServiceBusClient(builder.Configuration.GetConnectionString("ServiceBus"));
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    return client.CreateSender("image-review-queue");
});



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
builder.Services.AddScoped<IImageService, PlantImageService>();

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