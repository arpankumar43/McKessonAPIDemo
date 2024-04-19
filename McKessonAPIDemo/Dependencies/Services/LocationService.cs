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
        private readonly string _csvFilePath;
        public LocationService(string csvFilePath)
        {
            _csvFilePath = csvFilePath;
        }

        public IEnumerable<Location> GetAllLocations()
        {

            return ReadLocationsFromCsv();
        }
        public IEnumerable<Location> GetAvailableLocations(DateTime startTime, DateTime endTime)
        {
            var locations = ReadLocationsFromCsv();
            return FilterLocations(locations, startTime, endTime);
        }

        static IEnumerable<Location> FilterLocations(IEnumerable<Location> locations, DateTime targetStartTime, DateTime targetEndTime)
        {
            return locations.Where(location =>
            {
                DateTime openTime = DateTime.ParseExact(location.OpenTime.ToLongTimeString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                DateTime closeTime = DateTime.ParseExact(location.CloseTime.ToLongTimeString(), "HH:mm:ss", CultureInfo.InvariantCulture);

                return openTime <= targetStartTime && closeTime >= targetEndTime;
            });
        }
        private bool IsLocationAvailable(Location location, DateTime startTime, DateTime endTime)
        {

            var openingTime = location.OpenTime;
            var closingTime = location.CloseTime;

            // Check if the location's opening and closing times are between 10 AM and 1 PM (inclusive)
            return (openingTime >= startTime && openingTime < endTime) &&
                   (closingTime > startTime && closingTime <= endTime);

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
