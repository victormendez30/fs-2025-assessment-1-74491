using System.Net;
using System.Text.Json;
using Microsoft.Azure.Cosmos;
using fs_2025_assessment_1_74491.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace fs_2025_assessment_1_74491.Services;

public class CosmosStationService : IStationServiceV2
{
    public async Task<StationSummary> GetSummaryAsync()
    {
        var allStations = await GetStationsAsync(
            status: null,
            minBikes: null,
            search: null,
            sortBy: null,
            sortDir: null,
            page: null,
            pageSize: null
        );

        return new StationSummary
        {
            TotalStations = allStations.Count,
            TotalBikeStands = allStations.Sum(s => s.bike_stands),
            TotalAvailableBikes = allStations.Sum(s => s.available_bikes),
            OpenStations = allStations.Count(s =>
                string.Equals(s.status, "OPEN", StringComparison.OrdinalIgnoreCase)),
            ClosedStations = allStations.Count(s =>
                string.Equals(s.status, "CLOSED", StringComparison.OrdinalIgnoreCase))
        };
    }

    private readonly CosmosClient _client;
    private readonly Container _container;

    public CosmosStationService(
        CosmosClient client,
        IConfiguration configuration,
        IWebHostEnvironment env)
    {
        _client = client;

        var dbName = configuration["CosmosDb:DatabaseName"] ?? "DublinBikesDb";
        var containerName = configuration["CosmosDb:ContainerName"] ?? "Stations";
        var partitionKeyPath = "/number";

        var databaseResponse = _client.CreateDatabaseIfNotExistsAsync(dbName)
                                      .GetAwaiter().GetResult();

        var containerResponse = databaseResponse.Database
            .CreateContainerIfNotExistsAsync(containerName, partitionKeyPath)
            .GetAwaiter().GetResult();

        _container = containerResponse.Container;

        SeedIfEmpty(env);
    }

    private void SeedIfEmpty(IWebHostEnvironment env)
    {
        var iterator = _container.GetItemQueryIterator<Station>("SELECT TOP 1 * FROM c");
        if (iterator.HasMoreResults)
        {
            var firstPage = iterator.ReadNextAsync().GetAwaiter().GetResult();
            if (firstPage.Count > 0) return;
        }

        var dataFilePath = Path.Combine(env.ContentRootPath, "Data", "dublinbike.json");
        if (!File.Exists(dataFilePath)) return;

        var json = File.ReadAllText(dataFilePath);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var stations = JsonSerializer.Deserialize<List<Station>>(json, options);
        if (stations is null || stations.Count == 0) return;

        var tasks = new List<Task>();
        foreach (var station in stations)
        {
            tasks.Add(_container.CreateItemAsync(station, new PartitionKey(station.number)));
        }

        Task.WhenAll(tasks).GetAwaiter().GetResult();
    }

    public async Task<IReadOnlyList<Station>> GetStationsAsync(
        string? status,
        int? minBikes,
        string? search,
        string? sortBy,
        string? sortDir,
        int? page,
        int? pageSize)
    {
        var sql = "SELECT * FROM c";
        var filters = new List<string>();
        var parameters = new Dictionary<string, object>();

        if (!string.IsNullOrWhiteSpace(status))
        {
            filters.Add("c.status = @status");
            parameters["@status"] = status;
        }

        if (minBikes.HasValue)
        {
            filters.Add("c.available_bikes >= @minBikes");
            parameters["@minBikes"] = minBikes.Value;
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            filters.Add("(CONTAINS(c.name, @search, true) OR CONTAINS(c.address, @search, true))");
            parameters["@search"] = search;
        }

        if (filters.Count > 0)
        {
            sql += " WHERE " + string.Join(" AND ", filters);
        }

        var desc = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
        var orderBy = sortBy?.ToLowerInvariant() switch
        {
            "availablebikes" => "c.available_bikes",
            "bikestands" => "c.bike_stands",
            "name" => "c.name",
            _ => "c.name"
        };
        sql += desc ? $" ORDER BY {orderBy} DESC" : $" ORDER BY {orderBy} ASC";

        var queryDef = new QueryDefinition(sql);
        foreach (var p in parameters)
        {
            queryDef.WithParameter(p.Key, p.Value);
        }

        var items = new List<Station>();
        var iterator = _container.GetItemQueryIterator<Station>(queryDef);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            items.AddRange(response);
        }

        if (page.HasValue || pageSize.HasValue)
        {
            var effectivePage = page.GetValueOrDefault(1);
            var effectivePageSize = pageSize.GetValueOrDefault(20);

            if (effectivePage <= 0) effectivePage = 1;
            if (effectivePageSize <= 0) effectivePageSize = 20;

            items = items
                .Skip((effectivePage - 1) * effectivePageSize)
                .Take(effectivePageSize)
                .ToList();
        }

        return items;

    }

    public async Task<Station?> GetStationByNumberAsync(int number)
    {
        try
        {
            var response = await _container.ReadItemAsync<Station>(
                number.ToString(),
                new PartitionKey(number));
            return response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Station> AddStationAsync(Station station)
    {
        var existing = await GetStationByNumberAsync(station.number);
        if (existing != null)
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

        var response = await _container.CreateItemAsync(station, new PartitionKey(station.number));
        return response.Resource;
    }

    public async Task<Station?> UpdateStationAsync(int number, Station updatedStation)
    {
        var existing = await GetStationByNumberAsync(number);
        if (existing is null) return null;

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

        var response = await _container.UpsertItemAsync(existing, new PartitionKey(existing.number));
        return response.Resource;
    }
}
