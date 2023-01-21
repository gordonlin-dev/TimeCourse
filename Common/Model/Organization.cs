using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Model
{
    public class Organization
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string OrganizationName { get; set; }
        public string OrganizationDescription { get; set; }
        public string Owner { get; set; }
        public Guid InvitationCode { get; set; }
        public List<OrganizationMembers> Members { get; set; }
    }

    public class OrganizationMembers
    {
        public string UserId { get; set; }
        public string Name { get; set; }
    }
}
