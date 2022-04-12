using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenStockApp.Extensions
{
     /// <summary>
     /// Extensions for the IHost
     /// </summary>
    public static class HostApplicationExtensions
    {
        /// <summary>
        /// Migrates the given database using EF core.
        /// It's important to use MAuiApp and not IHost since Maui has removed IHost from the MauiApp in the RC.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public static MauiApp MigrateDatabase<T>(this MauiApp app) where T : DbContext
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<T>();
                    if (db != null)
                    {
                        //db.Database.EnsureCreated();
                        db.Database.Migrate();
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"{ex}");

                    //var logger = services.GetRequiredService<ILogger>();
                    //logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
            return app;
        }
    }
}
