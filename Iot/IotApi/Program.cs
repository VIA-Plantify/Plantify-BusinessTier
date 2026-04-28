using GrpcRepositories;
using ServiceContracts;
using RepositoryContracts;
using GrpcRepositories.Services;
using Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

var grpcAddress = builder.Configuration["GrpcServer:Address"]
                  ?? throw new InvalidOperationException("GrpcServer:Address is missing.");

builder.Services.AddGrpcClient<SoilHumidityServiceProto.SoilHumidityServiceProtoClient>(options =>
{
    options.Address = new Uri(grpcAddress);
});

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

builder.Services.AddScoped<ISoilHumidityRepository, SoilHumidityRepositoryGrpc>();
builder.Services.AddScoped<ISoilHumidityService, SoilHumidityService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("Arduino");

app.UseAuthorization();

app.MapControllers();

app.Run();