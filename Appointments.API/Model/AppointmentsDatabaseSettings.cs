using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Appointments.API.Model
{
    public class AppointmentsDatabaseSettings
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? AppointmentsCollectionName { get; set; }
        public string? AvailabilitiesCollectionName { get; set; }
    }
}
