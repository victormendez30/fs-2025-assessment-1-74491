using fs_2025_assessment_1_74491.Models;

namespace fs_2025_assessment_1_74491.Services;

public interface IStationService
{
    IReadOnlyList<Station> GetStations(
        string? status,
        int? minBikes,
        string? search,
        string? sortBy,
        string? sortDir,
        int? page,
        int? pageSize);

    Station? GetStationByNumber(int number);
    Station AddStation(Station station);
    Station? UpdateStation(int number, Station updatedStation);
    void ApplyRandomUpdates();

    StationSummary GetSummary();
}
