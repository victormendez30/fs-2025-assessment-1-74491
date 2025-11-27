namespace fs_2025_assessment_1_74491.BlazorApp.Models
{
    public class StationDto
    {
        public int number { get; set; }
        public string? contract_name { get; set; }
        public string? name { get; set; }
        public string? address { get; set; }
        public PositionDto? position { get; set; }
        public bool banking { get; set; }
        public bool bonus { get; set; }
        public int bike_stands { get; set; }
        public int available_bike_stands { get; set; }
        public int available_bikes { get; set; }
        public string? status { get; set; }
        public long last_update { get; set; }
        public string? last_update_utc { get; set; }
        public string? last_update_local { get; set; }
        public string? occupancy { get; set; }
    }

    public class PositionDto
    {
        public float lat { get; set; }
        public float lng { get; set; }
    }
}
