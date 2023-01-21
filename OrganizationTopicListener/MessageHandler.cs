using Newtonsoft.Json;
using Shared.Model;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrganizationTopicListener
{
    public interface IMessageHandler
    {
        public Task HandleMessage(string message);
    }
    public class MessageHandler : IMessageHandler
    {
        private IUserService _userService;
        public MessageHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task HandleMessage(string message)
        {
            var userOrganizationMessage = JsonConvert.DeserializeObject<UserOrganizationMessage>(message);
            if(userOrganizationMessage.ActionMode == UserOrganizationMessage.Mode.Add)
            {
                await _userService.AddOrganization(userOrganizationMessage.UserId, userOrganizationMessage.OrgId);
            }
            else if(userOrganizationMessage.ActionMode == UserOrganizationMessage.Mode.Remove){
                await _userService.RemoveOrganization(userOrganizationMessage.UserId, userOrganizationMessage.OrgId);
            }
        }
    }
}
