using EnsureThat;
using NuGet.Server.Core.Infrastructure;

namespace LightNuGetServer
{
    public class LightNuGetFeed
    {
        public string Key { get; }
        public LightNuGetFeedSettings Settings { get; }
        public IServerPackageRepository Repository { get; }

        public LightNuGetFeed(LightNuGetFeedSettings settings, IServerPackageRepository repository)
        {
            Ensure.Any.IsNotNull(settings, nameof(settings));
            Ensure.Any.IsNotNull(repository, nameof(repository));

            Key = $"/{settings.RelativeUrl.Trim('/')}/";
            Settings = settings;
            Repository = repository;
        }
    }
}