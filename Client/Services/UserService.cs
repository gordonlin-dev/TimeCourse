using Client.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Model;
using System.Net.Http.Headers;

namespace Client.Services
{
    public interface IUserService
    {
        public void SetBearerToken(string token);
        public Task<string> Auth(string token);

        public Task<User> GetProfile();

        public Task<IEnumerable<string>> GetUserOrganizationsAsync();
    }
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        public UserService(HttpClient httpClient, IOptions<UserApi> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);

        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<string> Auth(string token)
        {
            var response = await _httpClient.PostAsJsonAsync("Auth", token);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<IEnumerable<string>> GetUserOrganizationsAsync()
        {
            var response = await _httpClient.GetStringAsync("User/Organizations");
            return JsonConvert.DeserializeObject<List<string>>(response);
        }

        public async Task<User> GetProfile()
        {
            var response = await _httpClient.GetStringAsync("User");
            return JsonConvert.DeserializeObject<User>(response);
        }
    }
}
