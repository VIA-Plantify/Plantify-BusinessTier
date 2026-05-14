using Plantify.BusinessTier.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthorization();

builder.Services.AddSingleton<MqttSensorService>();

var app = builder.Build();

Console.WriteLine("Starting MQTT service...");

var mqttService =
    app.Services.GetRequiredService<MqttSensorService>();

await mqttService.ConnectAsync();

Console.WriteLine("MQTT service started.");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();