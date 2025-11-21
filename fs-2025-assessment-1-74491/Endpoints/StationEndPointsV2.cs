using fs_2025_assessment_1_74491.Models;
using fs_2025_assessment_1_74491.Services;
using Microsoft.Extensions.Caching.Memory;

namespace fs_2025_assessment_1_74491.Endpoints;

public static class StationEndPointsV2
{
    public static void AddStationEndPointsV2(this WebApplication app)
    {
        var v2 = app.MapGroup("/api/v2/stations");

        v2.MapGet("/", async (
            IMemoryCache cache,
            IStationServiceV2 stationService,
            string? status,
            int? minBikes,
            string? search,
            string? sortBy,
            string? sortDir,
            int? page,
            int? pageSize) =>
        {
            var cacheKey = $"stations_v2_{status}_{minBikes}_{search}_{sortBy}_{sortDir}_{page}_{pageSize}";

            if (!cache.TryGetValue(cacheKey, out IReadOnlyList<Station>? stations))
            {
                stations = await stationService.GetStationsAsync(
                    status,
                    minBikes,
                    search,
                    sortBy,
                    sortDir,
                    page,
                    pageSize);

                cache.Set(cacheKey, stations, TimeSpan.FromSeconds(30));
            }

            return Results.Ok(stations);
        });

        v2.MapGet("/{number:int}", async (
            IMemoryCache cache,
            IStationServiceV2 stationService,
            int number) =>
        {
            var cacheKey = $"station_v2_{number}";

            if (!cache.TryGetValue(cacheKey, out Station? station))
            {
                station = await stationService.GetStationByNumberAsync(number);
                if (station is null) return Results.NotFound();

                cache.Set(cacheKey, station, TimeSpan.FromSeconds(30));
            }

            return Results.Ok(station);
        });

        v2.MapGet("/summary", async (IStationServiceV2 stationService) =>
        {
            var summary = await stationService.GetSummaryAsync();
            return Results.Ok(summary);
        });


        v2.MapPost("/", async (
            IStationServiceV2 stationService,
            Station station) =>
        {
            try
            {
                var created = await stationService.AddStationAsync(station);
                return Results.Created($"/api/v2/stations/{created.number}", created);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        v2.MapPut("/{number:int}", async (
            IStationServiceV2 stationService,
            int number,
            Station station) =>
        {
            if (number != station.number)
            {
                return Results.BadRequest("Route number and body number must match.");
            }

            try
            {
                var updated = await stationService.UpdateStationAsync(number, station);
                if (updated is null) return Results.NotFound();
                return Results.Ok(updated);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });
    }
}
