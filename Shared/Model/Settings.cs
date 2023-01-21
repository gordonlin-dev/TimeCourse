using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model
{
    public class MongoDbSettings
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
    }

    public class UserDatabaseSettings
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? UserCollectionName { get; set; }
    }

    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
    }

    public class APIOptions
    {
        public class UserApi
        {
            public string BaseUrl { get; set; }
        }

        public class AppointmentsApi
        {
            public string BaseUrl { get; set; }
        }
        public class OrganizationApi
        {
            public string BaseUrl { get; set; }
        }
    }
}
