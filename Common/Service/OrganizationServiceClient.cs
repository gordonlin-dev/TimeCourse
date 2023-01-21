using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Service
{
    public interface IOrganizationServiceClient
    {
        public Task<List<Model.Organization>> GetOrganizationById(List<string> orgIds);
    }
    public class OrganizationServiceClient : IOrganizationServiceClient
    {
        private HttpClient _httpClient { get; set; }
        public OrganizationServiceClient(HttpClient httpClient, IOptions<APIOptions.OrganizationApi> options)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(options.Value.BaseUrl);
        }

        public async Task<List<Model.Organization>> GetOrganizationById(List<string> orgIds)
        {
            var builder = new StringBuilder();
            foreach(var id in orgIds)
            {
                builder.Append(id);
                builder.Append(",");
            }
            builder.Remove(builder.Length - 1, 1);
            var msg = new HttpRequestMessage(HttpMethod.Get,"Organizations?ids=" + builder.ToString());
            var result = await _httpClient.SendAsync(msg);
            var orgs = JsonConvert.DeserializeObject<List<Model.Organization>>(await result.Content.ReadAsStringAsync());
            return orgs;
        }
    }
}
