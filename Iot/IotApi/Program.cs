using Microsoft.AspNetCore.Mvc;
using Entities.Plant;
using ServiceContracts;
using Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

//Services
builder.Services.AddScoped<ISoilHumidityService, SoilHumidityService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.MapControllers();

app.Run();