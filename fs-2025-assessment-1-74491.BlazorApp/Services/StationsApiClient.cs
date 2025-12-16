using fs_2025_assessment_1_74491.BlazorApp.Models;

namespace fs_2025_assessment_1_74491.BlazorApp.Services
{
    public class StationsApiClient
    {
        private readonly HttpClient _http;

        public StationsApiClient(HttpClient http)
        {
            _http = http;
        }

        
        public async Task<List<StationDto>> GetStationsAsync(
            string? status = null,
            int? minBikes = null,
            string? search = null,
            string? sortBy = null,
            string? sortDir = null,
            int? page = null,
            int? pageSize = null)
        {
            var query = new List<string>();

            if (!string.IsNullOrWhiteSpace(status))
                query.Add($"status={Uri.EscapeDataString(status)}");
            if (minBikes.HasValue)
                query.Add($"minBikes={minBikes.Value}");
            if (!string.IsNullOrWhiteSpace(search))
                query.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrWhiteSpace(sortBy))
                query.Add($"sortBy={Uri.EscapeDataString(sortBy)}");
            if (!string.IsNullOrWhiteSpace(sortDir))
                query.Add($"sortDir={Uri.EscapeDataString(sortDir)}");
            if (page.HasValue)
                query.Add($"page={page.Value}");
            if (pageSize.HasValue)
                query.Add($"pageSize={pageSize.Value}");

            var qs = query.Count > 0 ? "?" + string.Join("&", query) : string.Empty;

            var url = $"/api/v2/stations{qs}";
            var result = await _http.GetFromJsonAsync<List<StationDto>>(url);
            return result ?? new List<StationDto>();
        }

        
        public async Task<StationDto?> GetStationAsync(int number)
        {
            var url = $"/api/v2/stations/{number}";
            return await _http.GetFromJsonAsync<StationDto>(url);
        }

        
        public async Task<StationSummaryDto?> GetSummaryAsync()
        {
            var url = "/api/v2/stations/summary";
            return await _http.GetFromJsonAsync<StationSummaryDto>(url);
        }

        
        public async Task<StationDto?> CreateStationAsync(StationDto item)
        {
            var response = await _http.PostAsJsonAsync("/api/v2/stations", item);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StationDto>();
        }

        public async Task<StationDto?> UpdateStationAsync(int number, StationDto item)
        {
            var response = await _http.PutAsJsonAsync($"/api/v2/stations/{number}", item);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<StationDto>();
        }

        public async Task<bool> DeleteStationAsync(int number)
        {
            var response = await _http.DeleteAsync($"/api/v2/stations/{number}");

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            response.EnsureSuccessStatusCode();
            return true;
        }


    }
}
