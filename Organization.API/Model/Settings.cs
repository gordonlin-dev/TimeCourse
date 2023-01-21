namespace Organization.API.Model
{
    public class DatabaseSettings
    {
        public string? ConnectionString { get; set; }
        public string? DatabaseName { get; set; }
        public string? OrganizationCollectionName { get; set; }
    }
}
