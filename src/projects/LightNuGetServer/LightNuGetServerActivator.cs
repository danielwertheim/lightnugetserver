using System;
using System.Collections.Generic;
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
            Action<IDictionary<string, LightNuGetFeed>> cfg = null)
        {
            Ensure.Any.IsNotNull(app, nameof(app));
            Ensure.Any.IsNotNull(config, nameof(config));
            Ensure.Any.IsNotNull(settings, nameof(settings));

            var logger = new SerilogProxyLogger(Log.Logger);

            var feeds = settings.Feeds
                .Select(fs => new LightNuGetFeed(fs, NuGetV2WebApiEnabler.CreatePackageRepository(Path.Combine(settings.PackagesDirPath, fs.Name), fs, logger)))
                .ToDictionary(f => f.Key);

            cfg?.Invoke(feeds);

            foreach (var feed in feeds.Values.Select(f => f.Settings))
            {
                config.UseNuGetV2WebApiFeed(
                    routeName: feed.Name,
                    routeUrlRoot: feed.RelativeUrl,
                    oDatacontrollerName: "LightNuGetFeed");
            }

            return app;
        }
    }
}