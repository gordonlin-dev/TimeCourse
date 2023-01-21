using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Organization.API.Model;
using Shared.Services;

namespace Organization.API.Services
{
    public interface IOrganizationService
    {
        public Task<string> CreateOrganization(string userId, Shared.Model.Organization org);
        public Task UpdateOrganization(string userId, Shared.Model.Organization org);

        public Task<string> JoinOrganization(string userId, string userName, Guid invitationCode);

        public Task<Shared.Model.Organization> RemoveUserFromOrganization(string ownerId, string orgId, string memberId);

        public Task<List<Shared.Model.Organization>> GetOrganizations(string UserId, List<string> orgIds);
    }
    public class OrganizationService : IOrganizationService
    {
        private IMongoDBService _mongoDBService;
        private IMongoCollection<Shared.Model.Organization> _organizationCollection;
        public OrganizationService(IMongoDBService mongoDBService, IOptions<DatabaseSettings> settings)
        {
            _mongoDBService = mongoDBService;
            _organizationCollection = _mongoDBService.GetDatabase().GetCollection<Shared.Model.Organization>(settings.Value.OrganizationCollectionName);
        }

        public async Task<string> CreateOrganization(string userId, Shared.Model.Organization org)
        {
            org.Owner = userId;
            org.Members = new List<string>();
            org.InvitationCode = Guid.NewGuid();
            await _organizationCollection.InsertOneAsync(org);
            return org.Id;
        }

        public async Task UpdateOrganization(string userId, Shared.Model.Organization org)
        {
            await _organizationCollection.FindOneAndReplaceAsync(x => x.Owner == userId, org);
        }

        public async Task<List<Shared.Model.Organization>> GetOrganizationsByUser(string userId)
        {
            var result = await _organizationCollection.FindAsync(x => x.Owner == userId);
            return result.ToList();
        }

        public async Task<string> JoinOrganization(string userId, string userName, Guid invitationCode)
        {
            var result = await _organizationCollection.FindAsync(x => x.InvitationCode == invitationCode);
            var org = result.First();
            if(org.Owner == userId)
            {
                return string.Empty;
            }
            org.Members.Add(userId);
            await _organizationCollection.FindOneAndReplaceAsync(x => x.Id == org.Id, org);
            return org.Id;
        }

        public async Task<Shared.Model.Organization> RemoveUserFromOrganization(string ownerId, string orgId, string memberId)
        {
            var result = await _organizationCollection.FindAsync(x => x.Owner == ownerId && x.Id == orgId);
            var org = result.First();
            var member = org.Members.Find(x => x == memberId);
            org.Members.Remove(member);
            await _organizationCollection.FindOneAndReplaceAsync(x => x.Id == org.Id, org);
            return org;
        }

        public async Task<List<Shared.Model.Organization>> GetOrganizations(string userId, List<string> orgIds)
        {
            var result = await _organizationCollection.FindAsync(x => orgIds.Contains(x.Id));
            var sanitzed = new List<Shared.Model.Organization>();
            foreach (var item in result.ToList())
            {
                if(item.Owner != userId) {
                    item.Members = null;
                    item.InvitationCode = Guid.Empty;
                }
                sanitzed.Add(item);
            }
            return sanitzed;
        }
    }
}
