using EnsureThat;
using LightNuGetServer.Internals;
using NuGet.Server.Core.Infrastructure;

namespace LightNuGetServer
{
    public class LightNuGetFeed
    {
        public string Name => Settings.Name;
        public string Slug { get; }
        public LightNuGetFeedSettings Settings { get; }
        public IServerPackageRepository Repository { get; }

        public LightNuGetFeed(LightNuGetFeedSettings settings, IServerPackageRepository repository)
        {
            Ensure.Any.IsNotNull(settings, nameof(settings));
            Ensure.Any.IsNotNull(repository, nameof(repository));

            Slug = settings.Name.Slugify();
            Settings = settings;
            Repository = repository;
        }
    }
}