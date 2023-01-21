using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Services
{
    public interface IUserService
    {
        public Task<User?> GetUserAsync(string id);
        public Task<List<User>> GetUsersAsync(string[] ids);
        public Task<User?> GetUserByEmailAsync(string email);
        public Task<User> CreateUserAsync(string email, string name);
        public Task<User> UpdateUserAsync(User user);
        public Task RemoveUserAsync(string id);

        public Task AddOrganization(string userId, string orgId);
        public Task RemoveOrganization(string userId, string orgId);
        public Task<List<string>> GetOrganizations(string userId);
        public Task AddAppointments(string userId, List<Appointment> appointments);
        public Task RemoveAppointments(string userId, List<Appointment> appointments);

        public Task<List<string>> GetAppointmentIdsByMonth(string userId, int year, int month);

        public Task<List<string>> GetAppointmentIdsByDate(string userId, int year, int month, int day);
    }

    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserService(IOptions<UserDatabaseSettings> userDatabaseSettings, IMongoDBService mongoDBService)
        {
            _userCollection = mongoDBService.GetDatabase().GetCollection<User>(userDatabaseSettings.Value.UserCollectionName);
        }

        public async Task AddOrganization(string userId, string orgId)
        {
            var result = await _userCollection.FindAsync(x => x.Id == userId);
            var user = result.First();
            user.Organizations.Add(orgId);
            await _userCollection.FindOneAndReplaceAsync(x => x.Id == user.Id, user);
        }

        public async Task<User> CreateUserAsync(string email, string name)
        {
            var newUser = new User()
            {
                Name = name,
                Email = email,
                Organizations = new List<string>(),
                Appointments = new List<UserAppointments>(),

            };
            await _userCollection.InsertOneAsync(newUser);
            return newUser;
        }

        public async Task<List<string>> GetOrganizations(string userId)
        {
            var result = await _userCollection.FindAsync(x => x.Id == userId);
            return result.First().Organizations;
        }

        public async Task<User?> GetUserAsync(string id)
        {
            return await _userCollection.Find(x => x.Id == id).FirstAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var query = await _userCollection.FindAsync(x => x.Email == email);
            var result = query.ToList();

            return result.Any() ? result.First() : null;
        }

        public async Task<List<User>> GetUsersAsync(string[] ids)
        {
            var result = await _userCollection.FindAsync(x => ids.Contains(x.Id));
            return result.ToList();
        }

        public async Task RemoveOrganization(string userId, string orgId)
        {
            var result = await _userCollection.FindAsync(x => x.Id == userId);
            var user = result.First();
            user.Organizations.Remove(orgId);
            await _userCollection.FindOneAndReplaceAsync(x => x.Id == user.Id, user);
        }

        public async Task RemoveUserAsync(string id)
        {
            await _userCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            await _userCollection.ReplaceOneAsync(x => x.Id == user.Id, user);
            return user;
        }

        public async Task AddAppointments(string userId, List<Appointment> appointments)
        {
            var result = await _userCollection.FindAsync(x => x.Id == userId);
            var user = result.First();
            var userAppointments = user.Appointments;
            foreach(var appointment in appointments)
            {
                var query = userAppointments.Where(x => x.Year == appointment.StartTime.Year && x.Month == appointment.StartTime.Month);
                if (query.Any())
                {
                    var userAppointment = query.First();
                    userAppointment.Appointments.Add(new UserAppointmentItem() { Id = appointment.Id, DayOfMonth = appointment.StartTime.Day});
                }
                else
                {
                    var userAppointment = new UserAppointments()
                    {
                        Year = appointment.StartTime.Year,
                        Month = appointment.StartTime.Month,
                        Appointments = new List<UserAppointmentItem>() { new UserAppointmentItem() { Id = appointment.Id, DayOfMonth = appointment.StartTime.Day } }
                    };
                    userAppointments.Add(userAppointment);
                }
            }

            await _userCollection.ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        public async Task RemoveAppointments(string userId, List<Appointment> appointments)
        {
            var result = await _userCollection.FindAsync(x => x.Id == userId);
            var user = result.First();
            foreach(var appointment in appointments)
            {
                var userAppointment = user.Appointments.Where(x => x.Year == appointment.StartTime.Year && x.Month == appointment.StartTime.Month).First();
                userAppointment.Appointments.RemoveAll(x => x.DayOfMonth == appointment.StartTime.Day);
            }           
            await _userCollection.ReplaceOneAsync(x => x.Id == user.Id, user);
        }

        public async Task<List<string>> GetAppointmentIdsByMonth(string userId, int year, int month)
        {
            var query = await _userCollection.FindAsync(x => x.Id == userId);
            var user = query.First();
            var result = user.Appointments.Where(x => x.Year == year && x.Month == month);
            if (result.Any())
            {
                return result.First().Appointments.Select(x => x.Id).ToList();
            }
            else
            {
                return new List<string>();
            }
        }

        public async Task<List<string>> GetAppointmentIdsByDate(string userId, int year, int month, int day)
        {
            var query = await _userCollection.FindAsync(x => x.Id == userId);
            var user = query.First();
            var result = user.Appointments.Where(x => x.Year == year && x.Month == month);
            if (result.Any())
            {
                return result.First().Appointments.Where(x => x.DayOfMonth == day).Select(x => x.Id).ToList();
            }
            else
            {
                return new List<string>();
            }
        }
    }
}
