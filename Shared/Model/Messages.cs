using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class UserOrganizationMessage
    {
        public enum Mode{
            Add = 0,
            Remove = 1
        }
        public string UserId { get; set; }
        public string OrgId { get; set; }
        public Mode ActionMode { get; set; }
    }

    public class UserAppointmentMessage
    {
        public enum Mode
        {
            Add = 0,
            Remove = 1
        }
        public string UserId { get; set; }
        public List<Appointment> Appointments { get; set; }

        public Mode ActionMode { get; set; }
    }
}
