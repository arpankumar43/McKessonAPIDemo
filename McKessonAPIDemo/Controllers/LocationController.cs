using McKessonAPIDemo.Dependencies.Infrastructure;
using McKessonAPIDemo.Dependencies.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace McKessonAPIDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }


        [HttpGet("AvailableLocation")]
        public IActionResult GetAvailableLocations()
        {
            var startTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0);
            var endTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 13, 0, 0);

            var availableLocations = _locationService.GetAvailableLocations(startTime, endTime);

            return Ok(availableLocations);
        }


        [HttpPost("AddLocation")]
        public IActionResult CreateLocation([FromBody] Location location)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Perform any necessary validation or processing on the location object
                // ...

                // Save the location object to a data store (e.g., database)
               var result = _locationService.SaveLocation(location);

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, $"An error occurred while creating the location: {ex.Message}");
            }
        }
    }
}
