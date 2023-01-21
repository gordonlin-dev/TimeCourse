using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Email { get; set; }
        public string Name { get; set; }

        //Ids of Appointments associated with the user grouped by month
        public List<UserAppointments> Appointments { get; set; }
        //Ids of Organizations associated with the user
        public List<string> Organizations { get; set; }
    }

    public class UserAppointments
    {
        public int Year { get; set; }
        public int Month { get; set; }

        public List<UserAppointmentItem> Appointments { get;set; }

    }

    public class UserAppointmentItem
    {
        public string Id { get; set; }  
        public int DayOfMonth { get; set; }
    }
}
