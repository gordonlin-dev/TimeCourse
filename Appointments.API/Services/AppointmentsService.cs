using Appointments.API.Model;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Model;
using Shared.Services;

namespace Appointments.API.Services
{
    public interface IAppointmentsService
    {
        public Task<List<Appointment>> GetAppointments(string userId, List<string> Ids);
        public Task<List<Appointment>> CreateAppointments(string userId, List<Appointment> appointments);
        public Task<List<Appointment>> GetAppointmentsByDateRange(string userId, DateTime start, DateTime end);

        public Task UpdateAppointment(string userId, List<Appointment> appointment);

        public Task DeleteAppointment(string userId, string appointmentId);
    }
    public class AppointmentsService : IAppointmentsService
    {
        private IMongoDBService _mongoDBService;
        private IMongoCollection<Appointment> _appointmentCollection;
        public AppointmentsService(IMongoDBService mongoDBService, IOptions<AppointmentsDatabaseSettings> settings)
        {
            _mongoDBService = mongoDBService;
            _appointmentCollection = _mongoDBService.GetDatabase().GetCollection<Appointment>(settings.Value.AppointmentsCollectionName);
        }

        public async Task<List<Appointment>> GetAppointmentsByDateRange(string userId, DateTime start, DateTime end)
        {
            var query = await _appointmentCollection.FindAsync(x => x.OwnerId == userId && start <= x.StartTime && x.StartTime <= end);
            return query.ToList();
        }

        public async Task<List<Appointment>> GetAppointments(string userId, List<string> Ids)
        {
            var query = await _appointmentCollection.FindAsync(x => Ids.Contains(x.Id) && x.OwnerId == userId);
            return query.ToList();
        }

        public async Task<List<Appointment>> CreateAppointments(string userId, List<Appointment> appointments)
        {
            var insertedAppointments = new List<Appointment>();
            var scheduleItems = new List<Appointment>();
            var appts = new List<Appointment>();
            foreach(var appointment in appointments)
            {
                appointment.OwnerId = userId;
                if(appointment.AppointmentType == AppointmentType.Appointment)
                {
                    appts.Add(appointment);
                }
                else
                {
                    scheduleItems.Add(appointment);
                }
            }
            if(appts.Count > 0)
            {
                await _appointmentCollection.InsertManyAsync(appts);
                insertedAppointments.AddRange(appts);
            }
            
            if(scheduleItems.Count > 0)
            {
                var existingScheduleItems = await _appointmentCollection.FindAsync(x => x.OwnerId == userId && 
                    x.AppointmentType == AppointmentType.ScheduleItem);
                var existingScheduleItemsList = existingScheduleItems.ToList();
                var distinctItems = new List<Appointment>();
                foreach(var item in scheduleItems)
                {
                    var found = existingScheduleItemsList.Find(x => x.StartTime == item.StartTime
                        && x.EndTime == item.EndTime && x.Description == item.Description);
                    if (found is null)
                    {
                        distinctItems.Add(item);
                    }
                }
                if(distinctItems.Count > 0)
                {
                    await _appointmentCollection.InsertManyAsync(distinctItems);
                    insertedAppointments.AddRange(distinctItems);
                }                
            }
            return insertedAppointments;
        }

        public async Task DeleteAppointment(string userId, string appointmentId)
        {
            await _appointmentCollection.DeleteOneAsync(x => x.OwnerId == userId && x.Id == appointmentId);
        }

        public async Task UpdateAppointment(string userId, List<Appointment> appointments)
        {
            foreach(var appointment in appointments)
            {
                await _appointmentCollection.ReplaceOneAsync(x => x.Id == appointment.Id, appointment);
            }            
        }
    }
}
