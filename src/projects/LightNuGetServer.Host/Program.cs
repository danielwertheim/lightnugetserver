using System;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Hosting;
using Owin;
using Serilog;
using Topshelf;

namespace LightNuGetServer.Host
{
    public class Program
    {
        static void Main(string[] args)
        {
            var logConfig = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .Enrich.FromLogContext();

            if (Environment.UserInteractive)
                logConfig = logConfig.WriteTo.Console();

            Log.Logger = logConfig.CreateLogger();

            var serviceName = GetRequiredAppSetting("service:name");
            var baseAddress = GetRequiredAppSetting("host:baseaddress");
            var serverSettingsFilePath = GetRequiredAppSetting("lightnugetserver:settingsfilepath");

            HostFactory.Run(x =>
            {
                x.UseSerilog();

                x.SetServiceName(serviceName);

                x.Service<LightNuGetService>(s =>
                {
                    s.ConstructUsing(name => new LightNuGetService());
                    s.WhenStarted(tc => tc.Start(baseAddress, serverSettingsFilePath));
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsNetworkService();
            });
        }

        private static string GetRequiredAppSetting(string key)
        {
            var value = ConfigurationManager.AppSettings.Get(key);
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"AppSetting missing for key '{key}'.");

            return value;
        }

        private class LightNuGetService
        {
            private IDisposable _app;

            public void Start(string baseAddress, string jsonSettingsFilePath)
            {
                if (_app != null)
                    throw new InvalidOperationException("Service is already started.");

                _app = WebApp.Start(baseAddress, app =>
                {
                    var settings = LightNugetServerSettingsJsonFactory.Create(jsonSettingsFilePath);
                    var config = new HttpConfiguration();

                    app.UseLightNuGetServer(config, settings, feeds =>
                    {
                        config.Services.Replace(typeof(IHttpControllerActivator), new LightNuGetFeedControllerActivator(feeds));
                    });

                    app.UseWebApi(config);
                });
            }

            public void Stop()
            {
                _app?.Dispose();
                _app = null;
            }
        }
    }
}
