using Microsoft.EntityFrameworkCore;
using BackEnd.Data;
using BackEnd.Interfaces;

namespace BackEnd.Services
{
    public static class DatabaseStartup
    {

        public static readonly ILoggerFactory ConsoleLogFactory
                        = LoggerFactory.Create(builder => { builder.AddConsole(); });
        /// <summary>
        /// It is used to configure the connection properties for the database using centralized KeyVault service
        /// </summary>
        /// <param name="builder"></param>
        public static void ConfigureDatabase(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var keyVaultService = serviceProvider.GetRequiredService<IKeyVaultService>();
                
                options.UseSqlServer(
                    keyVaultService.DbConnectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);

                    });
                #if DEBUG
                options.UseLoggerFactory(ConsoleLogFactory);
                options.EnableSensitiveDataLogging();
                #endif
            });
        }
    }
}