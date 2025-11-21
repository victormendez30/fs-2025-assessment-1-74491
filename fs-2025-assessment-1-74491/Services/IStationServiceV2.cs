using fs_2025_assessment_1_74491.Models;

namespace fs_2025_assessment_1_74491.Services;

public interface IStationServiceV2
{
    Task<IReadOnlyList<Station>> GetStationsAsync(
        string? status,
        int? minBikes,
        string? search,
        string? sortBy,
        string? sortDir,
        int? page,
        int? pageSize);

    Task<Station?> GetStationByNumberAsync(int number);
    Task<Station> AddStationAsync(Station station);
    Task<Station?> UpdateStationAsync(int number, Station updatedStation);
    Task<StationSummary> GetSummaryAsync();
}
