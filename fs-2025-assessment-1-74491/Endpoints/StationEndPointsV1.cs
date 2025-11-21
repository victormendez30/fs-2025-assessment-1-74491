using fs_2025_assessment_1_74491.Models;
using fs_2025_assessment_1_74491.Services;

namespace fs_2025_assessment_1_74491.Endpoints;

public static class StationEndPointsV1
{
    public static void AddStationEndPointsV1(this WebApplication app)
    {
        var v1 = app.MapGroup("/api/v1/stations");

        v1.MapGet("/", (
            IStationService stationService,
            string? status,
            int? minBikes,
            string? search,
            string? sortBy,
            string? sortDir,
            int? page,
            int? pageSize) =>
        {
            var stations = stationService.GetStations(
                status,
                minBikes,
                search,
                sortBy,
                sortDir,
                page,
                pageSize);

            return Results.Ok(stations);
        });

        v1.MapGet("/{number:int}", (IStationService stationService, int number) =>
        {
            var station = stationService.GetStationByNumber(number);
            if (station is null) return Results.NotFound();
            return Results.Ok(station);
        });

        v1.MapGet("/summary", (IStationService stationService) =>
        {
            var summary = stationService.GetSummary();
            return Results.Ok(summary);
        });

        v1.MapPost("/", (IStationService stationService, Station station) =>
        {
            try
            {
                var created = stationService.AddStation(station);
                return Results.Created($"/api/v1/stations/{created.number}", created);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        });

        v1.MapPut("/{number:int}", (
            IStationService stationService,
            int number,
            Station station) =>
        {
            if (number != station.number)
            {
                return Results.BadRequest("Route number and body number must match.");
            }

            try
            {
                var updated = stationService.UpdateStation(number, station);
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
