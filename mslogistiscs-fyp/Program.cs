
using MSLogistics.Application.Logging.Providers;
using mslogistiscs_fyp;

namespace UNITY_Lite.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        var env = hostingContext.HostingEnvironment;
                        config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                    });

                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    // clear default logging providers
                    logging.ClearProviders();

                    // Create log file directory if it doesn't exist
                    var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                    if (!Directory.Exists(logFilePath))
                    {
                        Directory.CreateDirectory(logFilePath);
                    }
                    // File Limit
                    long maxLogFileSize = 50 * 1024 * 1024; // 50MB 

                    // Custom error log file
                    string logErrorFilePathWithFileName = Path.Combine(logFilePath, "Errors.log");

                    if (!File.Exists(logErrorFilePathWithFileName))
                    {
                        File.Create(logErrorFilePathWithFileName).Close();
                    }

                    var errorFileLoggerProvider = new ErrorFileLoggerProvider(logErrorFilePathWithFileName, maxLogFileSize);
                    logging.AddProvider(errorFileLoggerProvider);
                });
    }
}
