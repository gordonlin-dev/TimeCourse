namespace Client.Services
{
    public static class HttpServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpServices(this IServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IAppointmentService, AppointmentService>();
            services.AddSingleton<IOrganizationService, OrganizationService>();
            services.AddScoped<IMicrosoftGraphService, MicrosoftGraphService>();
            return services;
        }
    }
}
