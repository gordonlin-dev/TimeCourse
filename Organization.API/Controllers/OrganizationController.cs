using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Organization.API.Services;
using Newtonsoft.Json;
using Shared.Services;
using Shared.Model;

namespace Organization.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private IOrganizationService _organizationService { get; set; }

        private IRabbitMqPublisher _rabbitMqPublisher { get; set; }

        private IUserServiceClient _userServiceClient { get; set; }

        public OrganizationsController(IOrganizationService organizationService,
            IRabbitMqPublisher rabbitMqPublisher,
            IUserServiceClient userServiceClient)
        {
            _organizationService = organizationService;
            _rabbitMqPublisher = rabbitMqPublisher;
            _userServiceClient = userServiceClient;
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrganization()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var org = JsonConvert.DeserializeObject<Shared.Model.Organization>(body);
            await _organizationService.CreateOrganization(jwtModel.Id, org);
            _rabbitMqPublisher.PublishMessage(new UserOrganizationMessage()
            {
                UserId = jwtModel.Id,
                OrgId = org.Id
            });
            return new OkResult();
        }

        [HttpGet]
        [Route("User")]
        public async Task<IActionResult> GetOrganizations()
        {
            var token = JWTHelper.GetToken(Request);
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var orgIds = await _userServiceClient.GetUserOrganizations(token, jwtModel.Id);
            var result = await _organizationService.GetOrganizations(jwtModel.Id, orgIds);
            return new OkObjectResult(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganizationsById([FromQuery]string ids)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var orgIds = ids.Split(",").ToList();
            var result = await _organizationService.GetOrganizations(jwtModel.Id, orgIds);
            return new OkObjectResult(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateOrganization()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var org = JsonConvert.DeserializeObject<Shared.Model.Organization>(body);
            await _organizationService.UpdateOrganization(jwtModel.Id, org);
            return new OkResult();
        }

        [HttpPost]
        [Route("User")]
        public async Task<IActionResult> JoinOrganizationByCode()
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var body = await new StreamReader(Request.Body).ReadToEndAsync();
            var code = JsonConvert.DeserializeObject<Guid>(body);
            var orgId = await _organizationService.JoinOrganization(jwtModel.Id, jwtModel.Name, code);
            if(orgId != string.Empty)
            {
                _rabbitMqPublisher.PublishMessage(new UserOrganizationMessage()
                {
                    UserId = jwtModel.Id,
                    OrgId = orgId,
                    ActionMode = UserOrganizationMessage.Mode.Add
                });
            }      
            return new OkResult();
        }

        [HttpDelete]
        [Route("{organizationId}/Member/{memberId}")]
        public async Task<IActionResult> RemoveUserFromOrganization([FromRoute] string organizationId, [FromRoute] string memberId)
        {
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var org = await _organizationService.RemoveUserFromOrganization(jwtModel.Id, organizationId, memberId);
            _rabbitMqPublisher.PublishMessage(new UserOrganizationMessage()
            {
                UserId = memberId,
                OrgId = organizationId,
                ActionMode = UserOrganizationMessage.Mode.Remove
            });
            return new OkObjectResult(org);
        }

        [HttpGet]
        [Route("{organizationId}/Members")]
        public async Task<IActionResult> GetOrganizationMembers([FromRoute]string organizationId)
        {
            var token = JWTHelper.GetToken(Request);
            JwtModel jwtModel = HttpContext.Items["jwtModel"] as JwtModel;
            var organization = await _organizationService.GetOrganizations(jwtModel.Id, new List<string>() { organizationId });
            var members = await _userServiceClient.GetUserProfiles(token, organization.First().Members);
            var results = new List<User>();
            foreach(var member in members)
            {
                results.Add(new User() { 
                    Email = member.Email,
                    Name = member.Name,
                    Id = member.Id
                });
            }
            return new OkObjectResult(results);
        }
    }
}
