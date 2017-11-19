using System;
using System.IO;
using System.Linq;
using System.Web.Http;
using EnsureThat;
using LightNuGetServer.Internals;
using NuGet.Server.V2;
using Owin;
using Serilog;

namespace LightNuGetServer
{
    public static class LightNuGetServerActivator
    {
        public static IAppBuilder UseLightNuGetServer(
            this IAppBuilder app,
            HttpConfiguration config,
            LightNuGetServerSettings settings,
            Action<LightNuGetFeed[]> cfg = null)
        {
            Ensure.Any.IsNotNull(app, nameof(app));
            Ensure.Any.IsNotNull(config, nameof(config));
            Ensure.Any.IsNotNull(settings, nameof(settings));

            var logger = new SerilogProxyLogger(Log.Logger);

            var feeds = settings.Feeds
                .Select(fs => new LightNuGetFeed(fs, NuGetV2WebApiEnabler.CreatePackageRepository(Path.Combine(settings.PackagesDirPath, fs.Name), fs, logger)))
                .ToArray();

            cfg?.Invoke(feeds);

            foreach (var feed in feeds)
            {
                config.UseNuGetV2WebApiFeed(
                    routeName: feed.Name,
                    routeUrlRoot: feed.Slug,
                    oDatacontrollerName: "LightNuGetFeed");
            }

            return app;
        }
    }
}