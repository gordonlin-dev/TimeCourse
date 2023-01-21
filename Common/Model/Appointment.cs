using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Model
{
    public enum AppointmentType
    {
        ScheduleItem = 0,
        AppointmentBlock = 1
    }
    public interface IAppointment
    {
        public string? Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string AppointmentDescription { get; set; }

        public AppointmentType AppointmentType { get;}
    }

    public class AppointmentBlock : IAppointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        //User Id of the host for this appointment block
        public string HostId { get; set; }
        //Start time of appointment, in UTC
        public DateTime StartTime { get; set; }
        //End time of appointment, in UTC
        public DateTime EndTime { get; set; }
        public string AppointmentDescription { get; set; }
        //Maximum length of the appointment slots in minutes
        public int AppointmentSlotMaxDuration { get; set; }

        public List<AppointmentSlot> AppointmentSlots { get; set; }
        //The organization this appointment is under
        public string HostOrganizationId { get; set; }
        public AppointmentType AppointmentType { get; } = AppointmentType.AppointmentBlock;
    }

    public class AppointmentSlot {

        //User Id of the attendee
        public string AttendeeId { get; set; }
        //Duration of this appointment slot, in minutes
        public int SlotDuration { get; set; }
        //Start time of this appointment slot, in UTC
        public DateTime StartTime { get; set; }
        //End time of this appointment slot, in UTC
        public DateTime EndTime { get; set; }

    }
}
