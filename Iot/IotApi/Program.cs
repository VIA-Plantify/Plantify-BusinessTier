using GrpcRepositories;
using ServiceContracts;
using RepositoryContracts;
using GrpcRepositories.Services;
using Services;
using Plantify.BusinessTier.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<ISensorRepository, SensorRepositoryGrpc>();

builder.Services.AddSingleton<MqttSensorService>();

var grpcAddress = builder.Configuration["GrpcServer:Address"]
                  ?? throw new InvalidOperationException("GrpcServer:Address is missing.");

builder.Services.AddGrpcClient<SensorServiceProto.SensorServiceProtoClient>(options =>
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

var app = builder.Build();

Console.WriteLine("Starting MQTT service...");

var mqttService = app.Services.GetRequiredService<MqttSensorService>();
await mqttService.ConnectAsync();

Console.WriteLine("MQTT service started.");

app.UseRouting();

app.UseCors("Arduino");

app.UseAuthorization();

app.MapControllers();

app.MapPost("/arduino/{macAddress}/read-sensors", async (
    string macAddress,
    MqttSensorService mqttSensorService) =>
{
    try
    {
        Console.WriteLine($"API called: read sensors for {macAddress}");

        var sensorData =
            await mqttSensorService.AskArduinoForDataAsync(macAddress);

        return Results.Ok(sensorData);
    }
    catch (TimeoutException ex)
    {
        Console.WriteLine($"Timeout: {ex.Message}");

        return Results.Problem(
            ex.Message,
            statusCode: 504);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");

        return Results.Problem(
            ex.Message,
            statusCode: 500);
    }
});

app.MapPost("/arduino/{macAddress}/water/{pumpTime}", async (
    string macAddress,
    int pumpTime,
    MqttSensorService mqttSensorService) =>
{
    try
    {
        Console.WriteLine($"API called: water plant {macAddress} for {pumpTime} ms");

        await mqttSensorService.SendWaterCommandAsync(
            macAddress,
            pumpTime);

        return Results.Ok(new
        {
            Message = "Water command sent",
            MacAddress = macAddress,
            MethodName = "water",
            PumpTime = pumpTime
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending water command: {ex.Message}");

        return Results.Problem(
            ex.Message,
            statusCode: 500);
    }
});

app.MapPost("/arduino/{macAddress}/mist/{pumpTime}", async (
    string macAddress,
    int pumpTime,
    MqttSensorService mqttSensorService) =>
{
    try
    {
        Console.WriteLine($"API called: mist plant {macAddress} for {pumpTime} ms");

        await mqttSensorService.SendMistCommandAsync(
            macAddress,
            pumpTime);

        return Results.Ok(new
        {
            Message = "Mist command sent",
            MacAddress = macAddress,
            MethodName = "mist",
            PumpTime = pumpTime
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error sending mist command: {ex.Message}");

        return Results.Problem(
            ex.Message,
            statusCode: 500);
    }
});

app.Run();