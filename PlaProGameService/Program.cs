namespace PlaProGameService
{
    using System;
    using NLog.Web;
    using Logging;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = new Logger();

            try
            {
                logger.Info("Server GameService is RUNNING...");
                BuildWebHost(args).Run();
            }
            catch (Exception ex)
            {
                logger.Error("Server GameService is CRASHED.", ex);
                throw;
            }
            finally
            {
                logger.Warn("Server GameService is STOPPED...\n\n\n");
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseNLog()
                .Build();
    }
}
