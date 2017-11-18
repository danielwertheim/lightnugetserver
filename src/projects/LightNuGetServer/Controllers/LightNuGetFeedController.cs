using NuGet.Server.Core.Infrastructure;
using NuGet.Server.V2.Controllers;

namespace LightNuGetServer.Controllers
{
    public class LightNuGetFeedController : NuGetODataController
    {
        public LightNuGetFeedController(LightNuGetFeed feed)
            : base(feed.Repository, new ApiKeyPackageAuthenticationService(feed.Settings.RequiresApiKey, feed.Settings.ApiKey))
        {
        }
    }
}