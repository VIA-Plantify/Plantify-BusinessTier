using System.Text;
using System.Text.Json;
using Azure.Data.Tables;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Client;

namespace Plantify.BusinessTier.Services;

public class MqttSensorService
{
    private readonly IMqttClient _client;
    private readonly TableClient _tableClient;

    public MqttSensorService(IConfiguration configuration)
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
}