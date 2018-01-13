public class BuildConfig
{
    private const string Version = "0.3.1";

    public readonly string SrcDir = "./src/";
    public readonly string OutDir = "./build/";    
    
    public bool IsPreRelease { get; private set; }
    public string Target { get; private set; }
    public string SemVer { get; private set; }
    public string BuildVersion { get; private set; }
    public string BuildProfile { get; private set; }
    public bool IsTeamCityBuild { get; private set; }
    public string OctoServer { get; private set; }
    public string OctoApiKey { get; private set; }
    
    public static BuildConfig Create(
        ICakeContext context,
        BuildSystem buildSystem)
    {
        if (context == null)
            throw new ArgumentNullException("context");

        var isPreRelease = bool.Parse(context.Argument("prerelease", "False"));
        var buildRevision = context.Argument("buildrevision", "0");

        return new BuildConfig
        {
            IsPreRelease = isPreRelease,
            Target = context.Argument("target", "Default"),
            SemVer = Version + (isPreRelease ? "-pre" + buildRevision : string.Empty),
            BuildVersion = Version + "." + buildRevision,
            BuildProfile = context.Argument("configuration", "Release"),
            IsTeamCityBuild = buildSystem.TeamCity.IsRunningOnTeamCity,
            OctoServer = context.Argument("octoserver", string.Empty),
            OctoApiKey = context.Argument("octoapikey", string.Empty)
        };
    }
}