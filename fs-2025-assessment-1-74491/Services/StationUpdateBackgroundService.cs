namespace fs_2025_assessment_1_74491.Services;

public class StationUpdateBackgroundService : BackgroundService
{
    private readonly IStationService _stationService;
    private readonly ILogger<StationUpdateBackgroundService> _logger;
    private readonly Random _random = new();

    public StationUpdateBackgroundService(
        IStationService stationService,
        ILogger<StationUpdateBackgroundService> logger)
    {
        _stationService = stationService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _stationService.ApplyRandomUpdates();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying random station updates.");
            }

            var delaySeconds = _random.Next(10, 21);
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds), stoppingToken);
        }
    }
}
