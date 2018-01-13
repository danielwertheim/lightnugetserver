#addin nuget:?package=Cake.Git&version=0.16.1
#tool nuget:?package=OctopusTools&version=4.29.0

#load "./buildconfig.cake"

var config = BuildConfig.Create(Context, BuildSystem);
var deployableProjects = new [] { "LightNuGetServer.Host" };

Information($"SrcDir: {config.SrcDir}");
Information($"OutDir: {config.OutDir}");
Information($"SemVer: {config.SemVer}");
Information($"IsPreRelease: {config.IsPreRelease}");
Information($"BuildVersion: {config.BuildVersion}");
Information($"BuildProfile: {config.BuildProfile}");
Information($"IsTeamCityBuild: {config.IsTeamCityBuild}");

Task("Default")
    .IsDependentOn("InitOutDir")
    .IsDependentOn("Restore")
    .IsDependentOn("AssemblyVersion")
    .IsDependentOn("Build");

Task("CI")
    .IsDependentOn("Default")
    .IsDependentOn("Pack");
/********************************************/
Task("InitOutDir").Does(() => {
    EnsureDirectoryExists(config.OutDir);
    CleanDirectory(config.OutDir);
});

Task("Restore").Does(() => {
    foreach(var sln in GetFiles($"{config.SrcDir}*.sln")) {
        NuGetRestore(sln);
    }
});

Task("AssemblyVersion").Does(() => {
    var gitInfo = GitLogTip("./");
    var version = config.BuildVersion;
    var fileVersion = config.BuildVersion;
    var informationalVersion = $"{config.BuildVersion} gitsha:{gitInfo.Sha}";

    Information($"Version: {version}");
    Information($"FileVersion: {fileVersion}");
    Information($"InformationalVersion: {informationalVersion}");

    CreateAssemblyInfo($"{config.SrcDir}GlobalAssemblyVersion.cs", new AssemblyInfoSettings {
        Version = version,
        FileVersion = fileVersion,
        InformationalVersion = informationalVersion
    });
});
    
Task("Build").Does(() => {
    foreach(var sln in GetFiles($"{config.SrcDir}*.sln")) {
        MSBuild(sln, new MSBuildSettings {
            Verbosity = Verbosity.Minimal,
            ToolVersion = MSBuildToolVersion.VS2017,
            Configuration = config.BuildProfile,
            PlatformTarget = PlatformTarget.MSIL
        }
        .SetMSBuildPlatform(MSBuildPlatform.Automatic)
        .WithTarget("Rebuild")
        .WithWarningsAsError());
    }
});

Task("Pack").Does(() => {
    foreach(var proj in deployableProjects) {
        var srcDir = $"{config.SrcDir}projects/{proj}/bin/{config.BuildProfile}";
        var trgDir = $"{config.OutDir}{proj}.{config.SemVer}.zip";

        DeleteFiles($"{srcDir}/*.pdb");
        DeleteFiles($"{srcDir}/*.xml");
        Zip(srcDir, trgDir);
    }
});

RunTarget(config.Target);