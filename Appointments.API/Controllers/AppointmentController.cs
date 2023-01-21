using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Appointments.API.Services;
using Appointments.API.Model;
using Newtonsoft.Json;
using Shared.Model;
using Shared.Services;

namespace Appointments.API.Controllers
{
    [Route("api/Appointments")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private IAppointmentsService _appointmentsService;
        private IUserServiceClient _userServiceClient;
        private IRabbitMqPublisher _rabbitMqPublisher;
        private IAvailabilityService _availabilityService;

        public AppointmentController(IAppointmentsService appointmentsService,
            IAvailabilityService availabilityService,
            IRabbitMqPublisher rabbitMqPublisher,
            IUserServiceClient userServiceClient)
        {
            _appointmentsService = appointmentsService;
            _userServiceClient = userServiceClient;
            _rabbitMqPublisher = rabbitMqPublisher;
            _availabilityService = availabilityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAppointmentsByDateRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var token = JWTHelper.GetToken(Request);
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var result = await _appointmentsService.GetAppointmentsByDateRange(jwtModel.Id, start, end);
            return new OkObjectResult(result);
        }
        [HttpGet]
        [Route("year/{year}/month/{month}")]
        public async Task<IActionResult> GetAppointmentsByMonth([FromRoute] int year, int month)
        {
            var token = JWTHelper.GetToken(Request);
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var appointmentIds = await _userServiceClient.GetUserAppointmentsByMonth(token, year, month);
            var result = await _appointmentsService.GetAppointments(jwtModel.Id, appointmentIds);
            return new OkObjectResult(result);
        }

        [HttpGet]
        [Route("year/{year}/month/{month}/day/{day}")]
        public async Task<IActionResult> GetAppointmentsByDay([FromRoute] int year, int month, int day)
        {
            var token = JWTHelper.GetToken(Request);
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var appointmentIds = await _userServiceClient.GetUserAppointmentsByDate(token, year, month, day);
            var result = await _appointmentsService.GetAppointments(jwtModel.Id, appointmentIds);
            return new OkObjectResult(result);

        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointments()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var appointments = JsonConvert.DeserializeObject<List<Appointment>>(body);
            var createdAppointments = await _appointmentsService.CreateAppointments(jwtModel.Id, appointments);
            if (createdAppointments.Any())
            {
                _rabbitMqPublisher.PublishMessage(new UserAppointmentMessage()
                {
                    UserId = jwtModel.Id,
                    ActionMode = UserAppointmentMessage.Mode.Add,
                    Appointments = createdAppointments
                });
            }       
            return new OkResult();
        }

        [HttpPost]
        [Route("Appointment")]
        public async Task<IActionResult> CreateAppointment()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var appointment = JsonConvert.DeserializeObject<Appointment>(body);
            appointment.AppointmentType = AppointmentType.Appointment;
            appointment.OwnerId = jwtModel.Id;
            appointment.Id = null;
            var createdAppointments = await _appointmentsService.CreateAppointments(jwtModel.Id, new List<Appointment>() { appointment });
            await _availabilityService.UpdateAppointmentInAvailability(appointment.AvailabilityId, appointment);
            if (createdAppointments.Any())
            {
                _rabbitMqPublisher.PublishMessage(new UserAppointmentMessage()
                {
                    UserId = jwtModel.Id,
                    ActionMode = UserAppointmentMessage.Mode.Add,
                    Appointments = createdAppointments
                });
            }
            return new OkResult();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAppointment()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var appointments = JsonConvert.DeserializeObject<List<Appointment>>(body);
            await _appointmentsService.UpdateAppointment(jwtModel.Id, appointments);
            return new OkResult();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAppointment([FromRoute] string id)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var appointment = await _appointmentsService.GetAppointments(jwtModel.Id, new List<string>() { id });
            await _availabilityService.CancelAppointmentInAvailability(appointment.First().AvailabilityId, appointment.First());
            await _appointmentsService.DeleteAppointment(jwtModel.Id, id);
            return new OkResult();
        }
    }
}
