namespace LightNuGetServer
{
    public class LightNuGetServerSettings
    {
        public string PackagesDirPath { get; set; } = "Packages";
        public LightNuGetFeedSettings[] Feeds { get; set; } = new LightNuGetFeedSettings[0];
    }
}