namespace Client.Model
{
    public class CalendarItem
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Description { get; set; }

        public string Id { get; set; }
        public CalendarItemType Type { get; set; }
    }

    public enum CalendarItemType
    {
        Appointment = 0,
        Availability = 1,
        ScheduleItem = 2
    }
}
