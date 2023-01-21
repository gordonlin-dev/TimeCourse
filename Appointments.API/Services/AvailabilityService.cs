using Appointments.API.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Model;
using Shared.Services;

namespace Appointments.API.Services
{
    public interface IAvailabilityService
    {
        public Task CreateAvailability(string userId, Availability availability);
        public Task UpdateAvailability(string userId, Availability availability);
        public Task DeleteAvailability(string userId, string availabilityId);
        public Task UpdateAppointmentInAvailability(string availabilityId, Appointment appointment);

        public Task CancelAppointmentInAvailability(string availabilityId, Appointment appointment);
        public Task<List<Availability>> GetAvailabilitiesByMonth(string userId, int year, int month);
        public Task<List<Availability>> GetAvailabilitiesByDate(string userId, int year, int month, int day);

        public Task<List<Availability>> GetAvailabilitiesByDateRange(string userId, DateTime start, DateTime end);
        public Task<List<Availability>> GetAvailabilitiesByMonth(string userId, List<string> orgIds, int year, int month);
        public Task<List<Availability>> GetAvailabilitiesByDate(string userId, List<string> orgIds, int year, int month, int day);
        public Task<List<Availability>> GetAvailabilitiesByDateRange(string userId, List<string> orgIds, DateTime start, DateTime end);
    }
    public class AvailabilityService : IAvailabilityService
    {
        private IMongoDBService _mongoDBService;
        private IMongoCollection<Availability> _availabilityCollection;

        public AvailabilityService(IMongoDBService mongoDBService, IOptions<AppointmentsDatabaseSettings> settings)
        {
            _mongoDBService = mongoDBService;
            _availabilityCollection = _mongoDBService.GetDatabase().GetCollection<Availability>(settings.Value.AvailabilitiesCollectionName);
        }

        public async Task<List<Availability>> GetAvailabilitiesByMonth(string userId, int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var query = await _availabilityCollection.FindAsync(x => x.HostId == userId 
                && x.StartTime >= monthStart && x.StartTime < monthEnd);
            var result = query.ToList();
            result.ForEach(x => x.Appointments = x.Appointments.Where(y => y.OwnerId == null || y.OwnerId == string.Empty).ToList());
            return result;
        }

        public async Task<List<Availability>> GetAvailabilitiesByDate(string userId, int year, int month, int day)
        {
            var dateStart = new DateTime(year, month, day);
            var dateEnd = dateStart.AddDays(1);
            var query = await _availabilityCollection.FindAsync(x => x.HostId == userId
                && x.StartTime >= dateStart && x.StartTime < dateEnd);
            var result = query.ToList();
            result.ForEach(x => x.Appointments = x.Appointments.Where(y => y.OwnerId == null || y.OwnerId == string.Empty).ToList());
            return result;
        }

        public async Task<List<Availability>> GetAvailabilitiesByDateRange(string userId, DateTime start, DateTime end)
        {
            var query = await _availabilityCollection.FindAsync(x => x.HostId == userId && start <= x.StartTime && x.StartTime <= end);
            var result = query.ToList();
            result.ForEach(x => x.Appointments = x.Appointments.Where(y => y.OwnerId == null || y.OwnerId == string.Empty).ToList());
            return result;
        }

        public async Task<List<Availability>> GetAvailabilitiesByMonth(string userId, List<string> orgIds, int year, int month)
        {
            var monthStart = new DateTime(year, month, 1);
            var monthEnd = monthStart.AddMonths(1);
            var query = await _availabilityCollection.FindAsync(x => orgIds.Contains(x.HostOrganizationId) && x.HostId != userId
                && x.StartTime >= monthStart && x.StartTime <= monthEnd);
            var result = query.ToList();
            result.ForEach(x => x.Appointments = x.Appointments.Where(y => y.OwnerId == null || y.OwnerId == string.Empty).ToList());
            return result;
        }

        

        public async Task<List<Availability>> GetAvailabilitiesByDate(string userId, List<string> orgIds, int year, int month, int day)
        {
            var dateStart = new DateTime(year, month, day);
            var dateEnd = dateStart.AddDays(1);
            var query = await _availabilityCollection.FindAsync(x => orgIds.Contains(x.HostOrganizationId) && x.HostId != userId
                && x.StartTime >= dateStart && x.StartTime < dateEnd);
            var result = query.ToList();
            result.ForEach(x => x.Appointments = x.Appointments.Where(y => y.OwnerId == null || y.OwnerId == string.Empty).ToList());
            return result;
        }

        public async Task<List<Availability>> GetAvailabilitiesByDateRange(string userId, List<string> orgIds, DateTime start, DateTime end)
        {
            var query = await _availabilityCollection.FindAsync(x => orgIds.Contains(x.HostOrganizationId) 
                && x.HostId != userId && start <= x.StartTime && x.StartTime <= end);
            var result = query.ToList();
            result.ForEach(x => x.Appointments = x.Appointments.Where(y => y.OwnerId == null || y.OwnerId == string.Empty).ToList());
            return result;
        }

