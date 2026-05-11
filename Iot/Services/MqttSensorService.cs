using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using Plantify.BusinessTier.Models;

namespace Plantify.BusinessTier.Services;

public class MqttSensorService
{
    private readonly IMqttClient _client;

    private readonly ConcurrentDictionary<string,
        TaskCompletionSource<SensorResponse>> _pendingRequests = new();

    public MqttSensorService()
    {
        Console.WriteLine("=== MQTT SERVICE STARTING ===");

        var factory = new MqttFactory();

        Console.WriteLine("Creating MQTT client...");

        _client = factory.CreateMqttClient();

        Console.WriteLine("MQTT client created");

        _client.ApplicationMessageReceivedAsync += HandleMqttMessage;
    }

    public async Task ConnectAsync()
    {
        Console.WriteLine("=== CONNECT ASYNC STARTED ===");

        if (_client.IsConnected)
        {
            Console.WriteLine("Already connected to HiveMQ");
            return;
        }

        Console.WriteLine("Building MQTT options...");

        var options = new MqttClientOptionsBuilder()
            .WithClientId("plantify-business-tier")
            .WithTcpServer(
                "676d176f8a7b4b33ab4d8f1733d48d3d.s1.eu.hivemq.cloud",
                8883)
            .WithCredentials(
                "Plantify",
                "Password123")
            .WithTlsOptions(o =>
            {
                o.UseTls();
            })
            .Build();

        Console.WriteLine("Connecting to HiveMQ Cloud...");

        await _client.ConnectAsync(options);

        Console.WriteLine("CONNECTED SUCCESSFULLY");

        Console.WriteLine("Subscribing to topics...");

        await _client.SubscribeAsync("plantify/+/sensors");

        Console.WriteLine("Subscribed to:");
        Console.WriteLine("plantify/+/sensors");

        Console.WriteLine("=== MQTT READY ===");
    }

    public async Task<SensorResponse> AskArduinoForDataAsync(string macAddress)
    {
        Console.WriteLine("=== ASK ARDUINO FOR DATA ===");
        Console.WriteLine($"MAC ADDRESS: {macAddress}");

        await ConnectAsync();

        var requestId = Guid.NewGuid().ToString();

        Console.WriteLine($"REQUEST ID: {requestId}");

        var tcs = new TaskCompletionSource<SensorResponse>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _pendingRequests[requestId] = tcs;

        var request = new SensorRequest
        {
            RequestId = requestId,
            Command = "readSensors"
        };

        var json = JsonSerializer.Serialize(request);

        var topic = $"plantify/{macAddress}/commands";

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(json)
            .Build();

        Console.WriteLine("Publishing MQTT sensor request...");
        Console.WriteLine($"TOPIC: {topic}");
        Console.WriteLine($"PAYLOAD: {json}");

        await _client.PublishAsync(message);

        Console.WriteLine("MQTT SENSOR REQUEST SENT");
        Console.WriteLine("Waiting for Arduino response...");

        var timeoutTask = Task.Delay(TimeSpan.FromSeconds(10));
        var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

        if (completedTask == timeoutTask)
        {
            Console.WriteLine("TIMEOUT");
            Console.WriteLine("Arduino did not answer");

            _pendingRequests.TryRemove(requestId, out _);

            throw new TimeoutException(
                "Arduino did not answer within 10 seconds.");
        }

        Console.WriteLine("ARDUINO RESPONSE RECEIVED");

        return await tcs.Task;
    }

    public async Task SendWaterCommandAsync(string macAddress, int pumpTime)
    {
        await SendPumpCommandAsync(macAddress, "water", pumpTime);
    }

    public async Task SendMistCommandAsync(string macAddress, int pumpTime)
    {
        await SendPumpCommandAsync(macAddress, "mist", pumpTime);
    }

    private async Task SendPumpCommandAsync(
        string macAddress,
        string methodName,
        int pumpTime)
    {
        Console.WriteLine("=== SEND PUMP COMMAND ===");

        await ConnectAsync();

        var command = new PumpCommand
        {
            MethodName = methodName,
            PumpTime = pumpTime
        };

        var json = JsonSerializer.Serialize(command);

        var topic = $"plantify/{macAddress}/commands";

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(json)
            .Build();

        Console.WriteLine($"MAC ADDRESS: {macAddress}");
        Console.WriteLine($"METHOD NAME: {methodName}");
        Console.WriteLine($"PUMP TIME: {pumpTime}");
        Console.WriteLine($"TOPIC: {topic}");
        Console.WriteLine($"PAYLOAD: {json}");

        await _client.PublishAsync(message);

        Console.WriteLine("PUMP COMMAND SENT");
    }

    private Task HandleMqttMessage(
        MqttApplicationMessageReceivedEventArgs e)
    {
        Console.WriteLine("=== MQTT MESSAGE RECEIVED ===");

        var topic = e.ApplicationMessage.Topic;

        var json = Encoding.UTF8.GetString(
            e.ApplicationMessage.PayloadSegment);

        Console.WriteLine($"TOPIC: {topic}");
        Console.WriteLine($"PAYLOAD: {json}");

        try
        {
            var response = JsonSerializer.Deserialize<SensorResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (response == null)
            {
                Console.WriteLine("DESERIALIZATION FAILED");
                return Task.CompletedTask;
            }

            Console.WriteLine($"REQUEST ID: {response.RequestId}");

            if (_pendingRequests.TryRemove(
                response.RequestId,
                out var tcs))
            {
                Console.WriteLine("Matching request found");

                tcs.SetResult(response);

                Console.WriteLine("Response completed");
            }
            else
            {
                Console.WriteLine("No matching request found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR PROCESSING MQTT MESSAGE");
            Console.WriteLine(ex.Message);
        }

        return Task.CompletedTask;
    }
}