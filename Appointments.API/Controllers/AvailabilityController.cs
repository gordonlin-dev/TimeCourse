using Appointments.API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Model;
using Shared.Services;

namespace Appointments.API.Controllers
{
    [Route("api/Availabilities")]
    [ApiController]
    public class AvailabilityController : ControllerBase
    {
        private IAvailabilityService _availabilityService;
        private IUserServiceClient _userServiceClient;

        public AvailabilityController(IAvailabilityService availabilityService, IUserServiceClient userServiceClient)
        {
            _availabilityService = availabilityService;
            _userServiceClient = userServiceClient;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAvailability()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var availability = JsonConvert.DeserializeObject<Availability>(body);
            await _availabilityService.CreateAvailability(jwtModel.Id, availability);
            return new OkResult();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAvailability()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var availability = JsonConvert.DeserializeObject<Availability>(body);
            await _availabilityService.UpdateAvailability(jwtModel.Id, availability);
            return new OkResult();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAvailability([FromRoute] string id)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            await _availabilityService.DeleteAvailability(jwtModel.Id, id);
            return new OkResult();
        }

        [Route("year/{year}/month/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetAvailabilitiesByMonth([FromRoute] int year, int month)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var token = JWTHelper.GetToken(Request);
            var userOrgs = await _userServiceClient.GetUserOrganizations(token, jwtModel.Id);
            var results = await _availabilityService.GetAvailabilitiesByMonth(jwtModel.Id, year, month);
            var organizationHosted = await _availabilityService.GetAvailabilitiesByMonth(jwtModel.Id, userOrgs, year, month);
            results.AddRange(organizationHosted);
            return new OkObjectResult(results);
        }

        [Route("year/{year}/month/{month}/day/{day}")]
        [HttpGet]
        public async Task<IActionResult> GetAvailabilitiesByDate([FromRoute] int year, int month, int day)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var token = JWTHelper.GetToken(Request);
            var userOrgs = await _userServiceClient.GetUserOrganizations(token, jwtModel.Id);
            var results = await _availabilityService.GetAvailabilitiesByDate(jwtModel.Id, year, month, day);
            var organizationHosted = await _availabilityService.GetAvailabilitiesByDate(jwtModel.Id, userOrgs, year, month, day);
            results.AddRange(organizationHosted);
            return new OkObjectResult(results);
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailabilitiesByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var token = JWTHelper.GetToken(Request);
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var userOrgs = await _userServiceClient.GetUserOrganizations(token, jwtModel.Id);
            var results = await _availabilityService.GetAvailabilitiesByDateRange(jwtModel.Id, start, end);
            var organizationHosted = await _availabilityService.GetAvailabilitiesByDateRange(jwtModel.Id, userOrgs, start, end);
            results.AddRange(organizationHosted);
            return new OkObjectResult(results);
        }
    }
}
