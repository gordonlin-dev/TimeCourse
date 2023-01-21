using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared.Model;
using Shared.Services;

namespace User.API.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var user = await _userService.GetUserAsync(jwtModel.Id);
            return new JsonResult(user);
        }
        [HttpGet]
        [Route("Query")]
        public async Task<IActionResult> GetUsers([FromQuery] string[] ids)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var users = await _userService.GetUsersAsync(ids);
            return new OkObjectResult(users);
        }
        [HttpPut]
        public async Task<IActionResult> PutUser(Shared.Model.User user)
        {
            var result = await _userService.UpdateUserAsync(user);
            return new JsonResult(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            await _userService.RemoveUserAsync(userId);
            return new OkResult();
        }

        [HttpGet]
        [Route("Organizations")]
        public async Task<IActionResult> GetOrganizations()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var orgIds = await _userService.GetOrganizations(jwtModel.Id);
            return new OkObjectResult(orgIds);
        }

        [HttpGet]
        [Route("Appointments/Year/{year}/Month/{month}")]
        public async Task<IActionResult> GetAppointmentsByMonth(int year, int month)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var appointmentIds = await _userService.GetAppointmentIdsByMonth(jwtModel.Id, year, month);
            return new OkObjectResult(appointmentIds);
        }

        [HttpGet]
        [Route("Appointments/Year/{year}/Month/{month}/Day/{day}")]
        public async Task<IActionResult> GetAppointmentsByDate(int year, int month, int day)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var appointmentIds = await _userService.GetAppointmentIdsByDate(jwtModel.Id, year, month, day);
            return new OkObjectResult(appointmentIds);
        }
    }
}
