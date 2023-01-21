using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public enum AppointmentType
    {
        ScheduleItem = 0,
        Appointment = 1
    }

    public enum CalendarType
    {
        Outlook = 0,
    }

    public class Calendar
    {
        public CalendarType Type { get; set; }
        public string EventId { get; set; }
    }
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string OwnerId { get; set; }
        //Start time of appointment, in UTC
        public DateTime StartTime { get; set; }
        //End time of appointment, in UTC
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public AppointmentType AppointmentType { get; set; }

        public string AvailabilityId { get; set; }
        public Calendar Calendar { get; set; }

    }
}
