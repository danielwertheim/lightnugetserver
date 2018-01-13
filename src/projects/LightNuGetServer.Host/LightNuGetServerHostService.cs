using System;
using System.ServiceProcess;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using EnsureThat;
using Microsoft.Owin.Hosting;
using Owin;
using Serilog;

namespace LightNuGetServer.Host
{
    public class LightNuGetServerHostService : ServiceBase
    {
        private readonly ILogger _logger = Log.ForContext<LightNuGetServerHostService>();

        private readonly string _baseAddress;
        private readonly string _jsonSettingsFilePath;
        private IDisposable _app;

        public LightNuGetServerHostService(string baseAddress, string jsonSettingsFilePath)
        {
            Ensure.String.IsNotNullOrWhiteSpace(baseAddress, nameof(baseAddress));
            Ensure.String.IsNotNullOrWhiteSpace(jsonSettingsFilePath, nameof(jsonSettingsFilePath));

            _baseAddress = baseAddress;
            _jsonSettingsFilePath = jsonSettingsFilePath;
        }

        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            _logger.Information(
                "Starting service. BaseAddress='{BaseAddress}' JsonSettingsFile='{JsonSettingsFile}'",
                _baseAddress, _jsonSettingsFilePath);

            if (_app != null)
                throw new InvalidOperationException("Service is already started.");

            _app = WebApp.Start(_baseAddress, app =>
            {
                _logger.Information("Configuring WebApi.");

                var settings = LightNugetServerSettingsJsonFactory.Create(_jsonSettingsFilePath);
                var config = new HttpConfiguration();

                app.UseLightNuGetServer(config, settings, feeds =>
                {
                    config.Services.Replace(typeof(IHttpControllerActivator), new LightNuGetFeedControllerActivator(feeds));
                });

                app.UseWebApi(config);
            });

            _logger.Information("Service started.");
        }

        protected override void OnStop()
        {
            _logger.Information("Stopping service.");

            _app?.Dispose();
            _app = null;

            _logger.Information("Service stopped.");
        }
    }
}