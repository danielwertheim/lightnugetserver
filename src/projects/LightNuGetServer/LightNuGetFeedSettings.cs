using NuGet.Server.Core.Infrastructure;
using System;
using System.Collections.Generic;

namespace LightNuGetServer
{
    public class LightNuGetFeedSettings : ISettingsProvider
    {
        public string Name { get; set; } = "Default";
        public string ApiKey { get; set; }
        public bool RequiresApiKey { get; set; } = true;
        public Dictionary<string, object> NuGetServerSettings { get; set; } = new Dictionary<string, object>();

        bool ISettingsProvider.GetBoolSetting(string key, bool defaultValue) => NuGetServerSettings.TryGetValue(key, out var tmp)
            ? Convert.ToBoolean(tmp)
            : defaultValue;

        string ISettingsProvider.GetStringSetting(string key, string defaultValue) => NuGetServerSettings.TryGetValue(key, out var tmp)
            ? tmp as string
            : defaultValue;
    }
}