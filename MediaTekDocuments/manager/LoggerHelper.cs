using Serilog;
using System;
using System.IO;

namespace MediaTekDocuments.bddmanager
{
    public static class LoggerHelper
    {
        public static void Initialize()
        {
            string logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MediatekDocuments", "logs");
            Directory.CreateDirectory(logDir);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(logDir, "log.txt"), rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Logger initialisé.");
        }
    }
}
