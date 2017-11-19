using System.Collections.Generic;
using NuGet.Server.Core.Infrastructure;

namespace LightNuGetServer
{
    public class LightNuGetFeedSettings : ISettingsProvider
    {
        public string Name { get; set; } = "Default";
        public string ApiKey { get; set; }
        public bool RequiresApiKey { get; set; } = true;
        public Dictionary<string, bool> NuGetServerSettings { get; set; } = new Dictionary<string, bool>();

        bool ISettingsProvider.GetBoolSetting(string key, bool defaultValue)
            => NuGetServerSettings.TryGetValue(key, out var tmp) ? tmp : defaultValue;
    }
}