        public async Task UpdateAvailability(string userId, Availability availability)
        {
            var query = await _availabilityCollection.FindAsync(x => x.HostId == userId && x.Id == availability.Id);
            var documentId = query.First().Id;
            await _availabilityCollection.ReplaceOneAsync(x => x.Id == documentId, availability);
        }

        public async Task DeleteAvailability(string userId, string availabilityId)
        {
            var query = await _availabilityCollection.FindAsync(x => x.HostId == userId && x.Id == availabilityId);
            if(query.First() != null)
            {
                await _availabilityCollection.DeleteOneAsync(x => x.Id == availabilityId);
            }
        }

        public async Task UpdateAppointmentInAvailability(string availabilityId, Appointment appointment)
        {
            var query = await _availabilityCollection.FindAsync(x => x.Id == availabilityId);
            var availability = query.First();
            var index = availability.Appointments.IndexOf(availability.Appointments.Where(x => x.StartTime == appointment.StartTime && x.EndTime == appointment.EndTime).First());
            availability.Appointments[index] = appointment;
            await _availabilityCollection.ReplaceOneAsync(x => x.Id == availability.Id, availability);
        }

        public async Task CancelAppointmentInAvailability(string availabilityId, Appointment appointment)
        {
            var query = await _availabilityCollection.FindAsync(x => x.Id == availabilityId);
            var availability = query.First();
            var index = availability.Appointments.IndexOf(availability.Appointments.Where(x => x.StartTime == appointment.StartTime && x.EndTime == appointment.EndTime).First());
            availability.Appointments[index].OwnerId = null;
            await _availabilityCollection.ReplaceOneAsync(x => x.Id == availability.Id, availability);
        }

        public async Task CreateAvailability(string userId, Availability availability)
        {
            var inserts = new List<Availability>(); 
            var recurranceId = Guid.NewGuid();
            availability.RecurranceId = recurranceId;
            availability.HostId = userId;
            availability.Appointments = GenerateAppointmentSlotsForAvailability(availability);
            inserts.Add(availability);
            if (availability.AvailabilityRecurrance.HasValue && availability.RecurranceEndDate.HasValue)
            {
                var curStartDatetime = AddTimeBasedOnRecurranceOption(availability.StartTime, availability.AvailabilityRecurrance.Value);
                var curEndDatetime = AddTimeBasedOnRecurranceOption(availability.EndTime, availability.AvailabilityRecurrance.Value);
                while (curStartDatetime.Date <= availability.RecurranceEndDate.Value.Date)
                {
                    var newRecurrance = new Availability()
                    {
                        HostId = userId,
                        HostOrganizationId = availability.HostOrganizationId,
                        StartTime = curStartDatetime,
                        EndTime = curEndDatetime,
                        Description = availability.Description,
                        AppointmentDuration = availability.AppointmentDuration,
                        AvailabilityRecurrance = availability.AvailabilityRecurrance,
                        RecurranceEndDate = availability.RecurranceEndDate,
                        RecurranceId = recurranceId,
                        Appointments = GenerateAppointmentSlotsForAvailability(availability)
                    };
                    inserts.Add(newRecurrance);
                    curStartDatetime = AddTimeBasedOnRecurranceOption(curStartDatetime, availability.AvailabilityRecurrance.Value);
                    curEndDatetime = AddTimeBasedOnRecurranceOption(curEndDatetime, availability.AvailabilityRecurrance.Value);
                }
            }           
            await _availabilityCollection.InsertManyAsync(inserts);
        }
        private static List<Appointment> GenerateAppointmentSlotsForAvailability(Availability availability)
        {
            List<Appointment> results = new List<Appointment>();
            var appointmentStartTime = availability.StartTime;
            var appointmentEndTime = availability.StartTime.AddMinutes(availability.AppointmentDuration);
            while(appointmentEndTime <= availability.EndTime)
            {
                results.Add(new Appointment()
                {
                    StartTime = appointmentStartTime,
                    EndTime = appointmentEndTime
                });
                appointmentStartTime = appointmentEndTime;
                appointmentEndTime = appointmentEndTime.AddMinutes(availability.AppointmentDuration);
            }
            return results;
        }
        private static DateTime AddTimeBasedOnRecurranceOption(DateTime dateTime, AvailabilityRecurrance recurrenceOption)
        {
            DateTime newTime;
            switch (recurrenceOption)
            {
                case AvailabilityRecurrance.Daily:
                    newTime = dateTime.AddDays(1);
                    break;
                case AvailabilityRecurrance.Weekly:
                    newTime = dateTime.AddDays(7);
                    break;
                case AvailabilityRecurrance.Monthly:
                    newTime = dateTime.AddMonths(1);
                    break;
                default:
                    newTime = dateTime;
                    break;
            }
            return newTime;
        }
    }
}
