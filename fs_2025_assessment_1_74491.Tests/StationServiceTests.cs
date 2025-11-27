using System;
using fs_2025_assessment_1_74491.Models;
using fs_2025_assessment_1_74491.Services;
using Xunit;

namespace fs_2025_assessment_1_74491.Tests
{
    public class StationServiceTests
    {
        private StationService CreateService()
        {
            // Fake environment pointing to test bin folder
            var env = new FakeWebHostEnvironment();
            return new StationService(env);
        }

        [Fact]
        public void AddStation_Adds_New_Station_When_Number_Does_Not_Exist()
        {
            // Arrange
            var service = CreateService();
            var newNumber = 9999;

            // Make sure it does not exist yet
            var before = service.GetStationByNumber(newNumber);
            Assert.Null(before);

            var station = new Station
            {
                number = newNumber,
                contract_name = "dublin",
                name = "Test Station 9999",
                address = "Test Address",
                position = new GeoPosition { lat = 53.35f, lng = -6.26f },
                banking = false,
                bonus = false,
                bike_stands = 20,
                available_bike_stands = 10,
                available_bikes = 10,
                status = "OPEN",
                last_update = 1729065138000
            };

            // Act
            var created = service.AddStation(station);

            // Assert
            Assert.NotNull(created);
            Assert.Equal(newNumber, created.number);

            var after = service.GetStationByNumber(newNumber);
            Assert.NotNull(after);
            Assert.Equal("Test Station 9999", after!.name);
        }

        [Fact]
        public void AddStation_Throws_When_Number_Already_Exists()
        {
            // Arrange
            var service = CreateService();

            // assume number 1 already exists in the JSON
            var existing = service.GetStationByNumber(1);
            Assert.NotNull(existing); // sanity check

            var duplicate = new Station
            {
                number = 1,
                contract_name = "dublin",
                name = "Duplicate",
                address = "Duplicate",
                position = new GeoPosition { lat = 0, lng = 0 },
                banking = false,
                bonus = false,
                bike_stands = 10,
                available_bike_stands = 5,
                available_bikes = 5,
                status = "OPEN",
                last_update = 1729065138000
            };

            // Act + Assert
            Assert.Throws<InvalidOperationException>(() => service.AddStation(duplicate));
        }

        [Fact]
        public void UpdateStation_Changes_Fields_When_Valid()
        {
            
            var service = CreateService();

            
            var original = service.GetStationByNumber(1);
            Assert.NotNull(original);

            var updated = new Station
            {
                number = 1,
                contract_name = original!.contract_name,
                name = "Updated Name",
                address = original.address,
                position = original.position,
                banking = original.banking,
                bonus = original.bonus,
                bike_stands = original.bike_stands,
                available_bike_stands = original.available_bike_stands,
                available_bikes = original.available_bikes,
                status = original.status,
                last_update = original.last_update
            };

            
            var result = service.UpdateStation(1, updated);

            Assert.NotNull(result);
            Assert.Equal(1, result!.number);
            Assert.Equal("Updated Name", result.name);   
        }




        [Fact]
        public void GetSummary_Returns_Positive_Totals()
        {
            // Arrange
            var service = CreateService();

            // Act
            var summary = service.GetSummary();

            // Assert
            Assert.True(summary.TotalStations > 0);
            Assert.True(summary.TotalBikeStands >= summary.TotalAvailableBikes);
            Assert.True(summary.OpenStations + summary.ClosedStations <= summary.TotalStations);
        }
    }
}
