using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model
{
    public static class DaysOfWeek
    {
        public const string Monday = "Monday";
        public const string Tuesday = "Tuesday";
        public const string Wednesday = "Wednesday";
        public const string Thursday = "Thursday";
        public const string Friday = "Friday";
    }

    public class WorkingDay
    {
        public string DayofWeek { get; set; }
        //In UTC
        public CustomTimeObject StartTime { get; set; }
        //In UTC
        public CustomTimeObject EndTime { get; set; }
    }
    public class Schedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        public string? UserId { get; set; }
        public List<WorkingDay> WorkingDays { get; set; }
        public List<ScheduleItem> ScheduleItems { get; set; }

    }

    public class ScheduleItem : IAppointment
    {
        public string? Id { get; set; }
        //In UTC
        public DateTime StartTime { get; set; }
        //In UTC
        public DateTime EndTime { get; set; }
        public string AppointmentDescription { get; set; }
        public AppointmentType AppointmentType { get; } = AppointmentType.ScheduleItem;
    }

    public class CustomTimeObject
    {
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }

        public CustomTimeObject(int hour, int minute, int second)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
        }
    }
}
