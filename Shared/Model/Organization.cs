using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
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
        public List<string> Members { get; set; }
    }
}
