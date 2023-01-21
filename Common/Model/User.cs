using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Email { get; set; }
        public string Name { get; set; }

        //Ids of Appointments associated with the user
        public List<string> BookedAppointments { get; set; }
        //Ids of Organizations associated with the user
        public List<string> Organizations { get; set; }
    }
}
