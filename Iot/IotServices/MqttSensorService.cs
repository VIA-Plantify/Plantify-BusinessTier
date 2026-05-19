using System.Text;
using System.Text.Json;
using Entities.Plant;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using ServiceContracts;

namespace IotServices;

public class MqttSensorService
{
    private readonly IMqttClient _client;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MqttSensorService> _logger;

    public MqttSensorService(ILogger<MqttSensorService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _logger.LogInformation("=== MQTT SERVICE STARTING ===");

        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        _client.ApplicationMessageReceivedAsync += HandleMqttMessageToDatabase;
        _serviceProvider = serviceProvider;
        
    }

    public async Task ConnectAsync()
    { 
        _logger.LogInformation("=== CONNECTING TO MQTT BROKER ===");

        if (_client.IsConnected)
        {
            _logger.LogInformation("Already connected.");
            return;
        }

        var options = new MqttClientOptionsBuilder()
            .WithClientId($"plantify-api-{Guid.NewGuid()}")
            .WithTcpServer("46.62.140.54", 1883)
            .WithCredentials("Plantify", "Password123")
            .Build();

        await _client.ConnectAsync(options);

        _logger.LogInformation("CONNECTED SUCCESSFULLY");

        await _client.SubscribeAsync("arduino/+/sensors");

        _logger.LogInformation("SUBSCRIBED: arduino/+/sensors");
    }

    // private async Task HandleMqttMessage(
    //     MqttApplicationMessageReceivedEventArgs e)
    // {
    //     var topic = e.ApplicationMessage.Topic;
    //
    //     var payload = Encoding.UTF8.GetString(
    //         e.ApplicationMessage.PayloadSegment);
    //
    //     var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
    //
    //     _logger.LogInformation("=== MQTT MESSAGE RECEIVED ===");
    //     _logger.LogInformation($"TIMESTAMP: {timestamp}");
    //     _logger.LogInformation($"TOPIC: {topic}");
    //     _logger.LogInformation($"PAYLOAD: {payload}");
    //
    //     string jsonWithTimestamp;
    //
    //     try
    //     {
    //         using var jsonDocument = JsonDocument.Parse(payload);
    //
    //         jsonWithTimestamp = JsonSerializer.Serialize(
    //             new
    //             {
    //                 timestamp,
    //                 topic,
    //                 data = jsonDocument.RootElement
    //             },
    //             new JsonSerializerOptions
    //             {
    //                 WriteIndented = true
    //             });
    //     }
    //     catch
    //     {
    //         jsonWithTimestamp = JsonSerializer.Serialize(
    //             new
    //             {
    //                 timestamp,
    //                 topic,
    //                 rawPayload = payload
    //             },
    //             new JsonSerializerOptions
    //             {
    //                 WriteIndented = true
    //             });
    //     }
    //
    //     var entity = new TableEntity(
    //         partitionKey: "arduino",
    //         rowKey: Guid.NewGuid().ToString())
    //     {
    //         ["Topic"] = topic,
    //         ["Json"] = jsonWithTimestamp,
    //         ["Timestamp"] = timestamp
    //     };
    //     
    //     await _tableClient.AddEntityAsync(entity);
    //
    //     _logger.LogInformation("Saved to Azure Table Storage.");
    // }
    
    // Command for watering
    public async Task SendWaterCommandAsync(
        string macAddress,
        int seconds)
    {
        await ConnectAsync();

        var topic = $"arduino/{macAddress}/commands";

        var payload = $"pump_on_{seconds}";

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();

        _logger.LogInformation("=== SENDING WATER COMMAND ===");
        _logger.LogInformation($"TOPIC: {topic}");
        _logger.LogInformation($"PAYLOAD: {payload}");

        await _client.PublishAsync(message);

        _logger.LogInformation("Water command sent.");
    }
    private async Task HandleMqttMessageToDatabase(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        try
        {
            using var doc = JsonDocument.Parse(payload);
            _logger.LogInformation(payload);
            // if (!doc.RootElement.TryGetProperty("data", out var data))
            // {
            //     _logger.LogInformation("Payload missing 'data' field — skipping.");
            //     return;
            // }

            var sensorData = new SensorData
            {
                PlantMAC       = doc.RootElement.TryGetProperty("mac", out var mac) ? mac.GetString() ?? "unknown" : "unknown",
                Timestamp      = DateTime.UtcNow,
                Temperature    = doc.RootElement.TryGetProperty("temp",  out var temp)  ? temp.GetDouble()  : 0,
                AirHumidity    = doc.RootElement.TryGetProperty("hum",   out var hum)   ? hum.GetDouble()   : 0,
                SoilHumidity   = doc.RootElement.TryGetProperty("soil",  out var soil)  ? soil.GetDouble()  : 0,
                LightIntensity = doc.RootElement.TryGetProperty("light", out var light) ? light.GetDouble() : 0,
            };
            _logger.LogInformation($"Sensor: {sensorData}");
            using (var scope = _serviceProvider.CreateScope())
            {
                var sensorService = scope.ServiceProvider.GetRequiredService<ISensorService>();
                await sensorService.CreateSensorData(sensorData);
            }
            _logger.LogInformation($"Saved | mac={sensorData.PlantMAC} temp={sensorData.Temperature} " +
                                   $"airHum={sensorData.AirHumidity} soilHum={sensorData.SoilHumidity} " +
                                   $"light={sensorData.LightIntensity}");
        }
        catch (JsonException ex)
        {
            _logger.LogInformation($"Failed to parse payload: {ex.Message} | raw: {payload}");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.LogInformation($"Sensor value out of range: {ex.Message}");
        }
    }

}