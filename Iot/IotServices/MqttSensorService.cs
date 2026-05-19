using System.Text;
using System.Text.Json;
using Azure.Data.Tables;
using Entities.Plant;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;
using ServiceContracts;

namespace IotServices;

public class MqttSensorService
{
    private readonly IMqttClient _client;
    private readonly TableClient _tableClient;
    private readonly ISensorService _sensorService;
    private readonly IWateringService _wateringService;

    public MqttSensorService(IConfiguration configuration, ISensorService sensorService, IWateringService wateringService)
    {
        Console.WriteLine("=== MQTT SERVICE STARTING ===");

        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        _client.ApplicationMessageReceivedAsync += HandleMqttMessage;

        var connectionString =
            configuration["AzureTableStorage:ConnectionString"]
            ?? throw new InvalidOperationException(
                "AzureTableStorage:ConnectionString is missing.");

        _tableClient = new TableClient(connectionString, "ArduinoMessages");
        _tableClient.CreateIfNotExists();

        Console.WriteLine("Azure Table Storage ready.");
        
        //
        _sensorService = sensorService;
        _wateringService = wateringService;
    }

    public async Task ConnectAsync()
    {
        Console.WriteLine("=== CONNECTING TO MQTT BROKER ===");

        if (_client.IsConnected)
        {
            Console.WriteLine("Already connected.");
            return;
        }

        var options = new MqttClientOptionsBuilder()
            .WithClientId($"plantify-api-{Guid.NewGuid()}")
            .WithTcpServer("46.62.140.54", 1883)
            .WithCredentials("Plantify", "Password123")
            .Build();

        await _client.ConnectAsync(options);

        Console.WriteLine("CONNECTED SUCCESSFULLY");

        await _client.SubscribeAsync("arduino/+/sensors");

        Console.WriteLine("SUBSCRIBED: arduino/+/sensors");
    }

    private async Task HandleMqttMessage(
        MqttApplicationMessageReceivedEventArgs e)
    {
        var topic = e.ApplicationMessage.Topic;

        var payload = Encoding.UTF8.GetString(
            e.ApplicationMessage.PayloadSegment);

        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

        Console.WriteLine("=== MQTT MESSAGE RECEIVED ===");
        Console.WriteLine($"TIMESTAMP: {timestamp}");
        Console.WriteLine($"TOPIC: {topic}");
        Console.WriteLine($"PAYLOAD: {payload}");

        string jsonWithTimestamp;

        try
        {
            using var jsonDocument = JsonDocument.Parse(payload);

            jsonWithTimestamp = JsonSerializer.Serialize(
                new
                {
                    timestamp,
                    topic,
                    data = jsonDocument.RootElement
                },
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }
        catch
        {
            jsonWithTimestamp = JsonSerializer.Serialize(
                new
                {
                    timestamp,
                    topic,
                    rawPayload = payload
                },
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });
        }

        var entity = new TableEntity(
            partitionKey: "arduino",
            rowKey: Guid.NewGuid().ToString())
        {
            ["Topic"] = topic,
            ["Json"] = jsonWithTimestamp,
            ["Timestamp"] = timestamp
        };
        
        await _tableClient.AddEntityAsync(entity);

        Console.WriteLine("Saved to Azure Table Storage.");
    }
    
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

        Console.WriteLine("=== SENDING WATER COMMAND ===");
        Console.WriteLine($"TOPIC: {topic}");
        Console.WriteLine($"PAYLOAD: {payload}");

        await _client.PublishAsync(message);

        Console.WriteLine("Water command sent.");
    }
    private async Task HandleMqttMessageToDatabase(MqttApplicationMessageReceivedEventArgs e)
    {
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
        try
        {
            using var doc = JsonDocument.Parse(payload);
            
            if (!doc.RootElement.TryGetProperty("data", out var data))
            {
                Console.WriteLine("Payload missing 'data' field — skipping.");
                return;
            }

            var sensorData = new SensorData
            {
                PlantMAC       = data.TryGetProperty("mac", out var mac) ? mac.GetString() ?? "unknown" : "unknown",
                Timestamp      = DateTime.UtcNow,
                Temperature    = data.TryGetProperty("temp",  out var temp)  ? temp.GetDouble()  : 0,
                AirHumidity    = data.TryGetProperty("hum",   out var hum)   ? hum.GetDouble()   : 0,
                SoilHumidity   = data.TryGetProperty("soil",  out var soil)  ? soil.GetDouble()  : 0,
                LightIntensity = data.TryGetProperty("light", out var light) ? light.GetDouble() : 0,
            };

            await _sensorService.CreateSensorData(sensorData);
            Console.WriteLine($"Saved | mac={sensorData.PlantMAC} temp={sensorData.Temperature} " +
                              $"airHum={sensorData.AirHumidity} soilHum={sensorData.SoilHumidity} " +
                              $"light={sensorData.LightIntensity}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to parse payload: {ex.Message} | raw: {payload}");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine($"Sensor value out of range: {ex.Message}");
        }
    }

}