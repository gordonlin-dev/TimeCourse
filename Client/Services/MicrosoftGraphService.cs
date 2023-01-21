using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Shared.Model;

namespace Client.Services
{
    public interface IMicrosoftGraphService
    {
        public Task<List<Appointment>> SyncSchedule(int month, string timezone = "Eastern Standard Time");
        public Task<List<Appointment>> SyncSchedule(DateTime start, DateTime end, string timezone = "Eastern Standard Time");

        public Task CreateCalendarEvent(List<Appointment> appointments);

        public Task DeleteCalendarEvent(Appointment appointment);
    }
    public class MicrosoftGraphService : IMicrosoftGraphService
    {
        private GraphServiceClient _msGraphServiceClient;
        private MicrosoftIdentityConsentAndConditionalAccessHandler _consentHandler;
        private AuthenticationStateProvider _authenticationStateProvider;
        private IAppointmentService _appointmentService;
        public MicrosoftGraphService(GraphServiceClient msGraphServiceClient, 
            MicrosoftIdentityConsentAndConditionalAccessHandler consentHandler,
            AuthenticationStateProvider authenticationStateProvider,
            IAppointmentService appointmentService)
        {
            _msGraphServiceClient = msGraphServiceClient;
            _consentHandler = consentHandler;
            _authenticationStateProvider = authenticationStateProvider;
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="month">month from DateTime.Month (i.e January = 1)</param>
        /// <returns></returns>
        private async Task<IEnumerable<ScheduleItem>> GetSchedule(int month, string timezone = "Eastern Standard Time")
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var userEmail = authState.User.Claims.First(x => x.Type == "email").Value;

            var startDate = new DateTime(DateTime.UtcNow.Year, month, 1);
            try
            {
                var schedule = await _msGraphServiceClient.Me.Calendar.GetSchedule(
                    new List<string>() {
                        userEmail
                    },
                    new DateTimeTimeZone()
                    {
                        DateTime = startDate.AddMonths(1).ToString(),
                        TimeZone = timezone
                    },
                    new DateTimeTimeZone()
                    {
                        DateTime = startDate.ToString(),
                        TimeZone = timezone
                    }, 60)
                    .Request().Header("Prefer", "outlook.timezone=\"Eastern Standard Time\"").PostAsync();
                return schedule.First().ScheduleItems;
            }
            catch(Exception ex)
            {
                _consentHandler.HandleException(ex);
                return null;
            }
            
        }

        public async Task<List<Appointment>> SyncSchedule(int month, string timezone = "Eastern Standard Time")
        {
            var schedule = await GetSchedule(month, timezone);

            var newAppointments = new List<Appointment>();
            foreach(var item in schedule)
            {
                if (item.Location != "TimeCourse")
                {
                    DateTime.TryParse(item.Start.DateTime, out DateTime startTime);
                    var startTimeUTC = TimeZoneInfo.ConvertTimeToUtc(startTime, TimeZoneInfo.FindSystemTimeZoneById(item.Start.TimeZone));
                    DateTime.TryParse(item.End.DateTime, out DateTime endTime);
                    var endTimeUTC = TimeZoneInfo.ConvertTimeToUtc(endTime, TimeZoneInfo.FindSystemTimeZoneById(item.End.TimeZone));
                    newAppointments.Add(new Appointment()
                    {
                        StartTime = startTimeUTC,
                        EndTime = endTimeUTC,
                        Description = item.Subject,
                        AppointmentType = AppointmentType.ScheduleItem
                    });
                }            
            }
            await _appointmentService.CreateAppointments(newAppointments);
            return newAppointments;
        }

        private async Task<IEnumerable<ScheduleItem>> GetSchedule(DateTime start, DateTime end, string timezone = "Eastern Standard Time")
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var userEmail = authState.User.Claims.First(x => x.Type == "email").Value;
            try
            {
                var schedule = await _msGraphServiceClient.Me.Calendar.GetSchedule(
                    new List<string>() {
                        userEmail
                    },
                    new DateTimeTimeZone()
                    {
                        DateTime = end.ToString(),
                        TimeZone = timezone
                    },
                    new DateTimeTimeZone()
                    {
                        DateTime = start.ToString(),
                        TimeZone = timezone
                    }, 60)
                    .Request().Header("Prefer", "outlook.timezone=\"Eastern Standard Time\"").PostAsync();
                return schedule.First().ScheduleItems;
            }
            catch (Exception ex)
            {
                _consentHandler.HandleException(ex);
                return null;
            }

        }

        public async Task<List<Appointment>> SyncSchedule(DateTime start, DateTime end, string timezone = "Eastern Standard Time")
        {
            var schedule = await GetSchedule(start, end, timezone);
            var newAppointments = new List<Appointment>();
            foreach (var item in schedule)
            {
                if(item.Location != "TimeCourse")
                {
                    DateTime.TryParse(item.Start.DateTime, out DateTime startTime);
                    var startTimeUTC = TimeZoneInfo.ConvertTimeToUtc(startTime, TimeZoneInfo.FindSystemTimeZoneById(item.Start.TimeZone));
                    DateTime.TryParse(item.End.DateTime, out DateTime endTime);
                    var endTimeUTC = TimeZoneInfo.ConvertTimeToUtc(endTime, TimeZoneInfo.FindSystemTimeZoneById(item.End.TimeZone));
                    newAppointments.Add(new Appointment()
                    {
                        StartTime = startTimeUTC,
                        EndTime = endTimeUTC,
                        Description = item.Subject,
                        AppointmentType = AppointmentType.ScheduleItem
                    });
                }             
            }
            await _appointmentService.CreateAppointments(newAppointments);
            return newAppointments;
        }

        public async Task CreateCalendarEvent(List<Appointment> appointments)
        {
            foreach(var appointment in appointments)
            {
                if (appointment.AppointmentType == AppointmentType.Appointment && appointment.Calendar == null)
                {
                    var calendarEvent = new Event()
                    {
                        Subject = appointment.Description,
                        Start = new DateTimeTimeZone()
                        {
                            DateTime = appointment.StartTime.ToString(),
                            TimeZone = "Eastern Standard Time"
                        },
                        End = new DateTimeTimeZone()
                        {
                            DateTime = appointment.EndTime.ToString(),
                            TimeZone = "Eastern Standard Time"
                        },
                        Location = new Location()
                        {
                            DisplayName = "TimeCourse"
                        }
                    };
                    var createdEvent = await _msGraphServiceClient.Me.Calendar.Events.Request().AddAsync(calendarEvent);
                    appointment.Calendar = new Shared.Model.Calendar()
                    {
                        Type = CalendarType.Outlook,
                        EventId = createdEvent.Id
                    };
                }
            }
            await _appointmentService.UpdateAppointments(appointments);
        }
        public async Task DeleteCalendarEvent(Appointment appointment)
        {
            await _msGraphServiceClient.Me.Events[appointment.Calendar.EventId].Request().DeleteAsync();
        }
    }
}
