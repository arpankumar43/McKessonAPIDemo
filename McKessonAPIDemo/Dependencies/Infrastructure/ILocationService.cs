using McKessonAPIDemo.Dependencies.Models;

namespace McKessonAPIDemo.Dependencies.Infrastructure
{
    public interface ILocationService
    {
        IEnumerable<Location> GetAvailableLocations(DateTime startTime, DateTime endTime);
        Location SaveLocation(Location location);
        IEnumerable<Location> GetAllLocations();
    }
}
