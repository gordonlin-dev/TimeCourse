using Newtonsoft.Json;
using Shared.Model;
using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentTopicListener
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
            var userAppointmentsMessage = JsonConvert.DeserializeObject<UserAppointmentMessage>(message);
            if(userAppointmentsMessage.ActionMode == UserAppointmentMessage.Mode.Add)
            {
                await _userService.AddAppointments(userAppointmentsMessage.UserId, userAppointmentsMessage.Appointments);
            }
            else
            {
                await _userService.RemoveAppointments(userAppointmentsMessage.UserId, userAppointmentsMessage.Appointments);
            }
        }
    }
}
