using certificated_unemi.Data.Repositories;
using certificated_unemi.Interfaces.repositories;

namespace certificated_unemi.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
        {
            /*Regitros de los Servicios*/
            //services.AddScoped<ILoginServices, LoginServices>();
            //services.AddScoped<ITokenServices, TokenServices>();
            

            //services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();





            ///*Regitros de los Repositories*/
            ///
            services.AddScoped<IUserService, UserService>();
            //services.AddScoped<IUserNotificationMovilRepository, UserNotificationMovilRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IGroupRepository, GroupRepository>();




            return services;
        }
    }
}
