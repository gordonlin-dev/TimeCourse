using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class Availability
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string HostId { get; set; }
        public string HostOrganizationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }
        public int AppointmentDuration { get; set; }
        public AvailabilityRecurrance? AvailabilityRecurrance { get; set; }
        public DateTime? RecurranceEndDate { get; set; }

        public Guid RecurranceId { get; set; }
        public List<Appointment> Appointments { get; set; }
    }

    public enum AvailabilityRecurrance
    {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
    }
}
