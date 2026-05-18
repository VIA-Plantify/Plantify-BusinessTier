using GrpcRepositories;
using Plantify.BusinessTier.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddSingleton<MqttSensorService>();

var grpcAddress =
    builder.Configuration["GrpcServer:Address"]
    ?? throw new InvalidOperationException(
        "GrpcServer:Address is missing.");

builder.Services.AddGrpcClient<
    SensorServiceProto.SensorServiceProtoClient>(
    options =>
    {
        options.Address = new Uri(grpcAddress);
    });

var app = builder.Build();

Console.WriteLine("Starting MQTT service...");

var mqttService =
    app.Services.GetRequiredService<MqttSensorService>();

await mqttService.ConnectAsync();

Console.WriteLine("MQTT service started.");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.MapPost(
    "/arduino/{macAddress}/water/{seconds}",
    async (
        string macAddress,
        int seconds,
        MqttSensorService mqttService) =>
    {
        await mqttService.SendWaterCommandAsync(
            macAddress,
            seconds);

        return Results.Ok(new
        {
            message = "Water command sent",
            macAddress,
            seconds
        });
    });

app.Run();