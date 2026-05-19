using Entities.Plant;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RepositoryContracts;
using ServiceContracts;

namespace IotServices;

public class ScheduledWateringService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ScheduledWateringService> _logger;
    private readonly IConfiguration _configuration;
    private Timer? _timer;

    public ScheduledWateringService(
        IServiceProvider serviceProvider,
        ILogger<ScheduledWateringService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Scheduled Watering Service started");
        
        // Calculate time until next 12 PM
        var now = DateTime.Now;
        var nextRun = now.Date.AddHours(12);
        if (now > nextRun)
            nextRun = nextRun.AddDays(1);
        
        var timeUntilNextRun = nextRun - now;
        
        _logger.LogInformation($"Next watering scheduled for {nextRun}");
        
        // Initial delay until 12 PM
        await Task.Delay(timeUntilNextRun, stoppingToken);
        
        _timer = new Timer(
            async _ => await WaterAllPlantsAsync(),
            null,
            TimeSpan.Zero,
            TimeSpan.FromHours(24));
        
        await Task.CompletedTask;
    }

    private async Task WaterAllPlantsAsync()
    {
        _logger.LogInformation("=== SCHEDULED WATERING STARTED ===");

        using (var scope = _serviceProvider.CreateScope())
        {
            var plantService = scope.ServiceProvider.GetRequiredService<IPlantService>();
            var wateringService = scope.ServiceProvider.GetRequiredService<IWateringService>();
            var mqttService = scope.ServiceProvider.GetRequiredService<MqttSensorService>();
            var httpClient = scope.ServiceProvider.GetRequiredService<HttpClient>();

            var apiKey = _configuration["ML_API_KEY"];

            // Get all plants
            var plants = await plantService.GetAllPlantsAsync();

            foreach (var plant in plants)
            {
                try
                {
                    // Call ML endpoint to get recommendation
                    var request = new HttpRequestMessage(
                        HttpMethod.Get,
                        $"{apiKey}/pumptime/{plant.Username}/{plant.MAC}");
                    var response = await httpClient.SendAsync(request);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning($"ML API failed for {plant.MAC}");
                        continue;
                    }
                    
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var pumpSeconds = int.TryParse(responseContent.Trim(), out var seconds) ? seconds : 0;
                    
                    await mqttService.SendWaterCommandAsync(plant.MAC, pumpSeconds);
                    // Log the watering
                    var pastReading = await wateringService.GetAsync(plant.MAC);
                    var watering = new Watering
                    {
                       PumpTimeInSeconds = pumpSeconds,
                       WaterLevel =pastReading?.WaterLevel,
                       LastWaterTime = DateTime.UtcNow,
                    };

                    await wateringService.CreateAsync(plant.MAC,watering);

                    _logger.LogInformation($"Watered plant {plant.MAC} for {pumpSeconds}s");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error watering {plant.MAC}: {ex.Message}");
                }
            }

            _logger.LogInformation("=== SCHEDULED WATERING COMPLETED ===");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        await base.StopAsync(cancellationToken);
    }
}