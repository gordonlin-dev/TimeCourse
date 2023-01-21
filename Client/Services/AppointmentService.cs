using Client.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Model;
using System.Net.Http.Headers;

namespace Client.Services
{
    public interface IAppointmentService
    {
        public void SetBearerToken(string token);

        public Task CreateAppointments(List<Appointment> appointments);

        public Task<List<Appointment>> GetAppointments(int year, int month);
        public Task<List<Appointment>> GetAppointmentsByDateRange(DateTime start, DateTime end);
        public Task<List<Availability>> GetAvailabilities(int year, int month);
        public Task<List<Availability>> GetAvailabilities(DateTime day);
        public Task<List<Availability>> GetAvailabilitiesByDateRange(DateTime start, DateTime end);
        public Task<List<Appointment>> GetAppointments(DateTime day);

        public Task UpdateAppointments(List<Appointment> appointments);

        public Task DeleteAppointment(string appointmentId);

        public Task CreateAppointment(Appointment appointment);

        public Task CreateAvailability(Availability availability);

        public Task UpdateAvailability(Availability availability);

        public Task DeleteAvailability(string availabilityId);

    }
    public class AppointmentService : IAppointmentService
    {
        private readonly HttpClient _httpClient;

        public AppointmentService(HttpClient httpClient, IOptions<AppointmentApi> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        }

        public async Task CreateAppointments(List<Appointment> appointments)
        {
            await _httpClient.PostAsJsonAsync("Appointments", appointments);
        }

        public async Task<List<Appointment>> GetAppointmentsByDateRange(DateTime start, DateTime end)
        {
            var url = "appointments" + "?start=" + start.ToShortDateString() + "&" + "end=" + end.ToShortDateString();
            var response = await _httpClient.GetAsync(url);
            var appointments = JsonConvert.DeserializeObject<List<Appointment>>(await response.Content.ReadAsStringAsync());
            appointments.ForEach(x => x.StartTime = x.StartTime.ToLocalTime());
            appointments.ForEach(x => x.EndTime = x.EndTime.ToLocalTime());
            return appointments;
        }
        public async Task<List<Appointment>> GetAppointments(int year, int month)
        {
            var url = string.Format("appointments/year/{0}/month/{1}", year, month);
            var response = await _httpClient.GetAsync(url);
            var appointments = JsonConvert.DeserializeObject<List<Appointment>>(await response.Content.ReadAsStringAsync());
            return appointments;
        }

        public async Task<List<Appointment>> GetAppointments(DateTime day)
        {
            var url = string.Format("appointments/year/{0}/month/{1}/day/{2}", day.Year, day.Month, day.Day);
            var response = await _httpClient.GetAsync(url);
            var appointments = JsonConvert.DeserializeObject<List<Appointment>>(await response.Content.ReadAsStringAsync());
            return appointments;
        }

        public async Task DeleteAppointment(string appointmentId)
        {
            var url = string.Format("appointments/{0}", appointmentId);
            var response = await _httpClient.DeleteAsync(url);
        }

        public async Task<List<Availability>> GetAvailabilities(int year, int month)
        {
            var url = string.Format("Availabilities/year/{0}/month/{1}", year, month);
            var response = await _httpClient.GetAsync(url);
            var availabilities = JsonConvert.DeserializeObject<List<Availability>>(await response.Content.ReadAsStringAsync());
            return availabilities;
        }

        public async Task<List<Availability>> GetAvailabilities(DateTime day)
        {
            var url = string.Format("Availabilities/year/{0}/month/{1}/day/{2}", day.Year, day.Month, day.Day);
            var response = await _httpClient.GetAsync(url);
            var availabilities = JsonConvert.DeserializeObject<List<Availability>>(await response.Content.ReadAsStringAsync());
            return availabilities;
        }

        public async Task<List<Availability>> GetAvailabilitiesByDateRange(DateTime start, DateTime end)
        {
            var url = "Availabilities" + "?start=" + start.ToShortDateString() + "&" + "end=" + end.ToShortDateString();
            var response = await _httpClient.GetAsync(url);
            var availabilities = JsonConvert.DeserializeObject<List<Availability>>(await response.Content.ReadAsStringAsync());
            availabilities.ForEach(x => x.StartTime = x.StartTime.ToLocalTime());
            availabilities.ForEach(x => x.EndTime = x.EndTime.ToLocalTime());
            return availabilities;
        }

        public async Task CreateAppointment(Appointment appointment)
        {
            await _httpClient.PostAsJsonAsync("Appointments/Appointment", appointment);
        }
        public async Task UpdateAppointments(List<Appointment> appointments)
        {
            await _httpClient.PutAsJsonAsync("Appointments", appointments);
        }
        public async Task CreateAvailability(Availability availability)
        {
            await _httpClient.PostAsJsonAsync("Availabilities", availability);
        }

        public async Task UpdateAvailability(Availability availability)
        {
            await _httpClient.PutAsJsonAsync("Availabilities", availability);
        }

        public async Task DeleteAvailability(string availabilityId)
        {
            await _httpClient.DeleteAsync("Availabilities/" + availabilityId);
        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
