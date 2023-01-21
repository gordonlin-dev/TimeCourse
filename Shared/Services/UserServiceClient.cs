using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface IUserServiceClient
    {
        public Task<List<string>> GetUserOrganizations(string token, string userId);
        public Task<List<User>> GetUserProfiles(string token, List<string> userIds);
        public Task<List<string>> GetUserAppointmentsByMonth(string token, int year, int month);

        public Task<List<string>> GetUserAppointmentsByDate(string token, int year, int month, int day);
    }

    public class UserServiceClient : IUserServiceClient
    {
        private HttpClient _httpClient { get; set; }

        public UserServiceClient(HttpClient httpClient, IOptions<APIOptions.UserApi> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        }

        public async Task<List<string>> GetUserOrganizations(string token, string userId)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, "User/Organizations");
            JWTHelper.AddTokenToHttpRequestMessage(token, msg);
            var result = await _httpClient.SendAsync(msg);
            var content = await result.Content.ReadAsStringAsync();
            var orgIds = JsonConvert.DeserializeObject<List<string>>(content);
            return orgIds;
        }

        public async Task<List<User>> GetUserProfiles(string token, List<string> userIds)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("User/Query?");
            for(int i = 0; i < userIds.Count; i++)
            {
                stringBuilder.Append("ids=" + userIds.ElementAt(i));
                if(i != userIds.Count - 1)
                {
                    stringBuilder.Append("&");
                }
            }
            var msg = new HttpRequestMessage(HttpMethod.Get, stringBuilder.ToString());
            JWTHelper.AddTokenToHttpRequestMessage(token, msg);
            var result = await _httpClient.SendAsync(msg);
            var content = await result.Content.ReadAsStringAsync();
            var profiles = JsonConvert.DeserializeObject<List<User>>(content);
            return profiles;
        }

        public async Task<List<string>> GetUserAppointmentsByMonth(string token, int year, int month)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, string.Format("User/Appointments/Year/{0}/Month/{1}", year, month));
            JWTHelper.AddTokenToHttpRequestMessage(token, msg);
            var result = await _httpClient.SendAsync(msg);
            var content = await result.Content.ReadAsStringAsync();
            var appointmentIds = JsonConvert.DeserializeObject<List<string>>(content);
            return appointmentIds;
        }

        public async Task<List<string>> GetUserAppointmentsByDate(string token, int year, int month, int day)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, string.Format("User/Appointments/Year/{0}/Month/{1}/Day/{2}", year, month, day));
            JWTHelper.AddTokenToHttpRequestMessage(token, msg);
            var result = await _httpClient.SendAsync(msg);
            var content = await result.Content.ReadAsStringAsync();
            var appointmentIds = JsonConvert.DeserializeObject<List<string>>(content);
            return appointmentIds;
        }
    }
}
