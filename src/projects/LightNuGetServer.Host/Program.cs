using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using Serilog;

namespace LightNuGetServer.Host
{
    public class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            try
            {
                var logConfig = new LoggerConfiguration()
                    .ReadFrom.AppSettings()
                    .Enrich.FromLogContext();

                var logger = Log.Logger = logConfig.CreateLogger();

                logger.Information("Running in Directory='{Directory}' using UserName='{UserName}'",
                    Directory.GetCurrentDirectory(),
                    System.Security.Principal.WindowsIdentity.GetCurrent().Name);

                var host = new LightNuGetServerHostService(
                    GetRequiredAppSetting("host:baseaddress"),
                    GetRequiredAppSetting("lightnugetserver:settingsfilepath"));

                if (Environment.UserInteractive)
                    host.RunAsConsole(args);
                else
                    ServiceBase.Run(host);
            }
            finally
            {
                Log.Logger.Information("Exiting");

                Log.CloseAndFlush();
            }
        }

        private static string GetRequiredAppSetting(string key)
        {
            var value = ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"AppSetting missing for key '{key}'.");

            return value;
        }
    }
}
