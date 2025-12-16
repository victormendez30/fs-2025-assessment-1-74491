using System.Text.Json;
using fs_2025_assessment_1_74491.Models;

namespace fs_2025_assessment_1_74491.Services;

public class StationService : IStationService
{
    public StationSummary GetSummary()
    {
        return new StationSummary
        {
            TotalStations = _stations.Count,
            TotalBikeStands = _stations.Sum(s => s.bike_stands),
            TotalAvailableBikes = _stations.Sum(s => s.available_bikes),
            OpenStations = _stations.Count(s => string.Equals(s.status, "OPEN", StringComparison.OrdinalIgnoreCase)),
            ClosedStations = _stations.Count(s => string.Equals(s.status, "CLOSED", StringComparison.OrdinalIgnoreCase))
        };
    }

    private readonly List<Station> _stations;

    public StationService(IWebHostEnvironment env)
    {
        var dataFilePath = Path.Combine(env.ContentRootPath, "Data", "dublinbike.json");

        if (!File.Exists(dataFilePath))
        {
            _stations = new List<Station>();
            return;
        }

        var json = File.ReadAllText(dataFilePath);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var stations = JsonSerializer.Deserialize<List<Station>>(json, options);

        _stations = stations ?? new List<Station>();
    }

    public IReadOnlyList<Station> GetStations(
        string? status,
        int? minBikes,
        string? search,
        string? sortBy,
        string? sortDir,
        int? page,
        int? pageSize)
    {
        IEnumerable<Station> query = _stations;

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(s =>
                !string.IsNullOrEmpty(s.status) &&
                string.Equals(s.status, status, StringComparison.OrdinalIgnoreCase));
        }

        if (minBikes.HasValue)
        {
            query = query.Where(s => s.available_bikes >= minBikes.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                (!string.IsNullOrEmpty(s.name) &&
                 s.name.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(s.address) &&
                 s.address.Contains(search, StringComparison.OrdinalIgnoreCase)));
        }

        var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);

        switch (sortBy?.ToLowerInvariant())
        {
            case "availablebikes":
                query = desc
                    ? query.OrderByDescending(s => s.available_bikes)
                    : query.OrderBy(s => s.available_bikes);
                break;
            case "bikestands":
                query = desc
                    ? query.OrderByDescending(s => s.bike_stands)
                    : query.OrderBy(s => s.bike_stands);
                break;
            case "name":
            default:
                query = desc
                    ? query.OrderByDescending(s => s.name)
                    : query.OrderBy(s => s.name);
                break;
        }

        var effectivePage = page.GetValueOrDefault(1);
        var effectivePageSize = pageSize.GetValueOrDefault(20);

        if (effectivePage <= 0) effectivePage = 1;
        if (effectivePageSize <= 0) effectivePageSize = 20;

        query = query
            .Skip((effectivePage - 1) * effectivePageSize)
            .Take(effectivePageSize);

        return query.ToList();
    }

    public Station? GetStationByNumber(int number)
    {
        return _stations.FirstOrDefault(s => s.number == number);
    }

    public Station AddStation(Station station)
    {
        if (_stations.Any(s => s.number == station.number))
        {
            throw new InvalidOperationException($"Station with number {station.number} already exists.");
        }

        if (station.bike_stands < 0 || station.available_bikes < 0 || station.available_bike_stands < 0)
        {
            throw new InvalidOperationException("Negative values are not allowed.");
        }

        if (station.available_bikes + station.available_bike_stands != station.bike_stands)
        {
            throw new InvalidOperationException("available_bikes + available_bike_stands must equal bike_stands.");
        }

        _stations.Add(station);
        return station;
    }

    public Station? UpdateStation(int number, Station updatedStation)
    {
        var existing = _stations.FirstOrDefault(s => s.number == number);
        if (existing is null)
        {
            return null;
        }

        if (updatedStation.bike_stands < 0 || updatedStation.available_bikes < 0 || updatedStation.available_bike_stands < 0)
        {
            throw new InvalidOperationException("Negative values are not allowed.");
        }

        if (updatedStation.available_bikes + updatedStation.available_bike_stands != updatedStation.bike_stands)
        {
            throw new InvalidOperationException("available_bikes + available_bike_stands must equal bike_stands.");
        }

        existing.contract_name = updatedStation.contract_name;
        existing.name = updatedStation.name;
        existing.address = updatedStation.address;
        existing.position = updatedStation.position;
        existing.banking = updatedStation.banking;
        existing.bonus = updatedStation.bonus;
        existing.bike_stands = updatedStation.bike_stands;
        existing.available_bike_stands = updatedStation.available_bike_stands;
        existing.available_bikes = updatedStation.available_bikes;
        existing.status = updatedStation.status;
        existing.last_update = updatedStation.last_update;

        return existing;
    }

    public void ApplyRandomUpdates()
    {
        var random = new Random();

        foreach (var s in _stations)
        {
            if (s.bike_stands <= 0)
            {
                s.available_bikes = 0;
                s.available_bike_stands = 0;
                continue;
            }

            var availableBikes = random.Next(0, s.bike_stands + 1);
            var availableBikeStands = s.bike_stands - availableBikes;

            s.available_bikes = availableBikes;
            s.available_bike_stands = availableBikeStands;
            s.last_update = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }


}
