using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Common.Service
{
    public class APIOptions
    {
        public class UserApi
        {
            public string BaseUrl { get; set; }
        }

        public class AppointmentsApi
        {
            public string BaseUrl { get; set; }
        }
        public class OrganizationApi
        {
            public string BaseUrl { get; set; }
        }
    }

    public interface IUserServiceClient
    {
        public Task<int> JoinOrganization(string token, string orgId);
        public Task<int> RemoveOrganizationFromUser(string token, string userId, string orgId);
        public Task<List<string>> GetUserOrganizations(string token, string userId);
    }
    public class UserServiceClient : IUserServiceClient
    {
        private HttpClient _httpClient { get; set; }
        public UserServiceClient(HttpClient httpClient, IOptions<APIOptions.UserApi> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        }
        public async Task<int> JoinOrganization(string token, string orgId)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, "User/Organizations");
            Auth.JWTHelper.AddTokenToHttpRequestMessage(token, msg);
            var stringContent = new StringContent(orgId);
            msg.Content = stringContent;
            var result = await _httpClient.SendAsync(msg);
            return result.IsSuccessStatusCode ? 0 : 1;
        }
        public async Task<int> RemoveOrganizationFromUser(string token, string userId, string orgId)
        {
            var msg = new HttpRequestMessage(HttpMethod.Delete, "User/" + userId + "/Organizations/" + orgId);
            Auth.JWTHelper.AddTokenToHttpRequestMessage(token, msg);
            var result = await _httpClient.SendAsync(msg);
            return result.IsSuccessStatusCode ? 0 : 1;
        }

        public async Task<List<string>> GetUserOrganizations(string token, string userId)
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, "User/Organizations");
            Auth.JWTHelper.AddTokenToHttpRequestMessage(token, msg);
            var result = await _httpClient.SendAsync(msg);
            var content = await result.Content.ReadAsStringAsync();
            var orgIds = JsonConvert.DeserializeObject<List<string>>(content);
            return orgIds;
        }
    }
}
