using Client.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Model;
using System.Net.Http.Headers;

namespace Client.Services
{
    public interface IOrganizationService
    {
        public void SetBearerToken(string token);

        public Task<List<Organization>> GetOrganizationsAsync();

        public Task<string> CreateOrganizationAsync(Organization organization);

        public Task JoinOrganizationAsync(string invitationCode);

        public Task<List<User>> GetMembersAsync(string orgId);

        public Task<Organization> RemoveMemberFromOrganization(string orgId, string memberId);
        public Task UpdateOrganization(Organization organization);
    }
    public class OrganizationService : IOrganizationService
    {
        private HttpClient _httpClient;
        private readonly IUserService _userService;
        public OrganizationService(HttpClient httpClient, 
            IOptions<OrganizationApi> options,
            IUserService userService)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
            _userService = userService;
        }

        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            var response = await _httpClient.GetStringAsync("Organizations/User");
            return JsonConvert.DeserializeObject<List<Organization>>(response);
        }
        public async Task<string> CreateOrganizationAsync(Organization organization)
        {
            var response = await _httpClient.PostAsJsonAsync("Organizations", organization);
            return response.ToString();
        }

        public async Task JoinOrganizationAsync(string invitationCode)
        {
            var response = await _httpClient.PostAsJsonAsync("Organizations/User", invitationCode);
        }
        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<List<User>> GetMembersAsync(string orgId)
        {
            var response = await _httpClient.GetAsync("Organizations/" + orgId + "/Members");
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<User>>(content);
        }

        public async Task<Organization> RemoveMemberFromOrganization(string orgId, string memberId)
        {
            var response = await _httpClient.DeleteAsync("Organizations/" + orgId + "/Member/" + memberId);
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Organization>(content);
        }

        public async Task UpdateOrganization(Organization organization)
        {
            var result = await _httpClient.PutAsJsonAsync("Organizations", organization);
        }
    }
}
