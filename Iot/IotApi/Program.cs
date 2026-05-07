using GrpcRepositories;
using ServiceContracts;
using RepositoryContracts;
using GrpcRepositories.Services;
//using Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var grpcAddress = builder.Configuration["GrpcServer:Address"]
                  ?? throw new InvalidOperationException("GrpcServer:Address is missing.");


var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Arduino", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseRouting();

app.UseCors("Arduino");

app.UseAuthorization();

app.MapControllers();

app.Run();