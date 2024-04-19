using CsvHelper;
using McKessonAPIDemo.Dependencies.Infrastructure;
using McKessonAPIDemo.Dependencies.Models;
using System.Formats.Asn1;
using System.Globalization;
using System;

namespace McKessonAPIDemo.Dependencies.Services
{
    public class LocationService : ILocationService
    {
        //private static List<Location> _locations;
        private readonly string _csvFilePath;
        public LocationService(string csvFilePath)
        {
            //_locations = LoadLocationsFromCsv();
            _csvFilePath = csvFilePath;
        }


        public IEnumerable<Location> GetAvailableLocations(DateTime startTime, DateTime endTime)
        {
            var locations = ReadLocationsFromCsv();
            return locations.Where(l => IsLocationAvailable(l, startTime, endTime));
        }

        private bool IsLocationAvailable(Location location, DateTime startTime, DateTime endTime)
        {
            var openingTime = location.OpenTime;
            var closingTime = location.CloseTime;
            //var openingTime = location.OpenTime.TimeOfDay;
            //var closingTime = location.CloseTime.TimeOfDay;

            //// Check if the location's opening time is between the start and end times
            //var openingTimeInRange = openingTime >= startTime.TimeOfDay && openingTime < endTime.TimeOfDay;

            //// Check if the location's closing time is between the start and end times
            //var closingTimeInRange = closingTime > startTime.TimeOfDay && closingTime <= endTime.TimeOfDay;

            //// Check if the location's opening and closing times overlap with the start and end times
            //var overlappingTimes = openingTime < startTime.TimeOfDay && closingTime > endTime.TimeOfDay;
            return (openingTime >= startTime && openingTime < endTime) ||
          (closingTime > startTime && closingTime <= endTime);
            //return openingTime >= startTime && closingTime <= endTime;
            //return openingTimeInRange || closingTimeInRange || overlappingTimes;
        }

        private IEnumerable<Location> ReadLocationsFromCsv()
        {
            var locations = new List<Location>();

            using (var reader = new StreamReader(_csvFilePath))
            {
                // Skip the header row
                reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    locations.Add(new Location
                    {
                        Name = values[0],
                        OpenTime = DateTime.Parse(values[1]),
                        CloseTime = DateTime.Parse(values[2])
                    });
                }
            }

            return locations;
        }


        public Location SaveLocation(Location location)
        {
            var locations = ReadLocationsFromCsv().ToList();

            // Check if the location already exists (e.g., by comparing the name)
            var existingLocation = locations.FirstOrDefault(l => l.Name == location.Name);

            if (existingLocation != null)
            {
                // Update the existing location
                existingLocation.OpenTime = location.OpenTime;
                existingLocation.CloseTime = location.CloseTime;
            }
            else
            {
                // Add the new location
                locations.Add(location);
            }

            // Write the updated locations back to the CSV file
            WriteLocationsToCsv(locations);
            return location;
        }

        private void WriteLocationsToCsv(IEnumerable<Location> locations)
        {
            using (var writer = new StreamWriter(_csvFilePath))
            {
                // Write the header row
                writer.WriteLine("Name,OpenTime,CloseTime");

                foreach (var location in locations)
                {
                    writer.WriteLine($"{location.Name},{location.OpenTime},{location.CloseTime}");
                }
            }
        }

    }
}
