using System;

namespace fs_2025_assessment_1_74491.Models;

public class Station
{
    public string id => number.ToString();
    public int number { get; set; }
    public string contract_name { get; set; }
    public string name { get; set; }
    public string address { get; set; }
    public GeoPosition position { get; set; }
    public bool banking { get; set; }
    public bool bonus { get; set; }
    public int bike_stands { get; set; }
    public int available_bike_stands { get; set; }
    public int available_bikes { get; set; }
    public string status { get; set; }
    public long last_update { get; set; }

    public string last_update_utc
    {
        get
        {
            var utc = DateTimeOffset.FromUnixTimeMilliseconds(last_update).UtcDateTime;
            return utc.ToString("yyyy-MM-dd HH:mm:ss 'UTC'");
        }
    }

    public string last_update_local
    {
        get
        {
            var utc = DateTimeOffset.FromUnixTimeMilliseconds(last_update).UtcDateTime;

            try
            {
                var tz = TimeZoneInfo.FindSystemTimeZoneById("Europe/Dublin");
                var local = TimeZoneInfo.ConvertTimeFromUtc(utc, tz);
                return local.ToString("yyyy-MM-dd HH:mm:ss 'Dublin'");
            }
            catch
            {
                return utc.ToString("yyyy-MM-dd HH:mm:ss 'Dublin'");
            }
        }
    }


    public string occupancy
    {
        get
        {
            if (bike_stands <= 0) return "0%";
            var percent = ((double)available_bikes / bike_stands) * 100;
            return percent.ToString("0.0") + "%";
        }
    }

}
